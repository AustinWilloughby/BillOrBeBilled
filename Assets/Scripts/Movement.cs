using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float turnAcceleration = 8f;
    [SerializeField] private float drag = 2f;
    [SerializeField] private float axleWidth = 1f;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private LayerMask groundLayer = -1;

    [Header("Collision Detection")]
    [SerializeField] private LayerMask wallLayers = -1;

    [Header("Lean Settings")]
    [SerializeField] private float maxLeanAngle = 15f;
    [SerializeField] private float leanSpeed = 5f;
    [SerializeField] private float terrainAlignmentSpeed = 5f;

    [Header("Visual Wheels")]
    [SerializeField] private Transform leftSuspension;
    [SerializeField] private Transform leftWheelSpinner;
    [SerializeField] private Transform rightSuspension;
    [SerializeField] private Transform rightWheelSpinner;
    [SerializeField] private float wheelRadius = 0.3f;
    [SerializeField] private float maxSteerAngle = 10f;

    [Header("Energy Cost")]
    [SerializeField] private float energyCostScaler = 0.1f;

    private Rigidbody rb;
    private Vector2 moveInput;

    // Axle simulation
    private Vector3 leftWheelVelocity;
    private Vector3 rightWheelVelocity;

    // Lean tracking
    private float currentLeanAngle;
    private Quaternion currentSurfaceRotation;

    // Wheel rotation tracking
    private float leftWheelRotation;
    private float rightWheelRotation;
    private Quaternion leftSuspensionBaseRotation;
    private Quaternion rightSuspensionBaseRotation;

    private float stopDirection = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Movement script requires a Rigidbody component!");
            return;
        }

        // Configure rigidbody for kinematic movement
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Initialize surface rotation to identity (flat ground)
        currentSurfaceRotation = Quaternion.identity;

        // Store base suspension rotations to preserve X and Z
        if (leftSuspension != null)
            leftSuspensionBaseRotation = leftSuspension.localRotation;
        if (rightSuspension != null)
            rightSuspensionBaseRotation = rightSuspension.localRotation;
    }

    private void Update()
    {
        if (Energy.Instance.CurrentBattery <= 0)
        {
            Energy.Instance.gameOverScreen.SetActive(true);
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        SimulateAxle();
        UpdateRigidbody();
        UpdateVisualWheels();

        stopDirection = 0;
    }

    private void SimulateAxle()
    {
        // Get forward direction based on current rotation
        Vector3 forward = transform.forward;

        // Calculate target velocities for each wheel based on input
        float forwardInput = moveInput.y;
        float turnInput = moveInput.x;

        // Apply acceleration to both wheels for forward/backward movement
        Vector3 forwardAccel = forward * forwardInput * acceleration * Time.fixedDeltaTime;
        leftWheelVelocity += forwardAccel;
        rightWheelVelocity += forwardAccel;

        // Apply differential acceleration for turning
        // Turning right (positive X) = speed up left wheel, slow down right wheel
        Vector3 turnAccel = forward * turnInput * turnAcceleration * Time.fixedDeltaTime;
        leftWheelVelocity += turnAccel;
        rightWheelVelocity -= turnAccel;

        // Constrain velocities to forward direction only (no sideways sliding)
        leftWheelVelocity = forward * Vector3.Dot(leftWheelVelocity, forward);
        rightWheelVelocity = forward * Vector3.Dot(rightWheelVelocity, forward);

        // Apply drag
        leftWheelVelocity *= (1f - drag * Time.fixedDeltaTime);
        rightWheelVelocity *= (1f - drag * Time.fixedDeltaTime);

        // Clamp to max speed
        leftWheelVelocity = Vector3.ClampMagnitude(leftWheelVelocity, maxSpeed);
        rightWheelVelocity = Vector3.ClampMagnitude(rightWheelVelocity, maxSpeed);

        if(stopDirection != 0)
        {
            if(Mathf.Sign(Vector3.Dot(leftWheelVelocity, transform.forward)) == Mathf.Sign(stopDirection))
            {
                leftWheelVelocity = Vector3.zero;
            }
            if (Mathf.Sign(Vector3.Dot(rightWheelVelocity, transform.forward)) == Mathf.Sign(stopDirection))
            {
                rightWheelVelocity = Vector3.zero;
            }
        }

        Energy.Instance.EffectBatteryCharge((leftWheelVelocity.magnitude + rightWheelVelocity.magnitude) * energyCostScaler);
    }

    private void UpdateRigidbody()
    {
        // Calculate axle center velocity (average of both wheels)
        Vector3 centerVelocity = (leftWheelVelocity + rightWheelVelocity) * 0.5f;

        // Calculate rotation based on wheel velocity difference
        Vector3 velocityDiff = leftWheelVelocity - rightWheelVelocity;
        Vector3 forward = transform.forward;
        float forwardDiff = Vector3.Dot(velocityDiff, forward);
        float rotationAmount = (forwardDiff / axleWidth) * Time.fixedDeltaTime * Mathf.Rad2Deg;

        // Apply Y-axis rotation (turning)
        Quaternion yawRotation = Quaternion.Euler(0f, rotationAmount, 0f);
        Quaternion baseRotation = rb.rotation * yawRotation;
        float yRotation = baseRotation.eulerAngles.y;

        // Calculate horizontal movement only (ignore Y)
        Vector3 horizontalVelocity = new Vector3(centerVelocity.x, 0f, centerVelocity.z);
        Vector3 newPosition = rb.position + horizontalVelocity * Time.fixedDeltaTime;

        // Calculate wheel positions based on current rotation
        Vector3 right = Quaternion.Euler(0f, yRotation, 0f) * Vector3.right;
        Vector3 leftWheelPos = newPosition - right * (axleWidth * 0.5f);
        Vector3 rightWheelPos = newPosition + right * (axleWidth * 0.5f);

        // Raycast from each wheel to find ground contact points
        RaycastHit leftHit, rightHit;
        bool leftGrounded = Physics.Raycast(leftWheelPos + Vector3.up * 1000f, Vector3.down, out leftHit, Mathf.Infinity, groundLayer);
        bool rightGrounded = Physics.Raycast(rightWheelPos + Vector3.up * 1000f, Vector3.down, out rightHit, Mathf.Infinity, groundLayer);

        if (leftGrounded && rightGrounded)
        {
            // Snap wheel positions to ground
            Vector3 leftGroundPoint = leftHit.point + Vector3.up * hoverHeight;
            Vector3 rightGroundPoint = rightHit.point + Vector3.up * hoverHeight;

            // Calculate the vector between the two wheel contact points
            Vector3 axleVector = rightGroundPoint - leftGroundPoint;

            // Normalize and ensure correct axle length
            Vector3 axleDirection = axleVector.normalized;
            rightGroundPoint = leftGroundPoint + axleDirection * axleWidth;

            // Calculate new center position (middle of axle)
            Vector3 targetPosition = (leftGroundPoint + rightGroundPoint) * 0.5f;

            // Calculate target rotation based on axle orientation
            Vector3 axleForward = Vector3.Cross(axleDirection, Vector3.up).normalized;
            Vector3 axleUp = Vector3.Cross(axleForward, axleDirection).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(axleForward, axleUp);

            // Smoothly interpolate rotation toward target using terrain alignment speed
            currentSurfaceRotation = Quaternion.Slerp(
                currentSurfaceRotation,
                targetRotation,
                terrainAlignmentSpeed * Time.fixedDeltaTime
            );

            newPosition = targetPosition;
        }
        else if (leftGrounded || rightGrounded)
        {
            // If only one wheel is grounded, use that as reference
            RaycastHit hit = leftGrounded ? leftHit : rightHit;
            newPosition.y = hit.point.y + hoverHeight;
        }

        // Combine smooth surface alignment with Y rotation adjustments
        Quaternion finalRotation = currentSurfaceRotation;

        // Calculate forward lean based on velocity
        float forwardSpeed = Vector3.Dot(centerVelocity, transform.forward);
        float targetLean = Mathf.Clamp((forwardSpeed / maxSpeed) * maxLeanAngle, -maxLeanAngle, maxLeanAngle);
        currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLean, leanSpeed * Time.fixedDeltaTime);

        // Apply lean rotation in local space (pitch forward/backward)
        Quaternion leanRotation = Quaternion.Euler(currentLeanAngle, 0f, 0f);
        finalRotation = finalRotation * leanRotation;

        // Apply to rigidbody
        rb.MovePosition(newPosition);
        rb.MoveRotation(finalRotation);
    }

    private void UpdateVisualWheels()
    {
        if (leftWheelSpinner == null || rightWheelSpinner == null) return;

        // Calculate wheel spin based on velocity
        // Distance traveled = velocity * time, rotation = distance / circumference * 360
        float circumference = 2f * Mathf.PI * wheelRadius;

        // Get the signed speed (positive forward, negative backward)
        float leftSpeed = Vector3.Dot(leftWheelVelocity, transform.forward);
        float rightSpeed = Vector3.Dot(rightWheelVelocity, transform.forward);

        float leftDistance = leftSpeed * Time.fixedDeltaTime;
        float rightDistance = rightSpeed * Time.fixedDeltaTime;

        float leftRotationDelta = (leftDistance / circumference) * 360f;
        float rightRotationDelta = (rightDistance / circumference) * 360f;

        leftWheelRotation += leftRotationDelta;
        rightWheelRotation += rightRotationDelta;

        // Apply wheel spin rotation
        leftWheelSpinner.localRotation = Quaternion.Euler(leftWheelRotation, 0f, 0f);
        rightWheelSpinner.localRotation = Quaternion.Euler(rightWheelRotation, 0f, 0f);

        // Calculate steering deflection based on differential velocity
        if (leftSuspension != null && rightSuspension != null)
        {
            float speedDiff = leftSpeed - rightSpeed;

            // Normalize by max speed and apply steering angle
            float steerAmount = Mathf.Abs(Mathf.Clamp(speedDiff / maxSpeed, -0.5f, 1f) * maxSteerAngle);

            // Apply only Y rotation, preserving base X and Z rotations
            Vector3 leftEuler = leftSuspensionBaseRotation.eulerAngles;
            leftSuspension.localRotation = Quaternion.Euler(leftEuler.x, leftEuler.y + steerAmount, leftEuler.z);

            Vector3 rightEuler = rightSuspensionBaseRotation.eulerAngles;
            rightSuspension.localRotation = Quaternion.Euler(rightEuler.x, rightEuler.y - steerAmount, rightEuler.z);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((wallLayers.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            Vector3 toTarget = other.transform.position - transform.position;
            stopDirection = Vector3.Dot(toTarget, transform.forward);
        }
    }

    // Visualize the axle and ground detection in the editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector3 right = transform.right;
        Vector3 center = transform.position;

        Vector3 leftWheel = center - right * (axleWidth * 0.5f);
        Vector3 rightWheel = center + right * (axleWidth * 0.5f);

        // Draw axle
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftWheel, rightWheel);

        // Draw velocity vectors
        Gizmos.color = Color.green;
        Gizmos.DrawRay(leftWheel, leftWheelVelocity * 0.5f);
        Gizmos.DrawRay(rightWheel, rightWheelVelocity * 0.5f);

        // Draw wheels
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftWheel, 0.2f);
        Gizmos.DrawWireSphere(rightWheel, 0.2f);

        // Draw ground raycast
        Gizmos.color = Color.red;
        Vector3 rayOrigin = center + Vector3.up * groundCheckDistance;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance * 2f);
    }
}
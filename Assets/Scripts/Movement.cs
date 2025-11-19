using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

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
    //[SerializeField] private VisualEffect leftWheelVFX;
    //[SerializeField] private VisualEffect rightWheelVFX;


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

    Vector3 rotatedForward;
    float smoothedMovementX = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Movement script requires a Rigidbody component!");
            return;
        }

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        currentSurfaceRotation = Quaternion.identity;

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
        Vector3 forward = transform.forward;

        float forwardInput = moveInput.y;
        float turnInput = moveInput.x;

        Vector3 forwardAccel = forward * forwardInput * acceleration * Time.fixedDeltaTime;
        leftWheelVelocity += forwardAccel;
        rightWheelVelocity += forwardAccel;

        Vector3 turnAccel = forward * turnInput * turnAcceleration * Time.fixedDeltaTime;
        leftWheelVelocity += turnAccel;
        rightWheelVelocity -= turnAccel;

        leftWheelVelocity = forward * Vector3.Dot(leftWheelVelocity, forward);
        rightWheelVelocity = forward * Vector3.Dot(rightWheelVelocity, forward);

        leftWheelVelocity *= (1f - drag * Time.fixedDeltaTime);
        rightWheelVelocity *= (1f - drag * Time.fixedDeltaTime);

        leftWheelVelocity = Vector3.ClampMagnitude(leftWheelVelocity, maxSpeed);
        rightWheelVelocity = Vector3.ClampMagnitude(rightWheelVelocity, maxSpeed);

        //leftWheelVFX.enabled = leftWheelVelocity.magnitude > 0.5f;
        //rightWheelVFX.enabled = rightWheelVelocity.magnitude > 0.5f;

        if (stopDirection != 0)
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
        Vector3 centerVelocity = (leftWheelVelocity + rightWheelVelocity) * 0.5f;

        Vector3 velocityDiff = leftWheelVelocity - rightWheelVelocity;
        Vector3 forward = transform.forward;
        float forwardDiff = Vector3.Dot(velocityDiff, forward);
        float rotationAmount = (forwardDiff / axleWidth) * Time.fixedDeltaTime * Mathf.Rad2Deg;

        Quaternion yawRotation = Quaternion.Euler(0f, rotationAmount, 0f);
        Quaternion baseRotation = rb.rotation * yawRotation;
        float yRotation = baseRotation.eulerAngles.y;

        Vector3 horizontalVelocity = new Vector3(centerVelocity.x, 0f, centerVelocity.z);
        Vector3 newPosition = rb.position + horizontalVelocity * Time.fixedDeltaTime;

        Vector3 right = Quaternion.Euler(0f, yRotation, 0f) * Vector3.right;
        Vector3 leftWheelPos = newPosition - right * (axleWidth * 0.5f);
        Vector3 rightWheelPos = newPosition + right * (axleWidth * 0.5f);

        RaycastHit leftHit, rightHit;
        bool leftGrounded = Physics.Raycast(leftWheelPos + Vector3.up * 1000f, Vector3.down, out leftHit, Mathf.Infinity, groundLayer);
        bool rightGrounded = Physics.Raycast(rightWheelPos + Vector3.up * 1000f, Vector3.down, out rightHit, Mathf.Infinity, groundLayer);

        if (leftGrounded && rightGrounded)
        {
            Vector3 leftGroundPoint = leftHit.point + Vector3.up * hoverHeight;
            Vector3 rightGroundPoint = rightHit.point + Vector3.up * hoverHeight;

            Vector3 axleVector = rightGroundPoint - leftGroundPoint;

            Vector3 axleDirection = axleVector.normalized;
            rightGroundPoint = leftGroundPoint + axleDirection * axleWidth;

            Vector3 targetPosition = (leftGroundPoint + rightGroundPoint) * 0.5f;

            Vector3 axleForward = Vector3.Cross(axleDirection, Vector3.up).normalized;
            Vector3 axleUp = Vector3.Cross(axleForward, axleDirection).normalized;

            Quaternion targetRotation = Quaternion.identity;
            smoothedMovementX = Mathf.Lerp(smoothedMovementX, moveInput.x, 0.9f * Time.fixedDeltaTime);
            Debug.Log(Mathf.Abs(smoothedMovementX) < 0.1f && moveInput.x == 0);
            if (Mathf.Abs(smoothedMovementX) < 0.1f && moveInput.x == 0)
            {
                targetRotation = Quaternion.LookRotation(rotatedForward, axleUp);
            }
            else
            { 
                targetRotation = Quaternion.LookRotation(axleForward, axleUp);
            }

            currentSurfaceRotation = Quaternion.Slerp(
                currentSurfaceRotation,
                targetRotation,
                terrainAlignmentSpeed * Time.fixedDeltaTime
            );

            newPosition = targetPosition;
        }
        else if (leftGrounded || rightGrounded)
        {
            RaycastHit hit = leftGrounded ? leftHit : rightHit;
            newPosition.y = hit.point.y + hoverHeight;
        }

        Quaternion finalRotation = currentSurfaceRotation;

        float forwardSpeed = Vector3.Dot(centerVelocity, transform.forward);
        float targetLean = Mathf.Clamp((forwardSpeed / maxSpeed) * maxLeanAngle, -maxLeanAngle, maxLeanAngle);
        currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLean, leanSpeed * Time.fixedDeltaTime);

        Vector3 difference = newPosition - transform.position;
        Debug.DrawRay(transform.position, difference * 10);

        rotatedForward = finalRotation * Vector3.forward;
        newPosition = transform.position 
            + Vector3.ProjectOnPlane(difference, Vector3.Cross(Vector3.up, rotatedForward).normalized);

        Quaternion leanRotation = Quaternion.Euler(currentLeanAngle, 0f, 0f);
        finalRotation = finalRotation * leanRotation;


        rb.MovePosition(newPosition);
        rb.MoveRotation(finalRotation);
    }

    private void UpdateVisualWheels()
    {
        if (leftWheelSpinner == null || rightWheelSpinner == null) return;

        float circumference = 2f * Mathf.PI * wheelRadius;

        float leftSpeed = Vector3.Dot(leftWheelVelocity, transform.forward);
        float rightSpeed = Vector3.Dot(rightWheelVelocity, transform.forward);

        float leftDistance = leftSpeed * Time.fixedDeltaTime;
        float rightDistance = rightSpeed * Time.fixedDeltaTime;

        float leftRotationDelta = (leftDistance / circumference) * 360f;
        float rightRotationDelta = (rightDistance / circumference) * 360f;

        leftWheelRotation += leftRotationDelta;
        rightWheelRotation += rightRotationDelta;

        leftWheelSpinner.localRotation = Quaternion.Euler(leftWheelRotation, 0f, 0f);
        rightWheelSpinner.localRotation = Quaternion.Euler(rightWheelRotation, 0f, 0f);

        if (leftSuspension != null && rightSuspension != null)
        {
            float speedDiff = leftSpeed - rightSpeed;

            float steerAmount = Mathf.Abs(Mathf.Clamp(speedDiff / maxSpeed, -0.5f, 1f) * maxSteerAngle);

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

    private void OnDrawGizmos()
    {
        //if (!Application.isPlaying) return;
        //
        //Vector3 right = transform.right;
        //Vector3 center = transform.position;
        //
        //Vector3 leftWheel = center - right * (axleWidth * 0.5f);
        //Vector3 rightWheel = center + right * (axleWidth * 0.5f);
        //
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(leftWheel, rightWheel);
        //
        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(leftWheel, leftWheelVelocity * 0.5f);
        //Gizmos.DrawRay(rightWheel, rightWheelVelocity * 0.5f);
        //
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(leftWheel, 0.2f);
        //Gizmos.DrawWireSphere(rightWheel, 0.2f);
        //
        //Gizmos.color = Color.red;
        //Vector3 rayOrigin = center + Vector3.up * groundCheckDistance;
        //Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance * 2f);
    }
}
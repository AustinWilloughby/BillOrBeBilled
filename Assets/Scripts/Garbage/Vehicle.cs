using UnityEngine;
using UnityEngine.InputSystem;

//Source inspo: https://www.youtube.com/watch?v=IlqcaNkjMRY

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    [SerializeField] float power = 15000f;
    [SerializeField] float torque = 500f;
    [SerializeField] float downforceCoef = 0.05f;
    [SerializeField] float maxSpeed = 10.0f;
    [SerializeField] float stationaryTurnMultiplier = 20.0f;

    [SerializeField] Wheel[] wheels;

    [SerializeField] Wheel supportWheel;

    Vector2 movementInput;
    Rigidbody rb;

    void Awake()
    {
        movementInput = Vector2.zero;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (Wheel wheel in wheels)
        {
            //wheel.Steer(movementInput.x, rb.linearVelocity.magnitude, maxSpeed);
            wheel.Accelerate(movementInput.y * power);
            wheel.UpdatePosition();
        }

        //Make it so that the else statement only fires IF they aren't moving forward at all
        supportWheel.Steer(movementInput.x, rb.linearVelocity.magnitude, maxSpeed);
        if (Mathf.Abs(movementInput.y) >= Mathf.Abs(movementInput.x) * 0.8f || rb.linearVelocity.magnitude > 0.5f)
        {
            supportWheel.Accelerate(movementInput.y * power * (1.0f - Mathf.Abs(movementInput.x)));
        }
        else
        {
            supportWheel.Accelerate(Mathf.Abs(movementInput.x) * power * stationaryTurnMultiplier);
        }
        supportWheel.UpdatePosition();

        rb.AddForce(-transform.up * (downforceCoef * rb.linearVelocity.sqrMagnitude));

        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    float maxSpeed = 1;

    [SerializeField]
    float maxRotate = 1;

    Rigidbody rBody;
    Vector2 currentMovement;

    void Awake()
    {
        //SnapDownward();
        rBody = GetComponent<Rigidbody>();
        if(!rBody)
        {
            Debug.LogError("Movement Controller missing Rigidbody");
            Destroy(this);
        }
        rBody.maxLinearVelocity = maxSpeed;
       
    }

    void Update()
    {
        rBody.AddTorque(new Vector3(0, currentMovement.x * Time.deltaTime * 200, 0));
        Vector3 directonalMovement = transform.forward * currentMovement.y * Time.deltaTime * 1000;
        rBody.AddForceAtPosition(directonalMovement, transform.position);

        rBody.linearVelocity = transform.forward * rBody.linearVelocity.magnitude;
        if (rBody.angularVelocity.magnitude > maxRotate)
        {
            rBody.angularVelocity = rBody.angularVelocity.normalized * maxRotate;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        currentMovement = ctx.ReadValue<Vector2>();
    }
}

using UnityEngine;

//Source inspo: https://www.youtube.com/watch?v=IlqcaNkjMRY

public class Wheel : MonoBehaviour
{
    [SerializeField] bool powered = false;
    [SerializeField] float maxAngle = 90f;
    [SerializeField] float offset = 0f;
    [SerializeField] Transform wheelMesh;

    WheelCollider wheelCol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        wheelCol = GetComponentInChildren<WheelCollider>();
    }

    public void Steer(float steerInput, float currentSpeed, float maxSpeed)
    {
        float speedScaler = 1.0f - ((currentSpeed / maxSpeed) * 0.8f);
        wheelCol.steerAngle = (steerInput * maxAngle + offset) * speedScaler;
    }

    public void Accelerate(float powerInput)
    {
        if(powered)
        {
            wheelCol.motorTorque = powerInput;
        }
        else
        {
            wheelCol.brakeTorque = 0;
        }
    }

    public void UpdatePosition()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        wheelCol.GetWorldPose(out pos, out rot);
        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
    }
}

using UnityEngine;


public class ChargingPlatform : MonoBehaviour
{
    [SerializeField] float chargePerSecond = 1000;
    private void OnTriggerStay(Collider other)
    {
        Movement vehicle = other.transform.root.gameObject.GetComponent<Movement>();
        if(vehicle)
        {
            Energy.Instance.EffectBatteryCharge(-chargePerSecond);
        }
    }
}

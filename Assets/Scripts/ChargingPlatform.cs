using UnityEngine;


public class ChargingPlatform : MonoBehaviour
{
    [SerializeField] float chargePerSecond = 1000;
    [SerializeField] float costPerSecond = 10;

    private void OnTriggerStay(Collider other)
    {
        Movement vehicle = other.transform.root.gameObject.GetComponent<Movement>();
        if(vehicle)
        {
            if (Mathf.RoundToInt(Energy.Instance.CurrentBattery) < Energy.Instance.MaxBattery && Energy.Instance.Money > 0)
            {
                Energy.Instance.EffectBatteryCharge(-chargePerSecond);
                Energy.Instance.Money -= costPerSecond * Time.fixedDeltaTime;
                Energy.Instance.Money = Mathf.Max(0, Energy.Instance.Money);
            }
        }
    }


}

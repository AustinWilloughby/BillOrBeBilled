using UnityEngine;

public class Energy : MonoBehaviour
{
    public static Energy Instance { get; private set; }

    [SerializeField] float maxBattery = 100;
    [SerializeField] float currentBattery = 100;
    [SerializeField] float baseLevelDrain = 0.1f;
    [SerializeField] float drainPerSecond = 0.1f;

    public float BatteryPercent { get { return currentBattery / maxBattery; } }
    public float MaxBattery { get { return maxBattery; } set { maxBattery = value; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void LateUpdate()
    {
        currentBattery -= drainPerSecond * Time.deltaTime;
        drainPerSecond = baseLevelDrain;
    }
    public void EffectBatteryCharge(float changePerSecond)
    {
        drainPerSecond += changePerSecond;
    }

}

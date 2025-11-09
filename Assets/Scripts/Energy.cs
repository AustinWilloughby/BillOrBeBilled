using UnityEngine;

public class Energy : MonoBehaviour
{
    public static Energy Instance { get; private set; }

    [SerializeField] float currentMoney = 0;

    [SerializeField] float maxBattery = 100;
    [SerializeField] float currentBattery = 100;
    [SerializeField] float baseLevelDrain = 0.1f;
    [SerializeField] float drainPerSecond = 0.1f;

    public GameObject gameOverScreen;


    public float CurrentBattery { get { return currentBattery; } }
    public float MaxBattery { get { return maxBattery; } set { maxBattery = value; } }
    public float Money { get { return currentMoney; } set { currentMoney = value; } }

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
        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);
        drainPerSecond = baseLevelDrain;
    }

    public void EffectBatteryCharge(float deductPerSecond)
    {
        drainPerSecond += deductPerSecond;
    }
}

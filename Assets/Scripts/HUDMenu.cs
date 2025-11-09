using UnityEngine;
using UnityEngine.UI;

public class HUDMenu : MonoBehaviour
{
    [SerializeField]
    Slider EnergySlider;

    [SerializeField]
    Text moneyLabel;

    const string k_MONEY_STR = "Money: ${0}";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EnergySlider.maxValue = Energy.Instance.MaxBattery;

        EnergySlider.value = Energy.Instance.CurrentBattery;

        moneyLabel.text = string.Format(k_MONEY_STR, Energy.Instance.Money);
    }
}

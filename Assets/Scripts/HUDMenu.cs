using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDMenu : MonoBehaviour
{
    [SerializeField]
    Slider EnergySlider;

    [SerializeField]
    Text moneyLabel;

    [SerializeField]
    TMP_Text moneyText;

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

        if (moneyLabel != null)
        {
            moneyLabel.text = string.Format(k_MONEY_STR, Energy.Instance.Money);
        }

        if (moneyText != null)
        {
            moneyText.text = string.Format(k_MONEY_STR, Energy.Instance.Money);
        }
    }
}

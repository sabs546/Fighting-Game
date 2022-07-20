using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public enum Setting { Volume, Health, Rounds };
    public Setting setting;
    public TMPro.TextMeshProUGUI valueBox;

    // Start is called before the first frame update
    void Start()
    {
        if (setting == Setting.Volume) GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");
        valueBox.text = GetComponent<Slider>().value.ToString();
    }

    public void SetVolume()
    {
        WorldRules.volume = GetComponent<Slider>().value;
    }

    public void SetRounds()
    {
        WorldRules.roundLimit = (int)GetComponent<Slider>().value;
    }

    public void SetRoundTimer()
    {
        WorldRules.roundTimer = GetComponent<Slider>().value;
    }

    public void SetValueText()
    {
        valueBox.text = GetComponent<Slider>().value.ToString();
    }
}

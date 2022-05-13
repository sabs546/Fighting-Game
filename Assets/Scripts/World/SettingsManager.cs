using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");
    }

    public void SetVolume()
    {
        WorldRules.volume = GetComponent<Slider>().value;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsInputComponent : MonoBehaviour
{
    [SerializeField]
    private string _settingsName;
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private TextMeshProUGUI _perCentLabel;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(_settingsName))
        {
            PlayerPrefs.SetFloat(_settingsName, 1f);
        }
        else
        {
            float value = PlayerPrefs.GetFloat(_settingsName);
            _slider.value = value;
            _perCentLabel.text = string.Format("{0:0}", (value * 100)) + "%";
        }
    }

    public void SliderValueChanged()
    {
        PlayerPrefs.SetFloat(_settingsName, _slider.value);
        _perCentLabel.text = string.Format("{0:0}", (_slider.value * 100)) + "%";
    }
}

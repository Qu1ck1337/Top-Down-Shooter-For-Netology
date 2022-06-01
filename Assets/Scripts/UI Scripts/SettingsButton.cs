using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField]
    private GameObject _panel;

    public void ToggleSettingsPanel()
    {
        _panel.SetActive(!_panel.activeSelf);
    }
}

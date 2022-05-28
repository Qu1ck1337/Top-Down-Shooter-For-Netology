using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAssistant : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _ammoBar;
    [SerializeField]
    private TextMeshProUGUI _scoreBar;

    public void SetAmmoBar(int currentAmmoInStore, int currentAllAmmo)
    {
        if (currentAmmoInStore > 0 || currentAllAmmo > 0)
            _ammoBar.text = currentAmmoInStore.ToString() + "/" + currentAllAmmo.ToString();
        else
            ResetAmmoBar();
    }

    public void ResetAmmoBar()
    {
        _ammoBar.text = "NO WEAPON";
    }

    public void ShowPlayerScore(int score)
    {
        _scoreBar.text = score.ToString() + " pts";
    }
}

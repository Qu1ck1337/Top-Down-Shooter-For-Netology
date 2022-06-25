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
    [SerializeField]
    private TextMeshProUGUI _bonusBar;
    [SerializeField]
    private GameObject _rampageLabelStart;
    [SerializeField]
    private GameObject _rampageLabelEnd;
    [SerializeField]
    private GameObject _comeBackLabel;
    [SerializeField]
    private float _rampageLabelStayTime = 1f;

    public void SetAmmoBar(int currentAmmoInStore, int currentAllAmmo)
    {
        if (currentAmmoInStore > 0 || currentAllAmmo > 0)
            _ammoBar.text = currentAmmoInStore.ToString() + "/" + currentAllAmmo.ToString();
        else
            _ammoBar.text = "Õ≈“ œ¿“–ŒÕŒ¬";
    }

    public void ResetAmmoBar()
    {
        _ammoBar.text = "Õ≈“ Œ–”∆»ﬂ";
    }

    public void ShowPlayerScore(int score)
    {
        _scoreBar.text = score.ToString() + " pts";
    }

    public void ShowPlayerBonusScore(float score)
    {
        _bonusBar.text = string.Format("{0:0.0}", score) + "x";
    }

    public void ResetPlayerBonusScore()
    {
        _bonusBar.text = "";
    }

    public void StartRampageStartLabel()
    {
        StartCoroutine(RampageLabelStartTimer());
    }

    private IEnumerator RampageLabelStartTimer()
    {
        _rampageLabelStart.SetActive(true);
        yield return new WaitForSeconds(_rampageLabelStayTime);
        _rampageLabelStart.SetActive(false);
    }

    public void StartRampageEndLabel()
    {
        StartCoroutine(RampageLabelEndTimer());
    }

    private IEnumerator RampageLabelEndTimer()
    {
        _rampageLabelEnd.SetActive(true);
        yield return new WaitForSeconds(_rampageLabelStayTime);
        _rampageLabelEnd.SetActive(false);
    }

    public void EndLevelLabel()
    {
        _comeBackLabel.SetActive(true);
    }
}

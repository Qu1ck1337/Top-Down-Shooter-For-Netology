using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalLevelAssistant : MonoBehaviour
{
    [SerializeField]
    private GameObject _finalLevelDoor;
    [SerializeField]
    private GameObject _doNotGoLabel;
    [SerializeField]
    private string _textAfterTrigger;
    [SerializeField]
    private GameObject _blood;
    [SerializeField]
    private AudioClip _bossSoundtrack;

    [SerializeField]
    private EnemyComponent _boss;
    public EnemyComponent getBoss => _boss;

    private bool _bossActionStarted;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _gameManager.OnAllEnemiesDeadEvent += StartBossAction;
        _boss.OnUnitDeadEvent += NextLevel;
        _finalLevelDoor.GetComponent<DoNotGoLabelComponent>().OnTriggerEntered += ChangeStyle;
    }

    private void StartBossAction()
    {
        if (!_bossActionStarted)
        {
            Destroy(_finalLevelDoor.GetComponent<MeshRenderer>());
            _finalLevelDoor.GetComponent<Collider>().isTrigger = true;
            _doNotGoLabel.SetActive(true);
            _boss.gameObject.SetActive(true);
            _bossActionStarted = true;
        }
    }

    private void ChangeStyle()
    {
        _doNotGoLabel.SetActive(true);
        _doNotGoLabel.GetComponent<TextMeshPro>().text = _textAfterTrigger;
        _blood.SetActive(true);
        GetComponent<SoundAssistant>()?.SetSoundtrack(_bossSoundtrack);
    }

    private void NextLevel(EnemyComponent enemy)
    {
        _gameManager.SpawnEndLevelTrigger();
    }
}

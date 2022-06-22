using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalLevelAssistant : MonoBehaviour
{
    [SerializeField]
    private GameObject _finalLevelDoor;

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
    }

    private void StartBossAction()
    {
        if (!_bossActionStarted)
        {
            _finalLevelDoor.SetActive(false);
            _boss.gameObject.SetActive(true);
            _bossActionStarted = true;
        }
    }

    private void NextLevel(EnemyComponent enemy)
    {
        _gameManager.SpawnEndLevelTrigger();
    }
}

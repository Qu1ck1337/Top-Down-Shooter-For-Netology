using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private UIAssistant _uiAssistant;

    [Space, SerializeField]
    private string _sceneName;
    [SerializeField]
    private string _nextSceneName;

    [Space, SerializeField]
    private int _scoreAfterKillEnemy;

    [Space, SerializeField]
    private float _timeToStartRampage = 10f;
    [SerializeField]
    private float _secondsToDecreaseBonus = 1f;
    [SerializeField]
    private float _startBonusOnRampage = 1.6f;
    [SerializeField]
    private float _addBonusAfterKill = 0.5f;
    [SerializeField]
    private float _reduceBonusAfterDecreaseTime = 0.1f;
    [SerializeField]
    private int _decreasesToResetBonus = 5;

    [Space, SerializeField]
    private GameObject _endLevelTrigger;

    private FinalLevelAssistant _finalLevelAssistant;
    private int _totalyPlayerScore;
    private PlayerComponent _player;
    private List<EnemyComponent> _enemies = new List<EnemyComponent>();
    private float _bonus;
    private int _enemiesKilledForTime;
    private float _timer;
    private int _decreases;
    private bool _rampage;

    private void Start()
    {
        _player = FindObjectOfType<PlayerComponent>();
        _player.OnPlayerActionEvent += PlayerAction;
        _uiAssistant.SetAmmoBar(_player.PlayerWeapon.CurrentAmmoInStore, _player.PlayerWeapon.CurrentAllAmmo);

        _finalLevelAssistant = GetComponent<FinalLevelAssistant>();

        _enemies = FindObjectsOfType<EnemyComponent>().ToList();
        foreach(EnemyComponent enemy in _enemies)
        {
            enemy.OnUnitDeadEvent += PointsChecker;
        }
    }

    public delegate void AllEnemiesDeadEventHandler();
    public event AllEnemiesDeadEventHandler OnAllEnemiesDeadEvent;

    void Update()
    {
        NearestEnemies();
        EndGameLogic();
        RampageChecker();
    }

    private void EndGameLogic()
    {
        if (_player == null)
        {
            SceneManager.LoadScene(_sceneName);
        }

        if (_enemies.Count == 0)
        {
            CheckBestScores();

            if (_finalLevelAssistant != null)
                OnAllEnemiesDeadEvent?.Invoke();
            else
                SpawnEndLevelTrigger();
        }
    }

    private void RampageChecker()
    {
        if (!_rampage)
        {
            if (_enemiesKilledForTime >= 3 && _timer <= _timeToStartRampage)
            {
                StartCoroutine(StartRampage());
            }
            else if (_timer > _timeToStartRampage)
            {
                _timer = 0f;
                _enemiesKilledForTime = 0;
            }
            else if (_enemiesKilledForTime >= 1)
            {
                _timer += Time.deltaTime;
            }
        }
    }

    private void NearestEnemies()
    {
        foreach (EnemyComponent enemy in _enemies)
        {
            List<EnemyComponent> nearestEnemies = new List<EnemyComponent>();
            foreach (EnemyComponent checkingEnemy in _enemies)
            {
                if (checkingEnemy != enemy && Vector3.Distance(checkingEnemy.transform.position, enemy.transform.position) <= enemy.PlayerIdentificationRadius)
                {
                    nearestEnemies.Add(checkingEnemy);
                }
            }
            enemy.NearestEnemies = nearestEnemies;
        }
    }

    public void NextLevel()
    {
        if (_rampage)
        {
            _totalyPlayerScore += (int)(_scoreAfterKillEnemy * _enemiesKilledForTime * _bonus);
        }
        SceneManager.LoadScene(_nextSceneName);
    }

    private IEnumerator StartRampage()
    {
        _rampage = true;
        _bonus = _startBonusOnRampage;
        _enemiesKilledForTime = 0;
        _timer = 0f;
        _uiAssistant.ShowPlayerBonusScore(_bonus);
        while (_rampage)
        {
            yield return new WaitForSeconds(_secondsToDecreaseBonus);
            _decreases += 1;
            _bonus -= _reduceBonusAfterDecreaseTime;
            
            if (_decreases >= _decreasesToResetBonus)
            {
                _rampage = false;
                _totalyPlayerScore += (int)(_scoreAfterKillEnemy * _enemiesKilledForTime * _bonus);
                _uiAssistant.ShowPlayerScore(_totalyPlayerScore);
                _bonus = 1;
                _enemiesKilledForTime = 0;
                _decreases = 0;
            }

            if (_rampage)
                _uiAssistant.ShowPlayerBonusScore(_bonus);
            else
                _uiAssistant.ResetPlayerBonusScore();
        }
    }

    public void SpawnEndLevelTrigger()
    {
        _endLevelTrigger.SetActive(true);
    }

    private void PlayerAction(Enums.PlayerActionType actionType)
    {
        if (actionType == Enums.PlayerActionType.PickUpWeapon || actionType == Enums.PlayerActionType.Shoot || actionType == Enums.PlayerActionType.ReloadedWeapon)
        {
            var currentAmmoInStore = _player.PlayerWeapon.CurrentAmmoInStore;
            var currentAllAmmo = _player.PlayerWeapon.CurrentAllAmmo;
            _uiAssistant.SetAmmoBar(currentAmmoInStore, currentAllAmmo);
        }
        else if (actionType == Enums.PlayerActionType.DropWeapon)
        {
            _uiAssistant.ResetAmmoBar();
        }
    }

    private void PointsChecker(EnemyComponent enemy)
    {
        _enemiesKilledForTime += 1;
        if (_rampage)
        {
            _decreases = 0;
            _bonus += _addBonusAfterKill;
            _uiAssistant.ShowPlayerBonusScore(_bonus);
        }
        else
        {
            _totalyPlayerScore += _scoreAfterKillEnemy;
            _uiAssistant.ShowPlayerScore(_totalyPlayerScore);
        }
        ReduceEnemyList(enemy);
    }

    private void CheckBestScores()
    {
        if (PlayerPrefs.HasKey(_sceneName))
        {
            if (PlayerPrefs.GetInt(_sceneName) >= _totalyPlayerScore) return;
            PlayerPrefs.SetInt(_sceneName, _totalyPlayerScore);
        }
        else
        {
            PlayerPrefs.SetInt(_sceneName, _totalyPlayerScore);
        }
    }

    private void ReduceEnemyList(EnemyComponent enemy)
    {
        _enemies.Remove(enemy);
    }
}

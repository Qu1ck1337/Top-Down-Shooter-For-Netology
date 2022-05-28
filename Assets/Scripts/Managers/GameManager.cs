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

    private int _totalyPlayerScore;
    private PlayerComponent _player;
    private List<EnemyComponent> _enemies = new List<EnemyComponent>();

    private void Start()
    {
        _player = FindObjectOfType<PlayerComponent>();
        _player.OnPlayerActionEvent += PlayerAction;
        _uiAssistant.SetAmmoBar(_player.PlayerWeapon.CurrentAmmoInStore, _player.PlayerWeapon.CurrentAllAmmo);

        _enemies = FindObjectsOfType<EnemyComponent>().ToList();
        foreach(EnemyComponent enemy in _enemies)
        {
            enemy.OnUnitDeadEvent += PointsChecker;
        }
    }

    void Update()
    {
        if (_player == null)
        {
            SceneManager.LoadScene(_sceneName);
        }
        if (_enemies.Count == 0)
        {
            CheckBestScores();
            SceneManager.LoadScene(_nextSceneName);
        }
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
        _totalyPlayerScore += _scoreAfterKillEnemy;
        _uiAssistant.ShowPlayerScore(_totalyPlayerScore);
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

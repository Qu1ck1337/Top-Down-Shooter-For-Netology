using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string _sceneName;
    [SerializeField]
    private string _nextSceneName;

    private PlayerComponent _player;
    private List<EnemyComponent> _enemies = new List<EnemyComponent>();

    private void Start()
    {
        _player = FindObjectOfType<PlayerComponent>();
        _enemies = FindObjectsOfType<EnemyComponent>().ToList();
        foreach(EnemyComponent enemy in _enemies)
        {
            enemy.OnEnemyDeadEvent += ReduceEnemyList;
        }
    }

    void Update()
    {
        if (_player == null)
        {
            SceneManager.LoadScene(_sceneName);
        }
        Debug.Log(_enemies.Count);
        if (_enemies.Count == 0)
        {
            SceneManager.LoadScene(_nextSceneName);
        }
    }

    private void ReduceEnemyList(EnemyComponent enemy)
    {
        _enemies.Remove(enemy);
    }
}

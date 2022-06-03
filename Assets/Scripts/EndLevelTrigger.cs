using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelTrigger : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GetComponentInParent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _gameManager.NextLevel();
    }
}

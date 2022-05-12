using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerComponent _player;

    private void Start()
    {
        _player = FindObjectOfType<PlayerComponent>();
    }

    void Update()
    {
        if (_player == null)
        {
            SceneManager.LoadScene(0);
        }
    }
}

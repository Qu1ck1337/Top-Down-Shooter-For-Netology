using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoNotGoLabelComponent : MonoBehaviour
{
    private bool _isEntered;

    public event Action OnTriggerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerComponent>())
        {
            if (_isEntered) return;
            OnTriggerEntered?.Invoke();
            _isEntered = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private bool _isMoving = false;

    public void SetMoving(bool status) => _isMoving = status;

    void Update()
    {
        if (_isMoving)
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
}

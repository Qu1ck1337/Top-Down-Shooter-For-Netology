using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damage;

    public UnitComponent Owner;

    private bool _isMoving = false;

    public void SetMoving(bool status) => _isMoving = status;

    void Update()
    {
        if (_isMoving)
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        UnitComponent unit = other.GetComponent<UnitComponent>();
        if (unit != null)
        {
            unit.ReduceHealthAndKill(_damage);
        }
    }
}

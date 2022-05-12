using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitComponent : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    protected int _health;
    [SerializeField, Range(0f, 1000f)]
    protected float _movementSpeed;
    [SerializeField, Range(0f, 1000f)]
    protected float _maxVelocity;
    [SerializeField]
    protected WeaponComponent _weapon;

    public void ReduceHealthAndKill(int reduce)
    {
        _health -= reduce;
        if (_health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}

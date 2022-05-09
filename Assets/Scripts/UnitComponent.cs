using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitComponent : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    protected int _health;
    [SerializeField, Range(0f, 100f)]
    protected float _MovementSpeed;
    [SerializeField]
    protected WeaponComponent _weapon;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitComponent : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _weaponSpawn;
    [SerializeField, Range(0, 100)]
    protected int _health;
    [SerializeField, Range(0f, 1000f)]
    protected float _movementSpeed;
    [SerializeField, Range(0f, 1000f)]
    protected float _maxVelocity;
    [SerializeField]
    protected WeaponComponent _weapon;
    [SerializeField]
    private float _dropWeaponImpulse = 100f;

    public void ReduceHealthAndKill(int reduce)
    {
        _health -= reduce;
        if (_health <= 0)
        {
            DropWeapon();
            Destroy(this.gameObject);
        }
    }

    protected void DropWeapon()
    {
        if (_weapon != null)
        {
            var parent = _weapon.transform.parent;
            _weapon.WeaponRigidBody.isKinematic = false;
            _weapon.transform.parent = null;
            _weapon.transform.eulerAngles = new Vector3(0f, 0f, 30f);
            _weapon.WeaponRigidBody.AddForce(parent.forward * _dropWeaponImpulse);
            _weapon = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_weapon != null) return;
        var weaponComponent = collision.gameObject.GetComponent<WeaponComponent>();
        if (weaponComponent != null)
        {
            _weapon = weaponComponent;
            _weapon.WeaponRigidBody.isKinematic = true;
            _weapon.transform.parent = transform;
            _weapon.transform.localPosition = _weaponSpawn;
            _weapon.transform.rotation = transform.rotation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitComponent : MonoBehaviour
{
    [SerializeField]
    protected Animator _animator;

    protected bool _inAnimation;

    [Space, SerializeField]
    protected Vector3 _weaponSpawn;
    [SerializeField, Range(0, 100)]
    protected int _health;
    [SerializeField, Range(0f, 1000f)]
    protected float _movementSpeed;
    [SerializeField]
    protected WeaponComponent _weapon;
    [SerializeField]
    private float _dropWeaponImpulse = 100f;
    [SerializeField]
    protected int _damageOfHandAttack = 1;

    protected Rigidbody _rigidBody;

    protected SphereCollider _handTrigger;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

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
            _weapon.Owner = null;
            _weapon.SetFlyingTrue();
            _weapon = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_weapon != null) return;
        var weaponComponent = collision.gameObject.GetComponent<WeaponComponent>();
        if (weaponComponent != null && weaponComponent.Owner == null && weaponComponent.CanBePickedUp)
        {
            _weapon = weaponComponent;
            _weapon.Owner = this;
            _weapon.WeaponRigidBody.isKinematic = true;
            _weapon.transform.parent = transform;
            _weapon.transform.localPosition = _weaponSpawn;
            _weapon.transform.rotation = transform.rotation;
        }
    }

    protected void OnHandCollisionToggle_UnityEvent(AnimationEvent data)
    {
        _handTrigger.enabled = !_handTrigger.enabled;
    }

    protected void OnAnimationEnd_UnityEvent(AnimationEvent data)
    {
        _inAnimation = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var unit = other.GetComponent<UnitComponent>();
        if (unit != null)
        {
            unit.ReduceHealthAndKill(_damageOfHandAttack);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class UnitComponent : MonoBehaviour
{
    [SerializeField]
    protected Animator _animator;

    protected bool _inAnimation;

    [Space, SerializeField]
    protected Vector3 _weaponSpawn;
    [SerializeField]
    protected Vector3 _rifleSpawnPoint;
    [SerializeField, Range(0, 100)]
    protected int _health;
    [SerializeField, Range(0f, 1000f)]
    protected float _movementSpeed;
    [SerializeField]
    protected SimpleWeapon _weapon;
    [SerializeField]
    private float _dropWeaponImpulse = 100f;
    [SerializeField]
    protected int _damageOfHandAttack = 1;

    [Space, SerializeField]
    protected AudioSource _punchSound;
    [SerializeField]
    private AudioSource _dyingSound;

    protected Rigidbody _rigidBody;

    protected SphereCollider _handTrigger;

    protected bool _isDead;

    protected virtual void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public delegate void UnitDeadEventHandler(EnemyComponent enemy);
    public event UnitDeadEventHandler OnUnitDeadEvent;

    public void ReduceHealthAndKill(int reduce)
    {
        _health -= reduce;
        if (!_isDead && _health <= 0)
        {
            _isDead = true;
            if (_weapon != null)
                DropWeapon();
            OnUnitDeadEvent?.Invoke((EnemyComponent)this);
            _animator.SetTrigger("IsDying");
            Destroy(GetComponent<Collider>());
            _rigidBody.isKinematic = true;
            _dyingSound.Play();
            if (GetComponent<NavMeshAgent>() != null)
                Destroy(GetComponent<NavMeshAgent>());
        }
    }

    protected virtual void DropWeapon()
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
            _weapon.ToggleColliders(true);
            _weapon = null;
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

    protected void OnDeadAnimationEnd_UnityEvent(AnimationEvent data)
    {
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        var unit = other.GetComponent<UnitComponent>();
        if (unit != null)
        {
            unit.ReduceHealthAndKill(_damageOfHandAttack);
        }
    }

    protected void TransformWeaponToPoint()
    {
        switch (_weapon.GetWeaponType)
        {
            case Enums.WeaponType.Pistol:
                _weapon.transform.localPosition = _weaponSpawn;
                break;
            case Enums.WeaponType.Rifle:
                _weapon.transform.localPosition = _rifleSpawnPoint;
                break;
            case Enums.WeaponType.Shotgun:
                _weapon.transform.localPosition = _rifleSpawnPoint;
                break;
        }
    }
}

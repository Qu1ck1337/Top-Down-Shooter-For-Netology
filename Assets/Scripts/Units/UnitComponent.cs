using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class UnitComponent : MonoBehaviour
{
    [SerializeField]
    protected Animator _animator;

    //точка спавна, а для оружия offset
    [Space, SerializeField, Range(0, 100), Header("--- Аттрибуты юнита ---")]
    protected int _health;
    [SerializeField, Range(0f, 1000f)]
    protected float _movementSpeed;

    [Space, SerializeField, Header("--- Оружие, Урон ---")]
    protected Vector3 _weaponSpawn;
    [SerializeField]
    protected SimpleWeapon _weapon;
    [SerializeField]
    private float _dropWeaponImpulse = 100f;
    [SerializeField]
    protected int _damageOfHandAttack = 1;

    [Space, SerializeField, Header("--- Звуки ---")]
    protected AudioClip _punchSound;
    [SerializeField]
    protected AudioClip _dyingSound;

    protected AudioSource _audioSource;
    protected bool _lockAttack;
    protected Rigidbody _rigidBody;
    protected SphereCollider _handTrigger;

    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    //todo
    public delegate void UnitDeadEventHandler(EnemyComponent enemy);
    public event UnitDeadEventHandler OnUnitDeadEvent;

    public void ReduceHealthAndKill(int reduce)
    {
        _health -= reduce;
        if (_health == 0)
        {
            _animator.SetTrigger("IsDying");
            OnUnitDeadEvent?.Invoke((EnemyComponent)this);
            if (GetComponent<NavMeshAgent>() != null)
                Destroy(GetComponent<NavMeshAgent>());
            if (_weapon != null)
                DropWeapon();
            Destroy(GetComponent<Collider>());
            _rigidBody.isKinematic = true;
            _audioSource.PlayOneShot(_dyingSound);
        }
    }

    protected void SpawnWeapon(SimpleWeapon weapon)
    {
        _weapon = Instantiate(weapon, transform);
        _weapon.Owner = this;
        _weapon.transform.localPosition = _weaponSpawn + new Vector3(0f, 0f, _weapon.spawnWeaponOffsetOnZ);
        _weapon.ToggleColliders(false);
    }

    protected virtual void DropWeapon()
    {
        if (_weapon != null)
        {
            var parent = _weapon.transform.parent;
            _weapon.WeaponRigidBody.isKinematic = false;

            //можно закешировать
            Transform weaponTransform = _weapon.transform;
            weaponTransform.parent = null;
            weaponTransform.eulerAngles = new Vector3(0f, 0f, 30f);

            _weapon.WeaponRigidBody.AddForce(parent.forward * _dropWeaponImpulse);
            _weapon.Owner = null;
            _weapon.SetFlyingTrue();
            _weapon.ToggleColliders(true);
            _weapon = null;
        }
    }

    protected bool PickUpWeapon(SimpleWeapon weapon)
    {
        if (weapon == null || weapon.Owner != null || !weapon.CanBePickedUp) return false;
        _weapon = weapon;
        _weapon.Owner = this;
        _weapon.WeaponRigidBody.isKinematic = true;
        _weapon.transform.parent = transform;
        _weapon.transform.localPosition = _weaponSpawn + new Vector3(0f, 0f, _weapon.spawnWeaponOffsetOnZ);
        _weapon.transform.rotation = transform.rotation;
        _weapon.ToggleColliders(false);
        return true;
    }

    protected void OnHandCollisionToggle_UnityEvent(AnimationEvent data)
    {
        _handTrigger.enabled = !_handTrigger.enabled;
    }

    protected void OnAnimationEnd_UnityEvent(AnimationEvent data)
    {
        _lockAttack = false;
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
}

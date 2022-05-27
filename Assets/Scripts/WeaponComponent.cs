using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class WeaponComponent : MonoBehaviour
{
    //todo добавить поле для увеличения расстояния для выстрела врага
    [SerializeField]
    protected ProjectileComponent _projectile;
    [SerializeField]
    protected float _projectileDelay;
    [SerializeField, Range(0f, 1f)]
    protected float _projectileDeviation;

    [SerializeField]
    private float _radiusToFire;

    public float GetRadiusToFire() => _radiusToFire;

    [Space, SerializeField]
    private bool _showFireRadiusGizmos;

    [Space, SerializeField]
    protected int _allAmmo;
    [SerializeField]
    protected int _ammoInStore;
    [SerializeField]
    protected float _reloadTime;

    [SerializeField]
    protected int _currentAllAmmo;
    public int CurrentAllAmmo { get => _currentAllAmmo; }

    [SerializeField]
    protected int _currentAmmoInStore;

    [Space, SerializeField]
    private List<Collider> _hideCollidersWhenWeaponOnUnit = new List<Collider>();
    public int CurrentAmmoInStore { get => _currentAmmoInStore; }

    public UnitComponent Owner;

    private CapsuleCollider _triggerCollider;

    public bool CanBePickedUp { get; private set; }

    protected Rigidbody _rigidBody;
    public Rigidbody WeaponRigidBody => _rigidBody;

    protected Enums.WeaponType _weaponType;
    public Enums.WeaponType GetWeaponType => _weaponType;

    protected ProjectilePool _projectilePool;
    protected bool _isShootState = true;
    protected bool _isReloading;
    protected float _timer;

    private bool _isFlying;
    public void SetFlyingTrue() => _isFlying = true;

    private void Update()
    {
        if (Owner != null)
        {
            CanBePickedUp = false;
        }

        if (_isShootState == false)
        {
            _timer += Time.deltaTime;
            if (_timer >= _projectileDelay)
            {
                _isShootState = true;
                _timer = 0f;
            }
        }
    }

    public void checkAndFire()
    {
        if (_isShootState && !_isReloading && _currentAmmoInStore > 0)
        {
            _isShootState = false;
            Fire();
            BulletsCheck();
        }
    }

    private void BulletsCheck()
    {
        if (_currentAmmoInStore <= 0 && _currentAllAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(_reloadTime);
        if (_currentAllAmmo < _ammoInStore)
        {
            _currentAmmoInStore = _currentAllAmmo;
            _currentAllAmmo = 0;
        }
        else
        {
            _currentAmmoInStore = _ammoInStore;
            _currentAllAmmo -= _ammoInStore;
        }
        _isReloading = false;
    }

    protected virtual void Fire()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.isStatic == true)
        {
            _isFlying = false;
            CanBePickedUp = true;
        }
        else if (_isFlying && other.GetComponent<EnemyComponent>() != null)
        {
            other.GetComponent<EnemyComponent>().SetEnemyCooldown(2f);
        }
    }

    public void ToggleColliders()
    {
        foreach(Collider collider in _hideCollidersWhenWeaponOnUnit)
        {
            collider.enabled = !collider.enabled;
        }
    }

    private void OnDrawGizmos()
    {
        if (_showFireRadiusGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, _radiusToFire);
        }
    }
}

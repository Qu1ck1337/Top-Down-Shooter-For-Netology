using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField]
    protected ProjectileComponent _projectile;
    [SerializeField]
    protected float _projectileDelay;
    [SerializeField, Range(0f, 1f)]
    protected float _projectileDeviation;
    [SerializeField]
    private Enums.WeaponType _weaponType;

    public Enums.WeaponType GetWeaponType() => _weaponType;

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

    public int CurrentAmmoInStore { get => _currentAmmoInStore; }


    protected ProjectilePool _projectilePool;
    protected bool _isShootState = true;
    protected bool _isReloading;
    protected float _timer;

    private void Start()
    {
        _currentAllAmmo = _allAmmo;
        _currentAmmoInStore = _ammoInStore;
        _projectilePool = FindObjectOfType<ProjectilePool>();
    }

    private void Update()
    {
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

    protected virtual void BulletsCheck()
    {

    }

    protected virtual void Fire()
    {

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

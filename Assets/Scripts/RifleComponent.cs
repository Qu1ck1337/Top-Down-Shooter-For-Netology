using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleComponent : WeaponComponent
{
    private void Awake()
    {
        _weaponType = Enums.WeaponType.Rifle;
        _rigidBody = GetComponent<Rigidbody>();
        _currentAllAmmo = _allAmmo;
        _currentAmmoInStore = _ammoInStore;
        _projectilePool = FindObjectOfType<ProjectilePool>();
    }

    protected override void Fire()
    {
        var projectile = _projectilePool.GetProjectiles();
        projectile[0].gameObject.transform.position = transform.position;
        projectile[0].gameObject.transform.rotation = transform.rotation;
        projectile[0].SetMoving(true);
        projectile[0].Owner = GetComponentInParent<UnitComponent>();
        _currentAmmoInStore -= 1;
    }
}

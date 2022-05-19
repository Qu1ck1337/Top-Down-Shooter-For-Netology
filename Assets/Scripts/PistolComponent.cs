using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolComponent : WeaponComponent
{
    protected override void Fire()
    {
        var projectile = _projectilePool.GetProjectiles();
        projectile[0].gameObject.transform.position = transform.position;
        projectile[0].gameObject.transform.rotation = transform.rotation;
        projectile[0].SetMoving(true);
        projectile[0].Owner = GetComponentInParent<UnitComponent>();
        _currentAmmoInStore -= 1;
    }

    protected override void BulletsCheck()
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
}

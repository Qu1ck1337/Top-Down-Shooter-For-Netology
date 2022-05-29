using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiWeapon : SimpleWeapon
{
    protected override void Fire()
    {
        var projectile = _projectilePool.GetProjectiles(6);
        projectile[0].gameObject.transform.position = transform.position;
        projectile[0].gameObject.transform.rotation = transform.rotation;
        projectile[0].gameObject.transform.Rotate(new Vector3(0f, 1f, 0f));

        projectile[1].gameObject.transform.position = transform.position;
        projectile[1].gameObject.transform.rotation = transform.rotation;
        projectile[1].gameObject.transform.Rotate(new Vector3(0f, 3f, 0f));

        projectile[2].gameObject.transform.position = transform.position;
        projectile[2].gameObject.transform.rotation = transform.rotation;
        projectile[2].gameObject.transform.Rotate(new Vector3(0f, 6f, 0f));

        projectile[3].gameObject.transform.position = transform.position;
        projectile[3].gameObject.transform.rotation = transform.rotation;
        projectile[3].gameObject.transform.Rotate(new Vector3(0f, -1f, 0f));

        projectile[4].gameObject.transform.position = transform.position;
        projectile[4].gameObject.transform.rotation = transform.rotation;
        projectile[4].gameObject.transform.Rotate(new Vector3(0f, -3f, 0f));

        projectile[5].gameObject.transform.position = transform.position;
        projectile[5].gameObject.transform.rotation = transform.rotation;
        projectile[5].gameObject.transform.Rotate(new Vector3(0f, -6f, 0f));

        projectile[0].SetMoving(true);
        projectile[0].Owner = GetComponentInParent<UnitComponent>();
        projectile[1].SetMoving(true);
        projectile[1].Owner = GetComponentInParent<UnitComponent>();
        projectile[2].SetMoving(true);
        projectile[2].Owner = GetComponentInParent<UnitComponent>();
        projectile[3].SetMoving(true);
        projectile[3].Owner = GetComponentInParent<UnitComponent>();
        projectile[4].SetMoving(true);
        projectile[4].Owner = GetComponentInParent<UnitComponent>();
        projectile[5].SetMoving(true);
        projectile[5].Owner = GetComponentInParent<UnitComponent>();
        _currentAmmoInStore -= 1;
    }
}

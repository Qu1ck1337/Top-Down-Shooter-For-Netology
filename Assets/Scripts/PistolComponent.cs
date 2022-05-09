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
    }
}

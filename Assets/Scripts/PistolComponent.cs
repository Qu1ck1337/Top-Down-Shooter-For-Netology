using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolComponent : WeaponComponent
{
    protected override void Fire()
    {
        Instantiate(_projectile);
    }
}

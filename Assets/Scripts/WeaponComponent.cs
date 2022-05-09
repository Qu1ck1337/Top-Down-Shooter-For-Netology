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

    protected ProjectilePool _projectilePool;
    protected bool _isShootState = true;
    protected float _timer;

    private void Start()
    {
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
        if (_isShootState)
        {
            _isShootState = false;
            Fire();
        }
    }

    protected virtual void Fire()
    {

    }
}

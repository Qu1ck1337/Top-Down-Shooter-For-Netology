using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class SimpleWeapon : MonoBehaviour
{
    [SerializeField]
    private float _spawnWeaponOffsetOnZ;
    public float spawnWeaponOffsetOnZ => _spawnWeaponOffsetOnZ;

    [Space, SerializeField]
    protected float _projectileDelay;
    [SerializeField, Range(0f, 360f)]
    protected float _projectileDeviation;
    [SerializeField]
    private float _radiusToFire;
    public float GetRadiusToFire => _radiusToFire;

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
    [SerializeField]
    private Enums.WeaponType _weaponType;
    public Enums.WeaponType GetWeaponType => _weaponType;
    [SerializeField]
    private AudioClip _shootSound;

    private bool _isFlying;
    public void SetFlyingTrue() => _isFlying = true;
    private Collider[] _hideCollidersWhenWeaponOnUnit;
    private AudioSource _shootAudioSource;
    private ParticleSystem _fireParticles;

    protected Rigidbody _rigidBody;
    public Rigidbody WeaponRigidBody => _rigidBody;
    protected ProjectilePool _projectilePool;
    protected bool _isShootState = true;
    protected bool _isReloading;
    protected float _timer;

    public bool CanBePickedUp { get; private set; }
    public UnitComponent Owner;

    private void Awake()
    {
        _weaponType = Enums.WeaponType.Rifle;
        _rigidBody = GetComponent<Rigidbody>();
        _currentAllAmmo = _allAmmo;
        _currentAmmoInStore = _ammoInStore;
        _projectilePool = FindObjectOfType<ProjectilePool>();
        _hideCollidersWhenWeaponOnUnit = GetComponents<Collider>();
        _shootAudioSource = GetComponent<AudioSource>();
        _fireParticles = GetComponentInChildren<ParticleSystem>();
    }

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
            if (_shootAudioSource != null)
                _shootAudioSource.PlayOneShot(_shootSound);
            if (_fireParticles != null)
                _fireParticles.Play();
        }
    }

    protected virtual void Fire()
    {
        var projectile = _projectilePool.GetProjectiles();
        projectile[0].gameObject.transform.position = transform.position;
        projectile[0].gameObject.transform.rotation = transform.rotation;
        projectile[0].gameObject.transform.Rotate(new Vector3(0f, Random.Range(-_projectileDeviation, _projectileDeviation), 0f));
        projectile[0].SetMoving(true);
        projectile[0].Owner = GetComponentInParent<UnitComponent>();
        _currentAmmoInStore -= 1;
    }

    private void BulletsCheck()
    {
        if (_currentAmmoInStore <= 0 && _currentAllAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    public delegate void WeaponReloadingEventHandler();
    public event WeaponReloadingEventHandler OnWeaponReloadingEvent;

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
        OnWeaponReloadingEvent?.Invoke();
    }

    public void ToggleColliders(bool state)
    {
        foreach(Collider collider in _hideCollidersWhenWeaponOnUnit)
        {
            collider.enabled = state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            _isFlying = false;
            CanBePickedUp = true;
        }
        else if (_isFlying && other.GetComponent<EnemyComponent>() != null)
        {
            other.GetComponent<EnemyComponent>().SetEnemyCooldown(2f);
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

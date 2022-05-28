using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponent : UnitComponent
{
    [SerializeField, Range(0f, 1000f)]
    protected float _maxVelocity;

    public SimpleWeapon PlayerWeapon => _weapon;

    private PlayerControls _controls;

    protected override void Awake()
    {
        base.Awake();
        _controls = new PlayerControls();
        _handTrigger = GetComponentInChildren<SphereCollider>();
        if (_weapon != null)
        {
            _weapon = Instantiate(_weapon, transform);
            _weapon.Owner = this;
            TransformWeaponToPoint();
            _weapon.ToggleColliders();
            _weapon.OnWeaponReloadingEvent += ReloadingWeapon;
        }
        _controls.Player.Fire.performed += TouchFire;
        _controls.Player.DropWeapon.performed += _ => DropWeapon();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void Update()
    {
        RotationLogic();
    }

    private void FixedUpdate()
    {
        Vector2 direction = _controls.Player.Movement.ReadValue<Vector2>();
        MovementLogic(direction);
        UpdateAnimation(direction);
    }
    private void UpdateAnimation(Vector2 direction)
    {
        var velocity = _rigidBody.velocity.normalized;

        if (direction.x == 0 && direction.y == 0) _animator.SetBool("IsMoving", false);
        else
        {
            _animator.SetBool("IsMoving", true);
            _animator.SetFloat("HorizontalMoving", velocity.x);
            _animator.SetFloat("VerticalMoving", velocity.z);
        }
    }

    private void MovementLogic(Vector2 direction)
    {
        _rigidBody.AddForce(new Vector3(direction.x, 0, direction.y) * _movementSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        //_rigidBody.velocity = Vector3.ClampMagnitude(_rigidBody.velocity, _movementSpeed);
        //transform.position += new Vector3(direction.x, 0f, direction.y) * _movementSpeed * Time.deltaTime;
        if (_rigidBody.velocity.magnitude >= _maxVelocity)
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * _maxVelocity;
        }
    }

    private float _angleY;
    private float _angleX;
    private void RotationLogic()
    {
        var pos = _controls.Player.MouseRotation.ReadValue<Vector2>();
        transform.LookAt(new Vector3(pos.x - Screen.width / 2, transform.position.y, pos.y - Screen.height / 2));

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward * 10);
    }

    public delegate void PlayerActionEventHandler(Enums.PlayerActionType actionType);
    public event PlayerActionEventHandler OnPlayerActionEvent;

    private void TouchFire(InputAction.CallbackContext context)
    {
        if (_weapon != null)
        {
            _weapon.checkAndFire();
            OnPlayerActionEvent.Invoke(Enums.PlayerActionType.Shoot);
        }
        else
        {
            if (!_inAnimation)
            {
                _animator.SetTrigger("HandAttack");
                _inAnimation = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_weapon != null) return;
        var weaponComponent = collision.gameObject.GetComponent<SimpleWeapon>();
        if (weaponComponent != null && weaponComponent.Owner == null && weaponComponent.CanBePickedUp)
        {
            _weapon = weaponComponent;
            _weapon.Owner = this;
            _weapon.WeaponRigidBody.isKinematic = true;
            _weapon.transform.parent = transform;
            TransformWeaponToPoint();
            _weapon.transform.rotation = transform.rotation;
            _weapon.ToggleColliders();
            _weapon.OnWeaponReloadingEvent += ReloadingWeapon;
            OnPlayerActionEvent.Invoke(Enums.PlayerActionType.PickUpWeapon);
        }
    }

    private void ReloadingWeapon()
    {
        OnPlayerActionEvent.Invoke(Enums.PlayerActionType.ReloadedWeapon);
    }

    protected override void DropWeapon()
    {
        _weapon.OnWeaponReloadingEvent -= ReloadingWeapon;
        base.DropWeapon();
        OnPlayerActionEvent.Invoke(Enums.PlayerActionType.DropWeapon);
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_weaponSpawn + transform.position, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_rifleSpawnPoint + transform.position, 0.1f);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponent : UnitComponent
{
    private PlayerControls _controls;
    [SerializeField]
    private Animator _animator;
    
    private Rigidbody _rigidBody;

    private void Awake()
    {
        _controls = new PlayerControls();
        _rigidBody = GetComponent<Rigidbody>();
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
        MovementLogic();
    }

    private void Start()
    {
        _controls.Player.Fire.performed += FireLogic;
    }

    private void MovementLogic()
    {
        Vector2 direction = _controls.Player.Movement.ReadValue<Vector2>();
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

    private void FireLogic(InputAction.CallbackContext context)
    {
        if (_weapon != null)
            _weapon.checkAndFire();
        else
        {
            _animator.SetTrigger("HandAttack");
        }

    }


    private void OnDisable()
    {
        _controls.Disable();
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }
}

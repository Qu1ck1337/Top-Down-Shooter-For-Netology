using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponent : UnitComponent
{
    PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void Update()
    {
        MovementLogic();
        RotationLogic();
    }

    private void Start()
    {
        _controls.Player.Fire.performed += FireLogic;
    }

    private void MovementLogic()
    {
        Vector2 direction = _controls.Player.Movement.ReadValue<Vector2>();
        transform.position += new Vector3(direction.x, 0f, direction.y) * _movementSpeed * Time.deltaTime;
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
        _weapon.checkAndFire();
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

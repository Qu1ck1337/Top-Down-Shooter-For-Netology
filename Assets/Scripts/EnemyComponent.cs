using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : UnitComponent
{
    [SerializeField]
    private float _playerIdentificationRadius;
    [SerializeField]
    private float _fieldOfView;

    [Space, SerializeField]
    private Vector3[] _queuePointsForPatrolling;
    [SerializeField]
    private float _timeToStayAtPatrollPointMax;
    [SerializeField]
    private float _timeToStayAtPatrollPointMin;

    [Space, SerializeField]
    private bool _checkGizmos;

    [Space, SerializeField]
    private bool _checkPistolPoint;
    [SerializeField]
    private Vector3 _pointForPistol;

    [Space, SerializeField]
    private float _enemyCooldownAfterFire;

    private Vector3 _nextPatrollingPoint;
    private bool _isStayingAtPoint;
    private Queue<Vector3> _pointsForPatrolling = new Queue<Vector3>();
    private Transform _player;
    private Transform _target;
    private bool _inCooldown;

    private void Start()
    {
        if (_weapon != null)
        {
            var weapon = Instantiate(_weapon, transform);
            switch (_weapon.GetWeaponType())
            {
                case Enums.WeaponType.Pistol:
                    weapon.transform.position = _pointForPistol + transform.position;
                    break;
            }
            _weapon = weapon;
        }

        _player = FindObjectOfType<PlayerComponent>().GetComponent<Transform>();
        foreach (Vector3 point in _queuePointsForPatrolling)
        {
            _pointsForPatrolling.Enqueue(point);
        }
        _pointsForPatrolling.Enqueue(transform.position);
        _nextPatrollingPoint = _pointsForPatrolling.Dequeue();
        _pointsForPatrolling.Enqueue(_nextPatrollingPoint);
    }

    private void Update()
    {
        FolowingLogic();
        PlayerIdentification();
        RayDetection();
        FireLogic();
    }

    private IEnumerator ChangePatrollingPoint()
    {
        yield return new WaitForSeconds(Random.Range(_timeToStayAtPatrollPointMin, _timeToStayAtPatrollPointMax));
        _nextPatrollingPoint = _pointsForPatrolling.Dequeue();
        _pointsForPatrolling.Enqueue(_nextPatrollingPoint);
        _isStayingAtPoint = false;
    }

    private void FolowingLogic()
    {
        if (_inCooldown) return;
        if (_target == null)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_nextPatrollingPoint.x, transform.position.y, _nextPatrollingPoint.z), _movementSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(_nextPatrollingPoint.x, transform.position.y, _nextPatrollingPoint.z));
            if (Vector3.Distance(transform.position, new Vector3(_nextPatrollingPoint.x, transform.position.y, _nextPatrollingPoint.z)) < 0.2f && !_isStayingAtPoint)
            {
                StartCoroutine(ChangePatrollingPoint());
                _isStayingAtPoint = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, _movementSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(_target.position.x, transform.position.y, _target.position.z));
        }
    }

    private void PlayerIdentification()
    {
        if (Vector3.Distance(transform.position, _player.position) <= _playerIdentificationRadius)
        {
            _target = _player;
        }
        else if (Vector3.Distance(transform.position, _player.position) > _fieldOfView)
        {
            _target = null;
        }
    }

    private void RayDetection()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<PlayerComponent>())
        {
            _target = _player;
        }
    }

    private void OnDrawGizmos()
    {   
        if (_checkGizmos)
        {
            foreach (Vector3 point in _queuePointsForPatrolling)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(point, 0.5f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, _playerIdentificationRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, _fieldOfView);
        }
        if (_checkPistolPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_pointForPistol + transform.position, 0.1f);
        }
    }

    private void FireLogic()
    {
        if (_inCooldown || _target == null) return;
        if (Vector3.Distance(transform.position, _target.position) <= _weapon.GetRadiusToFire())
        {
            _weapon.checkAndFire();
            StartCoroutine(EnemyMoveCooldown());
            _inCooldown = true;
        }
    }

    private IEnumerator EnemyMoveCooldown()
    {
        yield return new WaitForSeconds(_enemyCooldownAfterFire);
        if (_weapon.CurrentAllAmmo > 0 && _weapon.CurrentAmmoInStore > 0)
            _inCooldown = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private AudioSource _missSound;
    [SerializeField]
    private GameObject _bloodParticles;
    [SerializeField]
    private GameObject _missParticles;

    public UnitComponent Owner;

    private bool _isMoving = false;

    public void SetMoving(bool status) => _isMoving = status;

    void Update()
    {
        if (_isMoving)
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        UnitComponent unit = collision.gameObject.GetComponent<UnitComponent>();
        if (unit != null)
        {
            transform.position = transform.parent.position;
            _isMoving = false;
            unit.ReduceHealthAndKill(_damage);
            var blood = Instantiate(_bloodParticles, collision.GetContact(0).point, transform.rotation);
            blood.transform.parent = collision.gameObject.transform;
        }
        else if (collision.gameObject.isStatic)
        {
            transform.position = transform.parent.position;
            _isMoving = false;
            _missSound.Play();
            Instantiate(_missParticles, collision.GetContact(0).point, transform.rotation);
        }
    }
}

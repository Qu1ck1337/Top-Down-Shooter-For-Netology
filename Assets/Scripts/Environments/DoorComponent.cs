using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class DoorComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject _doorDestroyEffects;
    [SerializeField]
    private float _kickForce;

    private Rigidbody _rigidbody;
    private bool _isDead;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(transform.localScale.x / 2, 0, transform.localScale.z / 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDead) return;
        if (collision.gameObject.GetComponent<ProjectileComponent>())
        {
            _isDead = true;
            _rigidbody.constraints = RigidbodyConstraints.None;
            _rigidbody.centerOfMass = new Vector3(0, 0, 0); //transform.localScale.y / 2
            _rigidbody.AddForce(collision.impulse * _kickForce);
            StartCoroutine(DestroyDoor());
        }
    }

    private IEnumerator DestroyDoor()
    {
        yield return new WaitForSeconds(0.1f);
        var effects = Instantiate(_doorDestroyEffects);
        effects.transform.position = transform.position;
        //GetComponent<Collider>().isTrigger = true;
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}

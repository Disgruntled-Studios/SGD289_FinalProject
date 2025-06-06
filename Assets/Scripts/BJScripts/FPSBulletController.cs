using System;
using System.Collections;
using UnityEngine;

public class FPSBulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    
    private const float BulletLifespan = 0.5f;
    private const float BulletSpeed = 15f;
    
    public void Initialize()
    {
        _rb.AddForce(transform.forward * BulletSpeed, ForceMode.Impulse);
        StartCoroutine(BulletLifeCycle());
    }

    private IEnumerator BulletLifeCycle()
    {
        yield return new WaitForSeconds(BulletLifespan);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}

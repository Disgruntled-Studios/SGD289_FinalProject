using System;
using System.Collections;
using UnityEngine;

public class FPSBulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    
    private const float BulletLifespan = 0.5f;
    private const float BulletSpeed = 15f;
    
    public Material CurrentMat { get; set; }
    
    public void InitializeAndFire(Material startingMat)
    {
        CurrentMat = startingMat;
        GetComponent<MeshRenderer>().material = CurrentMat;
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
        if (other.gameObject.CompareTag("Player")) return;
        
        Destroy(gameObject);
    }
}

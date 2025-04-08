using System;
using UnityEngine;

public class BulletTransform : MonoBehaviour
{
    public GameObject bulletPrefab;    
    public Transform[] firePoints; 
    public float speed = 100f;

    private int currentFirePoint = 0; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (firePoints.Length == 0) return; 

        Transform firePoint = firePoints[currentFirePoint]; 

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * speed;
        }


        currentFirePoint = (currentFirePoint + 1) % firePoints.Length;
    }
}

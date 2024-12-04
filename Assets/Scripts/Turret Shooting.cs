using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooting : MonoBehaviour
{
    // Reference to the projectile prefab
    public GameObject projectilePrefab;

    // Speed of the projectiles
    public float projectileSpeed = 10f;

    // Time interval between shots
    public float fireInterval = 1f;

    // Whether the turret is currently shooting
    private bool isShooting = true;

    void Start()
    {
        // Start shooting projectiles in a loop when the turret is initialized
        StartCoroutine(ShootProjectiles());
    }

    // Coroutine to handle the shooting behavior
    IEnumerator ShootProjectiles()
    {
        while (true) // This will run continuously
        {
            if (isShooting)
            {
                // Shoot the projectile
                ShootProjectile();
            }

            // Wait for the next shot based on the fire interval
            yield return new WaitForSeconds(fireInterval);
        }
    }

    // Method to shoot a projectile
    void ShootProjectile()
    {
        // Instantiate a projectile at the turret's position and orientation
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);

        // Get the Rigidbody of the projectile to control its movement
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Apply a forward force to the projectile to make it move
            rb.velocity = transform.forward * projectileSpeed;
        }
    }

    // Method to start or stop shooting (can be toggled at runtime)
    public void ToggleShooting(bool shoot)
    {
        isShooting = shoot;
    }

    // Method to change the firing interval (rate of fire)
    public void SetFireInterval(float interval)
    {
        fireInterval = interval;
    }

    // Method to change the projectile speed
    public void SetProjectileSpeed(float speed)
    {
        projectileSpeed = speed;
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLifetime : MonoBehaviour
{
    public WeaponController creator;

    // Bullet flies for 5 seconds, then is destroyed
    private void Start()
    {
        Destroy(gameObject, 5);
    }

    // If the bullet hits something, destroy it
    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.transform.gameObject;

        if (hitObject.tag == "Car")
        {
            creator.DealDamage(hitObject);
        }

        Destroy(gameObject);
    }
}

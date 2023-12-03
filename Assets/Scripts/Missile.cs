using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public WeaponController creator;
    public GameObject missileTarget;
    public bool armed = false;

    public void MissileFired()
    {
        Rigidbody newRB = gameObject.AddComponent<Rigidbody>();
        newRB.mass = 10;

        // Detach from parent to stop it from moving with the gun
        transform.parent = null;
        armed = true;

        // Thruster effect
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        // Go towards target
        if (armed)
        {
            transform.LookAt(missileTarget.transform);
            GetComponent<Rigidbody>().velocity = transform.forward * 60;
        }
    }

    // If the missile hits something, destroy it
    private void OnCollisionEnter(Collision collision)
    {
        // Exclude the firer's vehicle
        if (collision.gameObject != creator.gameObject)
        {
            GameObject hitObject = collision.transform.gameObject;

            if (hitObject.tag == "Car")
            {
                creator.DealDamage(hitObject);
            }

            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Transform weaponMount;
    public int weapNum;
    public bool hasWeapon = false;
    public int ammo;
    GameObject weapon;
    public float firingDelay;
    public bool canFire = true;
    public bool canRotate;
    public float weaponDamage;
    public GameObject waypointBundle;
    public GameObject target;

    private void Awake()
    {
        weaponMount = transform.Find("WeaponMount");
    }

    void FixedUpdate()
    {
        // If a car has a weapon that's out of ammo, destroy it.
        if (ammo <= 0 && weaponMount.childCount > 0)
        {
            Destroy(weapon);
            hasWeapon = false;
        }

        // Make the weapon look at the next target
        if (hasWeapon && canRotate)
        {
            weapon.transform.LookAt(target.transform);
        }
    }

    // If the controller's weapon has ammo, fire it. No need to check for having a weapon in the first place due to Player and AI checking that.
    public void fireWeapon()
    {
        if (ammo > 0 && canFire)
        {
            if (weapNum == 1)
            {
                FireGun();
            }
            else if (weapNum == 2)
            {
                FireMissile();
            }
            else if(weapNum == 3)
            {
                FireLightningRod();
            }

            ammo--;
            StartCoroutine(delayFire(firingDelay));
        }
    }

    // Delay script to stop instant expenditure of all ammo.
    private IEnumerator delayFire(float holdTime)
    {
        canFire = false;
        yield return new WaitForSeconds(holdTime);
        canFire = true;
    }

    // Rolls a random weapon and changes parameters to match it
    public void WeaponSelect()
    {
        // Check if a player doesn't already have a weapon
        if (!hasWeapon)
        {
            hasWeapon = true;
            weapNum = Random.Range(1, 4);

            // Gun
            if (weapNum == 1)
            {
                weapon = Instantiate(Resources.Load("Gun", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 20;
                firingDelay = 0.2f;
                canRotate = true;
                weaponDamage = 0.2f;
            }

            // Missile
            else if (weapNum == 2)
            {
                weapon = Instantiate(Resources.Load("Rocket Launcher", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 4;
                firingDelay = 0.5f;
                canRotate = true;
                weaponDamage = 1f;
            }

            // Lightning Rod
            else if(weapNum == 3)
            {
                weapon = Instantiate(Resources.Load("Lightning Rod", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 1;
                firingDelay = 0.1f;
                canRotate = false;
                weapon.transform.localEulerAngles = new Vector3(0f, -90f, 45f);
                weaponDamage = 8f;
            }
        }
    }

    // Deals weaponDamage to a target
    public void DealDamage(GameObject damagedTarget)
    {
        // Deal the damage
        if (damagedTarget.TryGetComponent(out PlayerInput playerScript))
        {
            if (!playerScript.invincible)
            {
                playerScript.damagePenalty += weaponDamage;
            }
        }
        else if (damagedTarget.TryGetComponent(out AI_Input aiScript))
        {
            if (!aiScript.invincible)
            {
                aiScript.damagePenalty += weaponDamage;
            }
        }
    }

    public void FireLightningRod()
    {
        List<placingData> positions = waypointBundle.GetComponent<RacingManager>().placements;

        foreach (placingData vehicle in positions)
        {
            if (vehicle.car != this.gameObject)
            {
                DealDamage(vehicle.car);

                // Bolt effect
                GameObject bolt = Instantiate(Resources.Load("Lightning Bolt", typeof(GameObject)) as GameObject, vehicle.car.transform);
                Destroy(bolt, 1f);
            }
        }
    }

    public void FireGun()
    {
        // Create a bullet at the barrel of the gun, and give it a velocity
        GameObject bullet = Instantiate(Resources.Load("Bullet", typeof(GameObject)) as GameObject, weaponMount.GetChild(0).GetChild(0));
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.TransformDirection(Vector3.forward * 100);

        // Tell the bullet it came from here so it can pass back what to damage if it hits a car
        bullet.GetComponent<BulletLifetime>().creator = this;

        // Detach from parent to stop it from moving with the gun
        bullet.transform.parent = null;
    }

    public void FireMissile()
    {
        // Get the next missile
        GameObject missile = weapon.transform.GetChild(0).gameObject;

        // Tell the missile it came from here and what its target is, so it can pass back what to damage if it hits a car
        missile.GetComponent<Missile>().creator = this;
        missile.GetComponent<Missile>().missileTarget = target;

        // Fire the missile
        missile.GetComponent<Missile>().MissileFired();
    }
}
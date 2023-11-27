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
    }

    // Make the weapon look at the next target
    public void lookAtTarget(GameObject target)
    {
        if (canRotate)
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
                
            }
            else if (weapNum == 2)
            {

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
                ammo = 10;
                firingDelay = 0.2f;
                canRotate = true;
                weaponDamage = 0.5f;
            }

            // Missile
            else if (weapNum == 2)
            {
                weapon = Instantiate(Resources.Load("Rocket Launcher", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 4;
                firingDelay = 0.5f;
                canRotate = true;
                weaponDamage = 4f;
            }

            // Lightning Rod
            else if(weapNum == 3)
            {
                weapon = Instantiate(Resources.Load("Lightning Rod", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 1;
                firingDelay = 0.1f;
                canRotate = false;
                weapon.transform.localEulerAngles = new Vector3(0f, -90f, 45f);
                weaponDamage = 10f;
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
                // Deal the damage
                if (vehicle.car.TryGetComponent(out PlayerInput playerScript))
                {
                    playerScript.damagePenalty += weaponDamage;
                }
                else
                {
                    vehicle.car.GetComponent<AI_Input>().damagePenalty += weaponDamage;
                }

                // Bolt effect
                GameObject bolt = Instantiate(Resources.Load("Lightning Bolt", typeof(GameObject)) as GameObject, vehicle.car.transform);
                Destroy(bolt, 1f);
            }
        }
    }
}

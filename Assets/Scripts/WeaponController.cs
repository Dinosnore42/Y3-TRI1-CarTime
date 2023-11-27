using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Transform weaponMount;
    public bool hasWeapon = false;
    public int ammo;
    GameObject weapon;

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

    public void WeaponSelect()
    {
        // Check if a player doesn't already have a weapon
        if (!hasWeapon)
        {
            hasWeapon = true;
            int weapNum = Random.Range(1, 4);

            // Gun
            if (weapNum == 1)
            {
                weapon = Instantiate(Resources.Load("Gun", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 10;
            }

            // Missile
            else if (weapNum == 2)
            {
                weapon = Instantiate(Resources.Load("Rocket Launcher", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 4;
            }

            // Lightning Rod
            else if(weapNum == 3)
            {
                weapon = Instantiate(Resources.Load("Lightning Rod", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 1;
            }
        }
    }
}

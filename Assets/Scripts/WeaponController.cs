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
        if (ammo == 0 && weaponMount.childCount > 0)
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
            int weapNum = 1; //Random.RandomRange(1, 3);

            // Gun
            if (weapNum == 1)
            {
                weapon = Instantiate(Resources.Load("Gun", typeof(GameObject)) as GameObject, weaponMount);
                ammo = 10;
            }

            // Missile
            else if (weapNum == 2)
            {

            }

            // Lightning Rod
            else
            {

            }
        }
    }
}

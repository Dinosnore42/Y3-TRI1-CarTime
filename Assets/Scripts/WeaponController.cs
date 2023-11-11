using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Transform weaponMount;
    private GameObject gunObject;

    private void Awake()
    {
        weaponMount = transform.Find("WeaponMount");
        gunObject = Resources.Load("Gun", typeof(GameObject)) as GameObject;
    }

    void FixedUpdate()
    {
        
    }

    public void WeaponSelect()
    {
        int weapNum = 1; //Random.RandomRange(1, 3);

        // Gun
        if (weapNum == 1)
        {
            Debug.Log("Bazinga");
            Instantiate(gunObject, weaponMount);
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

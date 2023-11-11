using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(WeaponController))]
public class PlayerInput : MonoBehaviour
{
    private CarController playerCar;
    private WeaponController playerWeapons;
    //public List<CarController> carList = new List<CarController>();

    // Start is called before the first frame update
    void Start()
    {
        playerCar = GetComponent<CarController>();
        playerWeapons = GetComponent<WeaponController>();
        playerCar.Identity(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerCar.InputResponse(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WeaponPickup"))
        {
            playerWeapons.WeaponSelect();
        }
    }
}

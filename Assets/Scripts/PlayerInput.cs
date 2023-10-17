using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class PlayerInput : MonoBehaviour
{
    private CarController playerCar;

    // Start is called before the first frame update
    void Start()
    {
        playerCar = GetComponent<CarController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerCar.InputResponse(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }
}

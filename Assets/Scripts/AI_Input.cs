using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AI_Input : MonoBehaviour
{
    private CarController aiCar;

    // Start is called before the first frame update
    void Start()
    {
        aiCar = GetComponent<CarController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        aiCar.InputResponse(1f, 0f);
    }
}

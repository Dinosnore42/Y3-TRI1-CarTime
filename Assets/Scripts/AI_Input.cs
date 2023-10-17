using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AI_Input : MonoBehaviour
{
    private CarController aiCar;
    private float vertical;
    private float horizontal;

    // Start is called before the first frame update
    void Start()
    {
        aiCar = GetComponent<CarController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;

        horizontal = 0;

        var leftRay = new Ray(this.transform.position, -this.transform.right);
        if (Physics.Raycast(leftRay, out hit, 7.5f))    // Use layers to not hit certain things
        {
            horizontal += 0.1f;
        }

        var rightRay = new Ray(this.transform.position, this.transform.right);
        if (Physics.Raycast(rightRay, out hit, 7.5f))
        {
            horizontal += -0.1f;
        }

        var forRay = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(forRay, out hit, 15f))    // Use layers to not hit certain things
        {
            vertical = -0.5f;
        }
        else
        {
            vertical = 0.5f;
        }

        aiCar.InputResponse(vertical, horizontal);
    }
}

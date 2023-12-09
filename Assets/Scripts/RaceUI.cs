using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceUI : MonoBehaviour
{
    // Stuff to grab
    public GameObject playerCar;
    public WeaponController playerWeaponController;
    public PlayerInput playerInput;
    public CarController playerCarController;

    // Output
    public TextMeshProUGUI carSpeed;
    public TextMeshProUGUI carGear;
    public TextMeshProUGUI carLap;

    // Start is called before the first frame update
    void Start()
    {
        playerWeaponController = playerCar.GetComponent<WeaponController>();
        playerInput = playerCar.GetComponent<PlayerInput>();
        playerCarController = playerCar.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = (int)Mathf.Round(playerCar.GetComponent<Rigidbody>().velocity.magnitude * 2.237f);
        float reccomendedSpeed = playerInput.target.transform.localScale.x;

        // Show if player is above reccomended speed
        if (reccomendedSpeed - speed < 0)
        {
            carSpeed.color = Color.red;
        }
        else
        {
            carSpeed.color = Color.green;
        }

        carSpeed.text = speed.ToString() + "mph / " + reccomendedSpeed.ToString() + "mph";
        carGear.text = playerCarController.curGear.ToString();
        carLap.text = (playerInput.lapsFinished + 1) + " / 3";
    }
}

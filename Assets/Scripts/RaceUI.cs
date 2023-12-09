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
    public GameObject raceManager;
    public GameObject endUI;

    // Output
    public TextMeshProUGUI carSpeed;
    public TextMeshProUGUI carGear;
    public TextMeshProUGUI carLap;
    public TextMeshProUGUI carWeapon;
    public TextMeshProUGUI carAmmo;
    public TextMeshProUGUI carPlace;
    public TextMeshProUGUI carPenalty;

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
        if (raceManager.GetComponent<RacingManager>().calledEnd)
        {
            endUI.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            #region Speed

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

            #endregion

            #region Weapon

            string currentWeap = "";

            if (playerWeaponController.hasWeapon == true)
            {
                if (playerWeaponController.weapNum == 1)
                {
                    currentWeap = "Gun Turret";
                }
                else if (playerWeaponController.weapNum == 2)
                {
                    currentWeap = "Rocket Launcher";
                }
                else if (playerWeaponController.weapNum == 3)
                {
                    currentWeap = "Lightning Rod";
                }
            }
            else
            {
                currentWeap = "Nothing";
            }

            #endregion

            #region Positioning

            int i = 0;
            int carPosition = 0;
            float carPenalisation = 0f;
            List<placingData> positions = new List<placingData>(raceManager.GetComponent<RacingManager>().placements);

            foreach (placingData vehicle in positions)
            {
                i++;

                if (vehicle.car == playerCar)
                {
                    carPosition = i;
                    carPenalisation = vehicle.penalty;
                }
            }

            #endregion

            // Set text
            carSpeed.text = speed.ToString() + "mph / " + reccomendedSpeed.ToString() + "mph";
            carGear.text = playerCarController.curGear.ToString();
            carLap.text = (playerInput.lapsFinished + 1) + " / " + raceManager.GetComponent<RacingManager>().numOfLapsInRace;
            carWeapon.text = currentWeap;
            carAmmo.text = "Ammo: " + playerWeaponController.ammo.ToString();
            carPlace.text = "Position: " + carPosition + " / 8";
            carPenalty.text = "Penalty: " + carPenalisation + "s";
        }
    }
}

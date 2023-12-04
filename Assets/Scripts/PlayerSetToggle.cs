using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSetToggle : MonoBehaviour
{
    public TextMeshProUGUI togAutoGearBut;
    public TextMeshProUGUI tog_ABS_But;
    public TextMeshProUGUI tog_TCS_But;

    private void Start()
    {
        // Set toggles to be on by default
        PlayerPrefs.SetInt("AutoGears", 1);
        PlayerPrefs.SetInt("ABS", 1);
        PlayerPrefs.SetInt("TCS", 1);
    }

    private void Update()
    {
        // Displayed text in settings menu
        togAutoGearBut.text = "Toggle Automatic Gears " + ToggleStatusChecker(PlayerPrefs.GetInt("AutoGears"));
        tog_ABS_But.text = "Toggle ABS " + ToggleStatusChecker(PlayerPrefs.GetInt("ABS"));
        tog_TCS_But.text = "Toggle TCS " + ToggleStatusChecker(PlayerPrefs.GetInt("TCS"));
    }

    public string ToggleStatusChecker(int value)
    {
        if (value == 0)
        {
            return "(Disabled)";
        }
        else
        {
            return "(Enabled)";
        }
    }

    public void ToggleAutoGears()
    {
        PlayerPrefs.SetInt("AutoGears", ToggleValue(PlayerPrefs.GetInt("AutoGears")));
    }

    public void ToggleABS()
    {
        PlayerPrefs.SetInt("ABS", ToggleValue(PlayerPrefs.GetInt("ABS")));
    }

    public void ToggleTCS()
    {
        PlayerPrefs.SetInt("TCS", ToggleValue(PlayerPrefs.GetInt("TCS")));
    }

    public int ToggleValue(int currentVal)
    {
        if (currentVal == 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

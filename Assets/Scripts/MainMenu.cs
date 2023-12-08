using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        // Create and set settings values to on if no value exists
        if (PlayerPrefs.HasKey("AutoGears") && PlayerPrefs.HasKey("ABS") && PlayerPrefs.HasKey("TCS") == false)
        {
            PlayerPrefs.SetInt("AutoGears", 1);
            PlayerPrefs.SetInt("ABS", 1);
            PlayerPrefs.SetInt("TCS", 1);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

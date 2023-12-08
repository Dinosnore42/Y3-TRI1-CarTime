using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Resume();
        SceneManager.LoadScene(0);
    }
}

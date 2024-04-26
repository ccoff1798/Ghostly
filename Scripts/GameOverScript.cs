using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    public static GameOverScript instance;
    public GameObject gameOverScreen;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameOverScreen.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1; // Make sure to reset the time scale, or the game will stay paused
    }
}


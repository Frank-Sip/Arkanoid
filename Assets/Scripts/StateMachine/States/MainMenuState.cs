using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 0f;
        Debug.Log("Press any key to Play");
        gameManager.MainMenuLayout.SetActive(true);
    }

    public override void Tick(GameManager gameManager)
    {

    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving Main Menu");
        gameManager.MainMenuLayout.SetActive(false);
    }
}

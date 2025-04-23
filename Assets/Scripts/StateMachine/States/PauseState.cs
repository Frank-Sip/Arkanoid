using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Debug.Log("Press Esc to resume");
        Time.timeScale = 0f;
        gameManager.PauseLayout.SetActive(true);

    }

    public override void Tick(GameManager gameManager)
    {

    }

    public override void Exit(GameManager gameManager)
    {
        Debug.Log("Leaving Pause");
        gameManager.PauseLayout.SetActive(false);
    }
}

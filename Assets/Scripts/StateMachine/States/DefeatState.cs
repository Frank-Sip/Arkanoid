using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatState : GameState
{
    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 0f;
        gameManager.DefeatLayout.SetActive(true);
        var audioManager = ServiceProvider.GetService<AudioManager>();
        audioManager.PlayBGM(3);
    }

    public override void Tick(GameManager gameManager)
    {
        
    }

    public override void Exit(GameManager gameManager)
    {
        gameManager.DefeatLayout.SetActive(false);
    }
}
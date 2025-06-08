using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : GameState
{
    public override async void Enter(GameManager gameManager)
    {
        await AddressablesManager.LoadGroupAsync("UIMenu");
        Time.timeScale = 0f;
        gameManager.MainMenuLayout.SetActive(true);
        var audioManager = ServiceProvider.GetService<AudioManager>();
        audioManager.PlayBGM(0);
    }

    public override void Tick(GameManager gameManager)
    {

    }

    public override async void Exit(GameManager gameManager)
    {
        gameManager.MainMenuLayout.SetActive(false);
        await AddressablesManager.UnloadGroupAsync("UIMenu");
    }
}
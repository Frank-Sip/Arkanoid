using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : GameState
{    public override void Enter(GameManager gameManager)
    {
        Time.timeScale = 0f;
        gameManager.MainMenuLayout.SetActive(true);
        var audioManager = ServiceProvider.GetService<AudioManager>();
        audioManager.PlayBGM(0);
        
        var addressableManager = ServiceProvider.GetService<AddressableManager>();
        if (!addressableManager.IsMenuUILoaded)
        {
            addressableManager.LoadMenuUIAsync(
                onComplete: () => Debug.Log("MenuUI loaded in MainMenu"),
                onError: (error) => Debug.LogError($"Failed to load MenuUI in MainMenu: {error}")
            );
        }
    }

    public override void Tick(GameManager gameManager)
    {

    }

    public override void Exit(GameManager gameManager)
    {
        gameManager.MainMenuLayout.SetActive(false);
    }
}
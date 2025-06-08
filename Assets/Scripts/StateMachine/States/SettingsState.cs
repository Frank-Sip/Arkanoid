using UnityEngine;

public class SettingsState : GameState
{    public override void Enter(GameManager gameManager)
    {
        gameManager.SettingsLayout.SetActive(true);
        var uiManager = ServiceProvider.GetService<UIManager>();
        var settingsManager = ServiceProvider.GetService<SettingsManager>();
        if (uiManager != null && settingsManager != null)
        {
            uiManager.UpdateVolumeDisplay("master", settingsManager.GetMasterVolume());
        }
    }public override void Tick(GameManager gameManager)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.stateMachine.ChangeState(new PauseState(), gameManager);
        }
    }

    public override void Exit(GameManager gameManager)
    {
        gameManager.SettingsLayout.SetActive(false);
    }
}

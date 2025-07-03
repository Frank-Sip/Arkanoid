using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager
{
    private ButtonSO config;
    private const float sound = 0.1f;
    
    public void Init(ButtonSO config)
    {
        this.config = config;
    }

    public void RegisterButton(string buttonName, Button button)
    {
        ConnectButton(buttonName, button);
    }

    private void ConnectButton(string buttonName, Button button)
    {
        var mapping = System.Array.Find(config.buttonMappings, m => m.buttonName == buttonName);
        if (mapping != null)
        {
            button.onClick.RemoveAllListeners();
            
            switch (mapping.actionType)
            {
                case ButtonSO.ButtonActionType.Play:
                    button.onClick.AddListener(OnPlayButtonClicked);
                    break;
                case ButtonSO.ButtonActionType.Resume:
                    button.onClick.AddListener(OnResumeButtonClicked);
                    break;
                case ButtonSO.ButtonActionType.MainMenu:
                    button.onClick.AddListener(OnMainMenuButtonClicked);
                    break;
                case ButtonSO.ButtonActionType.Quit:
                    button.onClick.AddListener(OnQuitButtonClicked);
                    break;
                case ButtonSO.ButtonActionType.VolumeUp:
                    button.onClick.AddListener(OnVolumeUpButtonClicked);
                    break;
                case ButtonSO.ButtonActionType.VolumeDown:
                    button.onClick.AddListener(OnVolumeDownButtonClicked);
                    break;
            }
        }
    }
    
    public void RegisterButtonsInLayout(GameObject layout)
    {
        if (layout == null) return;
        var buttons = layout.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            RegisterButton(button.name, button);
        }
    }
    
    public void OnVolumeUpButtonClicked()
    {
        var audioManager = ServiceProvider.GetService<AudioManager>();
        if (audioManager != null)
        {
            float currentVolume = audioManager.GetGlobalVolume();
            float newVolume = Mathf.Min(1.0f, currentVolume + sound);
            audioManager.SetGlobalVolume(newVolume);
        }
    }
    
    public void OnVolumeDownButtonClicked()
    {
        var audioManager = ServiceProvider.GetService<AudioManager>();
        if (audioManager != null)
        {
            float currentVolume = audioManager.GetGlobalVolume();
            float newVolume = Mathf.Max(0.0f, currentVolume - sound);
            audioManager.SetGlobalVolume(newVolume);
        }
    }

    public void OnPlayButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            ServiceProvider.GetService<LevelManager>().StartGame();
            GameManager.Instance.ChangeGameStatus(new GameplayState());
        }
    }

    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OnResumeButtonClicked()
    {
        var consoleManager = ServiceProvider.GetService<ConsoleManager>();
        if (consoleManager != null)
        {
            consoleManager.consoleUI.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameStatus(new GameplayState());
        }
    }

    public void OnMainMenuButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
            GameManager.Instance.ChangeGameStatus(new MainMenuState());
        }
    }
}
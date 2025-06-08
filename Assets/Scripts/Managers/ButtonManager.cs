using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager
{
    private Dictionary<string, Button> _registeredButtons = new Dictionary<string, Button>();
    private ButtonSO config;
    
    public void Init(ButtonSO config)
    {
        this.config = config;
    }

    public void RegisterButton(string buttonName, Button button)
    {
        if (!_registeredButtons.ContainsKey(buttonName))
        {
            _registeredButtons.Add(buttonName, button);
            ConnectButton(buttonName, button);
        }
    }    private void ConnectButton(string buttonName, Button button)
    {
        Debug.Log($"ButtonManager: Attempting to connect button '{buttonName}'");
        var mapping = System.Array.Find(config.buttonMappings, m => m.buttonName == buttonName);
        if (mapping != null)
        {
            Debug.Log($"ButtonManager: Found mapping for '{buttonName}' -> {mapping.actionType}");
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
                    break;                case ButtonSO.ButtonActionType.Quit:
                    button.onClick.AddListener(OnQuitButtonClicked);
                    break;
                case ButtonSO.ButtonActionType.Settings:
                    button.onClick.AddListener(OnSettingsButtonClicked);
                    break;
                case ButtonSO.ButtonActionType.IncreaseMasterVolume:
                    button.onClick.AddListener(OnIncreaseMasterVolumeClicked);
                    Debug.Log($"ButtonManager: Connected {buttonName} to OnIncreaseMasterVolumeClicked");
                    break;                case ButtonSO.ButtonActionType.DecreaseMasterVolume:
                    button.onClick.AddListener(OnDecreaseMasterVolumeClicked);
                    Debug.Log($"ButtonManager: Connected {buttonName} to OnDecreaseMasterVolumeClicked");
                    break;
                case ButtonSO.ButtonActionType.BackToMenu:
                    button.onClick.AddListener(OnBackToMenuClicked);
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"ButtonManager: No mapping found for button '{buttonName}'");
        }
    }
      public void RegisterButtonsInLayout(GameObject layout)
    {
        if (layout == null) 
        {
            Debug.LogWarning("ButtonManager: Layout is null, cannot register buttons");
            return;
        }

        Debug.Log($"ButtonManager: Registering buttons in layout '{layout.name}'");
        var buttons = layout.GetComponentsInChildren<Button>();
        Debug.Log($"ButtonManager: Found {buttons.Length} buttons in layout");
        
        foreach (var button in buttons)
        {
            Debug.Log($"ButtonManager: Registering button '{button.name}'");
            RegisterButton(button.name, button);
        }
    }

    public void OnPlayButtonClicked()
    {
        if (GameManager.Instance != null)
        {
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

    public void OnSettingsButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameStatus(new SettingsState());
        }
    }    public void OnIncreaseMasterVolumeClicked()
    {
        Debug.Log("ButtonManager: OnIncreaseMasterVolumeClicked called");
        var settingsManager = ServiceProvider.GetService<SettingsManager>();
        if (settingsManager != null)
        {
            Debug.Log("ButtonManager: SettingsManager found, calling IncreaseMasterVolume");
            settingsManager.IncreaseMasterVolume();
        }
        else
        {
            Debug.LogError("ButtonManager: SettingsManager is null!");
        }
    }

    public void OnDecreaseMasterVolumeClicked()
    {
        Debug.Log("ButtonManager: OnDecreaseMasterVolumeClicked called");
        var settingsManager = ServiceProvider.GetService<SettingsManager>();
        if (settingsManager != null)
        {
            Debug.Log("ButtonManager: SettingsManager found, calling DecreaseMasterVolume");
            settingsManager.DecreaseMasterVolume();
        }
        else
        {
            Debug.LogError("ButtonManager: SettingsManager is null!");
        }
    }

    public void OnBackToMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameStatus(new PauseState());
        }
    }
}
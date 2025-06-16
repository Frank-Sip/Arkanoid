using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager
{
    private ButtonSO config;
    
    public void Init(ButtonSO config)
    {
        this.config = config;
    }

    // Connect a button without using a dictionary
    public void RegisterButton(string buttonName, Button button)
    {
        ConnectButton(buttonName, button);
    }

    private void ConnectButton(string buttonName, Button button)
    {
        var mapping = System.Array.Find(config.buttonMappings, m => m.buttonName == buttonName);
        if (mapping != null)
        {
            Debug.Log($"Connecting button: {buttonName} with action: {mapping.actionType}");
            
            // Remove any existing listeners to prevent duplicates
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
            }
        }
    }
    
    public void RegisterButtonsInLayout(GameObject layout)
    {
        if (layout == null) return;

        Debug.Log($"Registering buttons in layout: {layout.name}");
        var buttons = layout.GetComponentsInChildren<Button>(true); // Include inactive buttons
        foreach (var button in buttons)
        {
            Debug.Log($"Found button: {button.name}");
            RegisterButton(button.name, button);
        }
    }

    // Button handler methods remain the same
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
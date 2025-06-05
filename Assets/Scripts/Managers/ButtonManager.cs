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
    }

    private void ConnectButton(string buttonName, Button button)
    {
        var mapping = System.Array.Find(config.buttonMappings, m => m.buttonName == buttonName);
        if (mapping != null)
        {
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

        var buttons = layout.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
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
}
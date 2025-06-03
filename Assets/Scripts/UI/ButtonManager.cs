using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager
{
    private Dictionary<string, Button> _registeredButtons = new Dictionary<string, Button>();

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
        switch (buttonName)
        {
            case "PlayButton":
                button.onClick.AddListener(OnPlayButtonClicked);
                break;
            case "ResumeButton":
                button.onClick.AddListener(OnResumeButtonClicked);
                break;
            case "MainMenuButton":
                button.onClick.AddListener(OnMainMenuButtonClicked);
                break;
            case "QuitButton":
                button.onClick.AddListener(OnQuitButtonClicked);
                break;
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
        GameManager.Instance.consoleManager.consoleUI.SetActive(false);
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
using UnityEngine;

public class ConsoleManager
{
    public GameObject consoleUI;
    private CommandInput commandInput;

    public void Init(GameObject console, CommandInput input)
    {
        consoleUI = console;
        commandInput = input;
    }

    public void Frame()
    {
        if (consoleUI.activeSelf)
        {
            commandInput.Frame();
        }
    }

    public void ToggleConsole()
    {
        bool isActive = consoleUI.activeSelf;
        consoleUI.SetActive(!isActive);
    }
}
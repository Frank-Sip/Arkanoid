using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    [SerializeField] private GameObject consoleUI;
    [SerializeField] private CommandInput commandInput;

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
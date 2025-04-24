using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    [SerializeField] private GameObject consoleUI;
    [SerializeField] private KeyCode toggleKey = KeyCode.Backslash;
    [SerializeField] private CommandInput commandInput;

    public void Frame()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleConsole();
        }

        if (consoleUI.activeSelf)
        {
            commandInput.Frame();
        }
    }

    private void ToggleConsole()
    {
        bool isActive = consoleUI.activeSelf;
        consoleUI.SetActive(!isActive);
    }
}
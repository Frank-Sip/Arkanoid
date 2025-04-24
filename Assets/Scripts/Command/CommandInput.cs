using UnityEngine;
using UnityEngine.UI;

public class CommandInput : MonoBehaviour
{
    public InputField inputField;
    public CommandManager commandManager;

    public void Frame()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmit();
        }
    }

    public void OnSubmit()
    {
        string input = inputField.text;
        Debug.Log("Input received: " + input);
        commandManager.ExecuteCommand(input);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }
}
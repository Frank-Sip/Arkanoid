using UnityEngine;
using TMPro;

public class CommandInput : MonoBehaviour
{
    public TMP_InputField inputField;
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
        commandManager.ExecuteCommand(input);
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }
}
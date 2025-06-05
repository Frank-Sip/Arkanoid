using UnityEngine;
using TMPro;

public class CommandInput
{
    private TMP_InputField inputField;
    private CommandManager commandManager;

    public void Init(TMP_InputField input)
    {
        inputField = input;
        commandManager = ServiceProvider.GetService<CommandManager>();
    }

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
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    public List<CommandSO> commands;
    private Dictionary<string, ICommand> commandDictionary;

    private void Awake()
    {
        commandDictionary = new Dictionary<string, ICommand>();

        foreach (var command in commands)
        {
            if (!string.IsNullOrWhiteSpace(command.commandName))
            {
                commandDictionary[command.commandName.ToLower()] = command;
            }
        }
    }

    public void ExecuteCommand(string input)
    {
        string commandInput = input.ToLower().Trim();

        if (commandDictionary.TryGetValue(commandInput, out ICommand command))
        {
            command.Execute();
        }
        else
        {
            Debug.Log("Command does not exist: " + input);
        }
    }
}
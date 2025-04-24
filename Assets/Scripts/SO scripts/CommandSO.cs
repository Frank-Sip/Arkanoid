using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCommand", menuName = "Commands/Command", order = 0)]
public class CommandSO : ScriptableObject, ICommand
{
    public string commandName;

    public virtual void Execute()
    {
        
    }
}

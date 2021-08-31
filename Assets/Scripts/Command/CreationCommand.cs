using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationCommand : ICommand
{
    public ICreator creator;

    public string name;
    public CreationCommand(ICreator creator, string name)
    {
        this.creator = creator;
        this.name = name;
    }
    /// <summary>
    /// Execute the creation command by its ICreator 
    /// </summary>
    public void Execute()
    {
        creator.Create(name);
    }
}

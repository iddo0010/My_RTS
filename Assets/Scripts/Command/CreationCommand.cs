using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationCommand : ICommand
{
    public ICreator creator;
    public CreationCommand(ICreator creator)
    {
        this.creator = creator;
    }
    /// <summary>
    /// Execute the creation command by its ICreator 
    /// </summary>
    public void Execute()
    {
        creator.Create();
    }
}

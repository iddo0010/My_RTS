using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICreator
{
    //ICreator interface for a building that can create objects
    Transform transform { get; }
    CommandHandler commandHandler { get; } 
    Queue<GameObject> GetIconQueue();
    void AddToQueue(GameObject icon);
    void Create();
    void Stop();
}
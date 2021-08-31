using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandHandler : MonoBehaviour
{
    private Queue<ICommand> commandbuffer;

    private void Awake()
    {
        commandbuffer = new Queue<ICommand>();
    }
    /// <summary>
    /// adds a command to the queue
    /// </summary>
    /// <param name="command"></param>
    public void AddCommand(ICommand command)
    {
        commandbuffer.Enqueue(command);
    }
    /// <summary>
    /// Executes the first command in queue
    /// </summary>
    public void ExecuteCommand()
    {
        if (commandbuffer.Count > 0)
        {
            ICommand c = commandbuffer.Dequeue();
            c.Execute();

        }
    }
    public void RemoveCommand(int index)
    {
        RemoveFromQueue.RemoveAt(commandbuffer, index);
    }
    void Update()
    {

    }
}
public static class RemoveFromQueue
{
    public static void RemoveAt<T>(this Queue<T> queue, int itemIndex)
    {
        var cycleAmount = queue.Count;

        for (int i = 0; i < cycleAmount; i++)
        {
            T item = queue.Dequeue();
            if (i == itemIndex)
                continue;

            queue.Enqueue(item);
        }
    }
    public static void RemoveSpecific(Queue<ICommand> queue, ICommand command)
    {
        var cycleAmount = queue.Count;

        for (int i = 0; i < cycleAmount; i++)
        {
            ICommand item = queue.Dequeue();
            if (item == command)
                continue;

            queue.Enqueue(item);
        }
    }    
    public static void RemoveSpecific(Queue<KeyValuePair<ICommand, GameObject>> queue, ICommand command)
    {
        var cycleAmount = queue.Count;

        for (int i = 0; i < cycleAmount; i++)
        {
            KeyValuePair<ICommand, GameObject> item = queue.Dequeue();
            if (item.Key == command)
                continue;

            queue.Enqueue(item);
        }
    }
}

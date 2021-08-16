using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockEngine : MonoBehaviour
{
    public ResourceType type;

    // Start is called before the first frame update
    void Awake()
    {

    }
    /// <summary>
    /// Visually Adds x  amount of Resources to the Stock Pile 
    /// </summary>
    /// <param name="amount">amount of resources</param>
    public void AddResource(int amount)
    {
        foreach (Transform child in transform) //enable resource spots acording to amount given
        {
            if (amount > 0)
            {
                if (!child.gameObject.activeInHierarchy) 
                {
                    child.gameObject.SetActive(true);
                    amount--;
                }
            }
            else
                break;
        }
    }
    /// <summary>
    /// Visually Removes X amount of resources from the stockpile
    /// </summary>
    /// <param name="amount">amount of resource to remove</param>
    /// <returns>leftover amount of resource to remove, 0 if none</returns>
    public int ReduceResource(int amount)
    {
        int lastResourceActive = -1;//index of the last resource spot active
        foreach (Transform child in transform) //Finds the index of the last resource spot active
        {
            if (!child.gameObject.activeInHierarchy)
                break;
            lastResourceActive++;
        }
        while(amount > 0)
        {
            if (lastResourceActive < 0) //return leftover amount 
            {
                return amount;
            }
            transform.GetChild(lastResourceActive).gameObject.SetActive(false);
            lastResourceActive--;
            amount--;
        }
        return 0; //no leftover
    }
}

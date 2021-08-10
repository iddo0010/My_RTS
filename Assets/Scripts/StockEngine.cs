using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockEngine : MonoBehaviour
{
    public ResourceType type;

    //List of the Resource spots available in the stock
    List<GameObject> stockSpots = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<transform.childCount - 1; i++)
        {
            stockSpots.Add(transform.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// Adds x  amount of Resources to the Stock Pile 
    /// </summary>
    /// <param name="amount">amount of resources</param>
    public void AddResource(int amount)
    {
        foreach (GameObject spot in stockSpots)
        {
            if (amount > 0)
            {
                if (!spot.activeInHierarchy)
                {
                    spot.SetActive(true);
                    amount--;
                }
            }
            else
                break;
        }
    }
}

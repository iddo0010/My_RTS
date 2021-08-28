using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockEngine : MonoBehaviour
{
    public ResourceType type;

    [SerializeField] int maxCapacity;
    [SerializeField] int currentQuantity;
    // Start is called before the first frame update
    void Awake()
    {
        switch (type)
        {
            case ResourceType.Tree:
                ResourceManager.instance.treeStocks.Add(gameObject);
                break;
            case ResourceType.Stone:
                ResourceManager.instance.stoneStocks.Add(gameObject);
                break;
        }
        ResourceManager.instance.UpdateUI();

        int lastResourceActive = -1;
        foreach (Transform child in transform) //Finds the index of the last resource spot active
        {
            if (lastResourceActive >= currentQuantity - 1)
                break;
            else
            {
                child.gameObject.SetActive(true);
                lastResourceActive++;
            }
        }
    }
    private void Start()
    {
        //Only when game starts, adds the existing quantity in each stock to the resource manager(for development purpeses)
        ResourceManager.instance.resources[type] += currentQuantity;
        ResourceManager.instance.UpdateUI();
    }
    /// <summary>
    /// Visually Adds x  amount of Resources to the Stock Pile 
    /// </summary>
    /// <param name="amount">amount of resources</param>
    public void AddResource(int amount)
    {
        currentQuantity += amount;
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
        int lastResourceActive = currentQuantity - 1;//index of the last resource spot active
        while(amount > 0)
        {
            if (lastResourceActive < 0) //return leftover amount 
            {
                return amount;
            }
            transform.GetChild(lastResourceActive).gameObject.SetActive(false);
            currentQuantity--;
            lastResourceActive--;
            amount--;
        }
        return 0; //no leftover
    }

    public int StockSpaceAvailavle()
    {
        return maxCapacity - currentQuantity;
    }
}

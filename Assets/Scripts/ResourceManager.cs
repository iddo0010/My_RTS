using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ResourceManager : MonoBehaviour
{
    //Singelton
    public static ResourceManager instance;

    //UI
    [SerializeField] TextMeshProUGUI woodText;
    [SerializeField] TextMeshProUGUI stoneText;
    [SerializeField] TextMeshProUGUI foodText;

    //Resource Manager
    public Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    public List<GameObject> treeStocks = new List<GameObject>();
    public List<GameObject> stoneStocks = new List<GameObject>();
    public List<GameObject> foodStocks = new List<GameObject>();

    //Max Stock Available
    public int maxTree;
    public int maxStone;
    public int maxFood;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        resources.Add(ResourceType.Tree, 126);
        resources.Add(ResourceType.Stone, 100);
        resources.Add(ResourceType.Food, 0);
        resources.Add(ResourceType.Gold, 0);
        UpdateUI();
    }

    /// <summary>
    /// Unload Specific Resource from a unit into the resource Manager
    /// </summary>
    /// <param name="unit">unit to unload from</param>
    /// <param name="type">type of the resource</param>
    /// <param name="stockPile">stockPile to unload to</param>
    public void Unload(Unit unit, StockEngine stockPile)
    {
        int amountToUnload = 0;
        int stockSpaceAvailable = stockPile.StockSpaceAvailavle();
        if (unit.resourceCarry[stockPile.type] > stockSpaceAvailable)
            amountToUnload = stockSpaceAvailable;
        else
            amountToUnload = unit.resourceCarry[stockPile.type];
        resources[stockPile.type] += amountToUnload;
        unit.resourceCarry[stockPile.type] -= amountToUnload;
        stockPile.AddResource(amountToUnload);
        UpdateUI();
    }
    /// <summary>
    /// Updates the Resources count in the UI
    /// </summary>
    public void UpdateUI()
    {
        maxTree = treeStocks.Count * 128;
        maxStone = stoneStocks.Count * 125;
        //maxFood = 0;
        woodText.text = resources[ResourceType.Tree] + " / " + maxTree.ToString();
        stoneText.text = resources[ResourceType.Stone] + " / " + maxStone.ToString();
        //foodText.text = resources[ResourceType.food] + " / " + foodStock.ToString();
    }

    /// <summary>
    /// Checks if there are enugh resorces to build
    /// </summary>
    /// <param name="treePrice">wood cost</param>
    /// <param name="stonePrice">stone cost</param>
    /// <param name="goldPrice">gold cost</param>
    /// <returns>true for enugh resouces</returns>
    public bool CanBuild(int treePrice, int stonePrice ,int goldPrice)
    {
        if (resources[ResourceType.Tree] >= treePrice && resources[ResourceType.Stone] >= stonePrice && resources[ResourceType.Gold] >= goldPrice)
            return true;
        else
            return false;
    }
    /// <summary>
    /// Arange in a list the stockpiles by range
    /// </summary>
    /// <param name="pos">position to search from</param>
    /// <param name="type">type of resource pile</param>
    /// <returns>List of all StockPiles arranged by distance to the spot given</returns>
    public List<GameObject> FindNearesStockPile(Vector3 pos ,ResourceType type)
    {
        List<GameObject> currentStockList = new List<GameObject>();
        switch (type) //defines which stock list to arrange by type of the resource
        {
            case ResourceType.Tree:
                foreach(GameObject g in treeStocks)
                {
                    currentStockList.Add(g);
                }
                break;
            case ResourceType.Stone:
                foreach (GameObject g in stoneStocks)
                {
                    currentStockList.Add(g);
                }
                break;
        }
        int maxIndex = currentStockList.Count; // amount of stockpiles in game
        List <GameObject> nearestStockList = new List<GameObject>(); 
        for (int i = 0; i < maxIndex; i++) //search x times by amount of stockpiles
        {
            int index = 0;
            GameObject nearestStock = currentStockList[index]; //selects a first spot to check distance
            while(nearestStock == null)//if the stock has already been added to the nearest stock list, gets the next stock
            {
                index++;
                nearestStock = currentStockList[index];
            }
            float bestDistance = Vector3.Distance(pos, nearestStock.transform.position);//distance to the first spot selected
            foreach (GameObject r in currentStockList) // run on all spots in the list, and find the nearest one
            {
                if (r != null)//check if the stock has already been added to the nearest stock list
                {
                    float distance = Vector3.Distance(pos, r.transform.position);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        nearestStock = r;
                    }
                }
            }
            nearestStockList.Add(nearestStock); //adds nearest stock to list
            currentStockList.Remove(nearestStock);//removes nearest stock from the temp list
        }

        return nearestStockList;
    }
    /// <summary>
    /// Reduce amount of resources from the resource manager
    /// </summary>
    /// <param name="treePrice">wood amount</param>
    /// <param name="stonePrice">stone amount</param>
    /// <param name="position">position of the building being build</param>
    public void ReduceAmount(int treePrice, int stonePrice, int goldPrice, Vector3 position)
    {
        if (treePrice > 0)
            ReduceWood(treePrice, position);
        if (stonePrice > 0)
            ReduceStone(stonePrice, position);
        if (goldPrice > 0)
            resources[ResourceType.Gold] -= goldPrice;
        UpdateUI();
    }
    /// <summary>
    /// Reduces amount of wood from the resource manager
    /// </summary>
    /// <param name="treePrice">amount of wood</param>
    /// <param name="pos">pos of the building being build</param>
    public void ReduceWood(int treePrice, Vector3 pos)
    {
        resources[ResourceType.Tree] -= treePrice;
        List<GameObject> nearestStockList = FindNearesStockPile(pos, ResourceType.Tree);
        for(int i=0; i<nearestStockList.Count;i++) //Visually removes the amount given, from the nearest stockpile onwards
        {
            treePrice = nearestStockList[i].GetComponent<StockEngine>().ReduceResource(treePrice);
            if (treePrice == 0)
                break;
        }
    }
    /// <summary>
    /// Reduces amount of stone from the resource manager
    /// </summary>
    /// <param name="stonePrice">amount of stone</param>
    /// <param name="pos">pos of the building being build</param>
    public void ReduceStone(int stonePrice, Vector3 pos)
    {
        resources[ResourceType.Stone] -= stonePrice;
        List<GameObject> nearestStockList = FindNearesStockPile(pos, ResourceType.Stone);
        for (int i = 0; i < nearestStockList.Count; i++)//Visually removes the amount given, from the nearest stockpile onwards
        {
            stonePrice = nearestStockList[i].GetComponent<StockEngine>().ReduceResource(stonePrice);
            if (stonePrice == 0)
                break;
        }
    }

}

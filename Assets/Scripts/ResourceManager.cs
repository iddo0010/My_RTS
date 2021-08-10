using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    //Singelton
    public static ResourceManager instance;

    //UI
    [SerializeField] TextMeshProUGUI woodText;
    [SerializeField] TextMeshProUGUI stoneText;
    [SerializeField] TextMeshProUGUI foodText;

    //Resource Manager
    Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    public List<GameObject> treeStocks = new List<GameObject>();
    public List<GameObject> stoneStocks = new List<GameObject>();
    public List<GameObject> foodStocks = new List<GameObject>();

    //Max Stock Available
    public int treeStock;
    public int stoneStock;
    public int foodStock;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        treeStocks.Add(GameObject.Find("Stockpile_Wood"));
        stoneStocks.Add(GameObject.Find("Stockpile_Stone"));
        treeStock = treeStocks.Count * 128;
        stoneStock = stoneStocks.Count * 125;
        foodStock = 0;
        resources.Add(ResourceType.Tree, 0);
        resources.Add(ResourceType.Stone, 0);
        resources.Add(ResourceType.Food, 0);
        UpdateUI();
    }




    /// <summary>
    /// Unload Specific Resource from a unit into the resource Manager
    /// </summary>
    /// <param name="unit">unit to unload from</param>
    /// <param name="type">type of the resource</param>
    /// <param name="stockPile">stockPile to unload to</param>
    public void Unload(Unit unit, ResourceType type, GameObject stockPile)
    {
        int amountToUnload = 0;
        int stockToUnload = 0;
        switch (type)
        {
            case ResourceType.Tree:
                stockToUnload = treeStock;
                break;
            case ResourceType.Stone:
                stockToUnload = stoneStock;
                break;
            case ResourceType.Food:
                stockToUnload = foodStock;
                break;
        }
        int stockSpaceAvailable = stockToUnload - resources[type];
        if (unit.resourceCarry[type] > stockSpaceAvailable)
            amountToUnload = stockSpaceAvailable;
        else
            amountToUnload = unit.resourceCarry[type];
        resources[type] += amountToUnload;
        unit.resourceCarry[type] -= amountToUnload;
        stockPile.GetComponent<StockEngine>().AddResource(amountToUnload);
        UpdateUI();
    }
    /// <summary>
    /// Updates the Resources count in the UI
    /// </summary>
    public void UpdateUI()
    {
        woodText.text = resources[ResourceType.Tree] + " / " + treeStock.ToString();
        stoneText.text = resources[ResourceType.Stone] + " / " + stoneStock.ToString();
        //foodText.text = resources[ResourceType.food] + " / " + foodStock.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitAllowance : MonoBehaviour
{
    //Singelton
    public static UnitAllowance instance;

    [SerializeField] TextMeshProUGUI unitsCapacityText;

    public int maxUnitCapacity;
    public int currentUnitAmount;
    public int unitsInProcess;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        maxUnitCapacity = 0;
        unitsInProcess = 0;
        currentUnitAmount = SelectionManager.instance.unitList.Count;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanBuildUnits()
    {
        if (currentUnitAmount + unitsInProcess < maxUnitCapacity)
            return true;
        return false;
    }
    public void AddUnitCapacity(int amount)
    {
        maxUnitCapacity += amount;
        UpdateUI();
    }
    /// <summary>
    /// Adds the given Unit to the game unitlist
    /// </summary>
    /// <param name="unit"></param>
    public void CreateNewUnit(GameObject unit)
    {
        unitsInProcess--;
        currentUnitAmount++;
        SelectionManager.instance.unitList.Add(unit);
        UpdateUI();
    }
    private void UpdateUI()
    {
        unitsCapacityText.text = currentUnitAmount.ToString() + " / " + maxUnitCapacity.ToString();
    }
}

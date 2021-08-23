using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAllowance : MonoBehaviour
{
    //Singelton
    public static UnitAllowance instance;

    public int maxUnitCapacity;
    public int currentUnitAmount;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        maxUnitCapacity = 4; //starts the game with 3 units.
        currentUnitAmount = SelectionManager.instance.unitList.Count;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanBuildUnits()
    {
        if (currentUnitAmount < maxUnitCapacity)
            return true;
        return false;
    }
    public void AddUnitCapacity(int amount)
    {
        maxUnitCapacity += amount;
    }

    public void CreateNewUnit(GameObject unit)
    {
        SelectionManager.instance.unitList.Add(unit);
        currentUnitAmount++;
    }
}

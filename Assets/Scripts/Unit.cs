using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit 
{
    public bool isSelected;
    public bool isMoving;
    public bool isGatheringRoutine;
    public bool isHarvesting;
    public bool isBuilding;
    public int bagSize;
    public Dictionary<ResourceType, int> resourceCarry;
    public float hp;
    public float dmg;
    public  Unit()
    {
        isSelected = false;
        isMoving = false;
        isGatheringRoutine = false;
        dmg = 0.1f;
        hp = 1f;
        bagSize = 3;
        resourceCarry = new Dictionary<ResourceType, int>();
        resourceCarry.Add(ResourceType.Tree, 0);
        resourceCarry.Add(ResourceType.Stone, 0);
        resourceCarry.Add(ResourceType.Food, 0);
    }

    private int GetCurrentAmountCarry()
    {
        int amount = 0;
        foreach(KeyValuePair<ResourceType,int> pair in resourceCarry)
        {
            amount += pair.Value;
        }
        return amount;
    }

    public bool IsBagFull()
    {
        if (GetCurrentAmountCarry() < bagSize)
            return false;
        return true;
    }

}

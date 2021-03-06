using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Building")]
public class BuildingSettings : ScriptableObject
{
    public string name;
    public Transform[] prefab;
    public Transform bluePrint;
    public Transform construction;
    public Transform visual;
    public int woodCost;
    public int stoneCost;
    public int goldCost;
    public int constructionDelay;
    public int countructionAmount;
    public Transform[] upgradeList;

}

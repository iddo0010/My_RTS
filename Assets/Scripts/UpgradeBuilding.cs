using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBuilding : MonoBehaviour
{
    public BuildingSettings building;

    [SerializeField] int buildingLvl;

    [SerializeField] int woodCost;
    [SerializeField] int stoneCost;
    [SerializeField] int goldCost;
    // Start is called before the first frame update
    void Awake()
    {
        //When Selecting a Building, check if the building has UpgadeBuilding Component,
        //if so Activate upgrade button in the Building Action panel and add a listener to the button with UpgradeBuilding() method
    }
    /// <summary>
    /// Adding a Construction Script to the object, making it upgradable by units
    /// </summary>
    public void UpgradeButton()
    {
        if (buildingLvl < building.upgradeList.Length && ResourceManager.instance.CanBuild(woodCost, stoneCost, goldCost))
        {
            ResourceManager.instance.ReduceAmount(woodCost, stoneCost, goldCost, transform.position);
            ConstructBuilding constructBuilding = gameObject.AddComponent<ConstructBuilding>();
            constructBuilding.building = this.building;
            constructBuilding.prefab = building.upgradeList[buildingLvl].gameObject;
            gameObject.tag = "Construction";
        }
    }

}

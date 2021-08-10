using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum ResourceType
{
    Tree, Stone, Food,
}
public class Resource : MonoBehaviour
{
    [SerializeField] GameObject[] treeTrunk;

    [SerializeField] int quantity;
    [SerializeField] float delay;

    public ResourceType type;

    List<UnitEngine> unitsGathering = new List<UnitEngine>();

    public void Update()
    {
        if (quantity <= 0)
        {
            StopAllCoroutines();
            foreach (UnitEngine u in unitsGathering)
            {
                u.resourceBeingGathered = null;
                u.unit.isHarvesting = false;
                u.SetHarvestAnimation(false);
            }
            Destroy(gameObject);
            if (type.Equals(ResourceType.Tree))
                Instantiate(treeTrunk[Random.Range(0, 1)], transform.position, transform.rotation);
        }
    }
    /// <summary>
    /// Harvest a Specific Resource
    /// </summary>
    /// <param name="unitEngine"></param>
    public void Gather(UnitEngine unitEngine)
    {
        //TODO - Reduce Movement speed while carry resource
        if (!unitEngine.unit.IsBagFull())
        {
            unitsGathering.Add(unitEngine);
            switch (type)
            {
                case ResourceType.Tree:
                    StartCoroutine(HarvestWood(unitEngine));
                    break;
                case ResourceType.Stone:
                    StartCoroutine(HarvestStone(unitEngine));
                    break;
            }
        }
    }


    /// <summary>
    /// Collects Wood by Ticks, sends back to stockpile if unit bag is full, sends looking for more resources if the resource has been destoryed
    /// </summary>
    /// <param name="unitEngine">Unit Harvesting</param>
    /// <returns></returns>
    public IEnumerator HarvestWood(UnitEngine unitEngine)
    {
        if(unitEngine.mainWeapon.type == weaponType.HarvestingAxe)
        { 
        unitEngine.unit.isHarvesting = true;
        unitEngine.SetHarvestAnimation(true);
            for (int i = 0; i <= (delay * 5); i++)
            {
                yield return new WaitForSeconds(delay);
                if (unitEngine.unit.isGatheringRoutine)
                {
                    Debug.Log("Tick");
                    quantity--;
                    unitEngine.unit.resourceCarry[type]++;
                    if (unitEngine.unit.IsBagFull())
                    {
                        unitEngine.unit.isHarvesting = false;
                        unitEngine.SetHarvestAnimation(false);
                        unitEngine.SendToStockPile(type);
                        unitsGathering.Remove(unitEngine);
                        break;
                    }
                }
                else
                    break;
            }
        }
    }
    /// <summary>
    /// Collects Stone by Ticks, sends back to stockpile if unit bag is full
    /// </summary>
    /// <param name="unitEngine">Unit Harvesting</param>
    public IEnumerator HarvestStone(UnitEngine unitEngine)
    {
        if (unitEngine.mainWeapon.type == weaponType.MiningHoe)
        {
            unitEngine.DeSelectUnit();
            unitEngine.unit.isHarvesting = true;
            unitEngine.ActivateUnit(false);
            for (int i = 0; i <= (delay * 5); i++)
            {
                yield return new WaitForSeconds(delay);
                Debug.Log("Tick");
                quantity--;
                unitEngine.unit.resourceCarry[type]++;
                if (unitEngine.unit.IsBagFull())
                {
                    unitEngine.unit.isHarvesting = false;
                    unitEngine.ActivateUnit(true);
                    unitEngine.SendToStockPile(type);
                    unitsGathering.Remove(unitEngine);
                    break;
                }
            }
        }
    }



}


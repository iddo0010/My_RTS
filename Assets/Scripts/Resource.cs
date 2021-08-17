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


    public Dictionary<Collider, Coroutine> colliders;
    Coroutine routine;

    public void Awake()
    {
        colliders = new Dictionary<Collider, Coroutine>();
        routine = null;
    }

    public void Update()
    {
        if (quantity <= 0)
        {
            StopAllCoroutines();
            foreach (KeyValuePair<Collider,Coroutine> pair in colliders)
            {
                UnitEngine unit = pair.Key.gameObject.GetComponent<UnitEngine>();
                unit.resourceBeingGathered = null;
                unit.unit.isHarvesting = false;
                unit.SetHarvestAnimation(false);
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
            switch (type)
            {
                case ResourceType.Tree:
                    routine = StartCoroutine(HarvestWood(unitEngine));
                    colliders.Add(unitEngine.GetComponent<CapsuleCollider>(), routine);
                    break;
                case ResourceType.Stone:
                    routine = StartCoroutine(HarvestStone(unitEngine));
                    colliders.Add(unitEngine.GetComponent<CapsuleCollider>(), routine);
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
        if (unitEngine.mainWeapon.type == weaponType.HarvestingAxe)
        {
            unitEngine.unit.isHarvesting = true;
            unitEngine.SetHarvestAnimation(true);
            for (int i = 0; i <= (delay * 5); i++)
            {
                yield return new WaitForSeconds(delay);
                Debug.Log("Tick");
                quantity--;
                unitEngine.unit.resourceCarry[type]++;
                if (unitEngine.unit.IsBagFull()) // if the units bag is full, break
                {
                    StopGathering(unitEngine);
                    unitEngine.SendToStockPile(type);
                }
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
                if (unitEngine.unit.IsBagFull())// if the units bag is full, break
                {
                    StopGathering(unitEngine);
                    unitEngine.SendToStockPile(type);
                }
            }
        }
    }

    /// <summary>
    /// Stops Single unit from gathering and disables its Coroutine
    /// </summary>
    /// <param name="unitEngine">unit</param>
    public void StopGathering(UnitEngine unitEngine)
    {
        switch (type)
        {
            case ResourceType.Tree:
                unitEngine.SetHarvestAnimation(false);
                break;
            case ResourceType.Stone:
                unitEngine.ActivateUnit(true);
                break;
        }
        StopCoroutine(colliders[unitEngine.GetComponent<CapsuleCollider>()]);
        unitEngine.unit.isHarvesting = false;
        colliders.Remove(unitEngine.GetComponent<CapsuleCollider>());
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructBuilding : MonoBehaviour
{
    public BuildingSettings building;
    public GameObject prefab;

    public Dictionary<Collider, Coroutine> colliders;// Dictionary with all the units currently working on this building, and their Corotuines
    Coroutine routine;

    int constructionAmount; 

    private void Awake()
    {
        constructionAmount = 100;
        colliders = new Dictionary<Collider, Coroutine>();
        routine = null;
    }
    private void Update()
    {
        if(constructionAmount <= 0)
        {
            Instantiate(prefab, transform.position, transform.rotation);
            StopAllCoroutines();
            foreach (KeyValuePair<Collider, Coroutine> pair in colliders) ///Disables all units currently building
            {
                UnitEngine unit = pair.Key.gameObject.GetComponent<UnitEngine>();
                unit.unit.isBuilding = false;
                unit.SetAnimation("isBuilding", false);
            }
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Starts the Build Coroutine
    /// </summary>
    /// <param name="unitEngine">unit that builds</param>
    public void StartBuildProcess(UnitEngine unitEngine)
    {
        routine = StartCoroutine(Build(unitEngine));
        colliders.Add(unitEngine.GetComponent<CapsuleCollider>(), routine);
    }
    /// <summary>
    /// Construct Building Over Time 
    /// </summary>
    /// <param name="unitEngine">unit</param>
    /// <returns></returns>
    private IEnumerator Build(UnitEngine unitEngine)
    {
        unitEngine.unit.isBuilding = true;
        unitEngine.SetAnimation("isBuilding" , true);
        for (int i = 0; i <= (100 / building.countructionAmount); i++) //
        {
            yield return new WaitForSeconds(building.constructionDelay);
            Debug.Log("Tick");
            constructionAmount -= building.countructionAmount;
        }

    }
    /// <summary>
    /// Stops Single unit from building and disables its Coroutine
    /// </summary>
    /// <param name="unitEngine">unit</param>
    public void StopBuilding(UnitEngine unitEngine)
    {
        unitEngine.SetAnimation("isBuilding", false);
        StopCoroutine(colliders[unitEngine.GetComponent<CapsuleCollider>()]);//stops a specific coroutine according to unit
        unitEngine.unit.isBuilding = false;
        colliders.Remove(unitEngine.GetComponent<CapsuleCollider>());//removes unit from the dictionary
    }
}

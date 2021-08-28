using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitCreation : MonoBehaviour
{
    [SerializeField] GameObject unit;
    [SerializeField] GameObject spawnCommand;

    Vector3 originalSpawnPoint;
    Vector3 spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        originalSpawnPoint = transform.Find("SpawnPoint").position;
        spawnPoint = originalSpawnPoint;
    }

    public void CreateUnit()
    {
        if (UnitAllowance.instance.CanBuildUnits())
        {
            UnitCreation currentCamp = SelectionManager.instance.selectedBuilding.GetComponent<UnitCreation>();
            GameObject newUnit = Instantiate(unit, currentCamp.originalSpawnPoint, Quaternion.identity);
            newUnit.GetComponent<NavMeshAgent>().SetDestination(currentCamp.spawnPoint);
            UnitAllowance.instance.CreateNewUnit(newUnit);
        }
    }

    public void SpawnPointButton()
    {
        SelectionManager.instance.isSetTargetMode = true;
    }
    public void ChangeSpawnPoint(Vector3 mousePos)
    {

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, 1 << 10))
        {
            UnitCreation currentCamp = SelectionManager.instance.selectedBuilding.GetComponent<UnitCreation>();
            Instantiate(spawnCommand, new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z), Quaternion.identity);
            currentCamp.spawnPoint = hit.point;
        }
    }
}

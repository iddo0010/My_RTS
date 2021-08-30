using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class UnitCreation : MonoBehaviour, ICreator
{
    [SerializeField] GameObject unit;
    [SerializeField] GameObject spawnCommand;

    Vector3 originalSpawnPoint;
    Vector3 spawnPoint;

    [SerializeField] int unitCreationDelay;
    [SerializeField] int unitCreationSteps;

    Transform multipleUnitContent;
    [SerializeField] GameObject unitIcon;

    Queue<GameObject> unitIcons;
    public CommandHandler commandHandler { get { return GetComponent<CommandHandler>(); } }

    public bool canBuild; // check if the building is during a creation coroutine
    void Awake()
    {
        canBuild = true;
        originalSpawnPoint = transform.Find("SpawnPoint").position;
        spawnPoint = originalSpawnPoint;
        unitIcons = new Queue<GameObject>();
        multipleUnitContent = GameObject.Find("MultipleUnits").transform.Find("UnitList/Viewport/Content");
    }
    void Update()
    {
        if (canBuild && UnitAllowance.instance.CanBuildUnits())
            commandHandler.ExecuteCommand(); // execute a command from the building command handler
    }
    /// <summary>
    /// Adds a UI icon to the queue
    /// </summary>
    /// <param name="icon"></param>
    public void AddToQueue(GameObject icon)
    {
        int iconIndex = unitIcons.Count;
        GameObject newIcon = Instantiate(icon, multipleUnitContent); //instantiates the icon in the info panel
        newIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate { UIManager.instance.UnQueueUnit(iconIndex); });
        unitIcons.Enqueue(icon);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>returns this building UI icon Queue</returns>
    public Queue<GameObject> GetIconQueue()
    {
        return unitIcons;
    }   
    /// <summary>
    /// Starts the unit creation coroutine
    /// </summary>
    public void Create()
    {
        UnitAllowance.instance.unitsInProcess++;
        StartCoroutine(StartCreation(unitCreationDelay, unitCreationSteps));
        canBuild = false;
    }
    public void Stop()
    {
        UnitAllowance.instance.unitsInProcess--;
        StopAllCoroutines();
        canBuild = true;
    }
    /// <summary>
    /// Create a unit during a given amount of time
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="steps"></param>
    /// <returns></returns>
    private IEnumerator StartCreation(int delay, int steps)
    {
        for (int i = 0; i < steps; i++) 
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Tick");
        }
        unitIcons.Dequeue(); // removes icon from queue
        GameObject newUnit = Instantiate(unit, originalSpawnPoint, Quaternion.identity);
        newUnit.GetComponent<NavMeshAgent>().SetDestination(spawnPoint); 
        UnitAllowance.instance.CreateNewUnit(newUnit); //adds the new unit to the game unitlist
        canBuild = true; 
        if(SelectionManager.instance.selectedBuilding == gameObject) // if the building is currently selected
            Destroy(multipleUnitContent.transform.GetChild(0).gameObject); //removes icon from the info panel
    }
    /// <summary>
    /// Changes the spawn point of the building by given mouse position
    /// </summary>
    /// <param name="mousePos"></param>
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

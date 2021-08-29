using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitEngine : MonoBehaviour
{
    [SerializeField] GameObject moveCommand;
    [SerializeField] GameObject resourceCommand;
    [SerializeField] GameObject attackCommand;


    public Unit unit;
    //WeaponManagement weaponManager;

    Animator anm;
    NavMeshAgent agent;

    List<GameObject> unitApperance = new List<GameObject>();
    bool isUnitVisable;

    GameObject unitSelectionCircle;

    public GameObject resourceBeingGathered;
    ResourceType lastResource;

    [SerializeField] float searchRadius;
    [SerializeField] LayerMask resourceLayer;

    public Weapon mainWeapon;
    public Weapon offWeapon;

    public GameObject targetToFind;
    public ToolsProduction currentWorkshop;
    // Start is called before the first frame update
    void Awake()
    {
        unit = new Unit();
        mainWeapon = new Weapon();
        offWeapon = new Weapon();
        //weaponManager = GetComponent<WeaponManagement>();
        //weaponManager.CheckIfWeaponEquipped();
        agent = GetComponent<NavMeshAgent>();
        unitSelectionCircle = transform.Find("SelectionRing").gameObject;
        anm = GetComponent<Animator>();
        unitSelectionCircle.SetActive(false);
        resourceBeingGathered = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject temp = transform.GetChild(i).gameObject;
            if (temp.activeInHierarchy)
            {
                unitApperance.Add(temp);
            }
        }
        isUnitVisable = true;
        targetToFind = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isUnitVisable)
        {
            CheckWalkingAnimation();
            if (unit.isSelected)
                CheckForUserInput();
            if (unit.isGatheringRoutine)
            {
                if (!unit.IsBagFull())
                {
                    if (resourceBeingGathered == null)// Go Looking for new resource if the unit is currently in gathering routine and the resource has been destroyed
                        SendToHarvest(lastResource);
                }
            }
            if (targetToFind != null)
                FindTarget(targetToFind);
        }
    }

    /// <summary>
    /// Activate/Deactivate Walking animation
    /// </summary>
    private void CheckWalkingAnimation()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            unit.isMoving = false;
        else
            unit.isMoving = true;
        anm.SetBool("isWalking", unit.isMoving);
    }

    /// <summary>
    /// Activate/Deactivate specific Animation
    /// </summary>
    /// <param name="animation">name</param>
    /// <param name="enable">true for enable</param>
    public void SetAnimation(string animation, bool enable)
    {
        anm.SetBool(animation, enable);
    }
    /// <summary>
    /// Check For User Command Over Unit, Move Unit Accordingly
    /// </summary>
    private void CheckForUserInput()
    {
        if (!SelectionManager.instance.isSetTargetMode && !UIManager.instance.isBluePrintEnabled)
        {
            if (Input.GetMouseButtonDown(1))
            {
                MoveUnit(Input.mousePosition);
            }
        }
        if (Input.GetKey(KeyCode.Space))
            FindObjectOfType<CameraMovement>().FollowUnit();
    }
    /// <summary>
    /// Move Unit Towards Clicked location, if clicked on game object act accordingly
    /// </summary>
    public void MoveUnit(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            switch (hit.transform.gameObject.layer)
            {
                case 9: //Resource Layer
                    Instantiate(resourceCommand, hit.transform.position, resourceCommand.transform.rotation);
                    GoToTarget(hit.transform.gameObject);
                    break;
                case 10://Ground Layer
                    CancelCurrentAction();
                    Instantiate(moveCommand, new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z), resourceCommand.transform.rotation);
                    agent.SetDestination(hit.point);
                    break;
                case 11://Building Layer
                    print(hit.transform.name);
                    GoToTarget(hit.transform.gameObject);
                    break;
            }

        }

    }

    /// <summary>
    /// Visualy and Logicly Select the unit
    /// </summary>
    public void SelectUnit()
    {
        unit.isSelected = true;
        unitSelectionCircle.SetActive(true);
    }
    /// <summary>
    /// Visualy and Logicly DeSelect the unit
    /// </summary>
    public void DeSelectUnit()
    {
        unit.isSelected = false;
        unitSelectionCircle.SetActive(false);
    }
    
    /// <summary>
    /// Automaticly Sends a unit to a stockpile by type
    /// </summary>
    /// <param name="type">type of the recource</param>
    public void SendToStockPile(ResourceType type)
    {
        List <GameObject> stockTargets = ResourceManager.instance.FindNearesStockPile(transform.position, type);
        GameObject target = stockTargets[0];
        foreach(GameObject stock in stockTargets)
        {
            if (stock.GetComponent<StockEngine>().StockSpaceAvailavle() > 0)
            {
                target = stock;
                break;
            }
        }
        GoToTarget(target);
    }
    /// <summary>
    /// Send Unit to Harvset the corrent resource, if there is none, finds the nearest target by Resource Type
    /// </summary>
    /// <param name="type">Type of resource to find</param>
    public void SendToHarvest(ResourceType type)
    {
        //Find Nearby Resource of the type
        if (resourceBeingGathered != null)
        {
            GoToTarget(resourceBeingGathered);
        }
        else
        {
            if (type == ResourceType.Tree)
            {
                GameObject[] resources = GameObject.FindGameObjectsWithTag(type.ToString());
                GameObject nearestResource = resources[1];
                float bestDistance = Vector3.Distance(transform.position, nearestResource.transform.position);
                foreach (GameObject r in resources)
                {
                    float distance = Vector3.Distance(transform.position, r.transform.position);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        nearestResource = r;
                    }
                }
                GoToTarget(nearestResource);
            }
        }
    }
    /// <summary>
    /// Move towards the target attached, change parameters according to targets layer
    /// </summary>
    /// <param name="target">target</param>
    public void GoToTarget(GameObject target)
    {
        CancelCurrentAction();
        targetToFind = target;
        switch (target.layer)
        {
            case 9: //Resource Layer
                if (!unit.IsBagFull())
                {
                    resourceBeingGathered = target;
                    unit.isGatheringRoutine = true;
                    lastResource = target.GetComponent<Resource>().type;
                }
                break;
            case 11://Building Layer
                switch(target.tag)
                {
                    case "StockPile":
                        targetToFind = target.transform.Find("UnloadSpot").gameObject;
                        break;
                    case "Construction":
                        if(target.transform.Find("BuildingPoint") != null) //Checks if the construction is a new building or a upgrade
                            targetToFind = target.transform.Find("BuildingPoint").gameObject;
                        break;
                }
                break;
        }
        agent.SetDestination(targetToFind.transform.position);
    }

    /// <summary>
    /// Creates a Overlap Sphere with searchRadius, if the target is withing the sphere - executes methods according to targets layer
    /// </summary>
    /// <param name="target">Target to find</param>
    private void FindTarget(GameObject target)
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (Collider c in collidersInRange)
        {
            if (c.gameObject == target)
            {
                agent.ResetPath();
                targetToFind = null;
                
                switch (target.layer)
                {
                    case 9: //Resource Layer
                        target.GetComponent<Resource>().Gather(this);
                        break;
                    case 11: //Building Layer
                        switch (target.tag)
                        {
                            case "StockPile":
                                GameObject stockPile = target.transform.parent.gameObject;
                                ResourceType stockType = stockPile.GetComponent<StockEngine>().type;
                                ResourceManager.instance.Unload(unit, stockPile.GetComponent<StockEngine>());
                                if (unit.resourceCarry[stockType] == 3)
                                    SendToStockPile(stockType);
                                else
                                    SendToHarvest(stockType);
                                break;
                            case "Construction":
                                if (mainWeapon.canBuild)
                                {
                                    ConstructBuilding construct;
                                    if (target.TryGetComponent<ConstructBuilding>(out construct)) // if the construct is an upgrade
                                        construct.StartBuildProcess(this);
                                    else
                                        target.GetComponentInParent<ConstructBuilding>().StartBuildProcess(this);
                                }
                                break;
                            case "Workshop":
                                {
                                    currentWorkshop = target.GetComponent<ToolsProduction>();
                                    currentWorkshop.ChooseTool(this);
                                }
                                break;
                        }
                        break;
                }
                break;

            }
        }
    }

    /// <summary>
    /// Activate/DeActivate unit appearance and components, except of UnitEngine
    /// </summary>
    /// <param name="isActive"></param>
    public void ActivateUnit(bool isActive)
    {
        foreach (GameObject g in unitApperance)
        {
            g.SetActive(isActive);
        }
        GetComponent<CapsuleCollider>().enabled = isActive;
        agent.enabled = isActive;
        isUnitVisable = isActive;
    }
    /// <summary>
    /// Cancels Unit Current action 
    /// </summary>
    public void CancelCurrentAction()
    {
        targetToFind = null;
        agent.ResetPath();
        if (unit.isGatheringRoutine) // Disable Gathering if unit has been moved during Process
        {
            if (unit.isHarvesting)
                resourceBeingGathered.GetComponent<Resource>().StopGathering(this); //stops coroutine only on this unit
            unit.isGatheringRoutine = false;
            resourceBeingGathered = null;
        }
        if(unit.isBuilding)
        {
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, searchRadius);
            foreach (Collider c in collidersInRange)
            {
                if (c.isTrigger) // only stop building from the buildingpoint collider(Sphere).
                {
                    ConstructBuilding building;
                    if (c.gameObject.transform.parent.TryGetComponent<ConstructBuilding>(out building))
                    {
                        building.StopBuilding(this);
                    }
                }

            }
        }
        if(unit.isInWorkshop)
        {
            unit.isInWorkshop = false;
            UIManager.instance.UpdateSelectedUnit(gameObject);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }

 
}

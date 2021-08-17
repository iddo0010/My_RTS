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
    //GameObject stockTarget;

    [SerializeField] float searchRadius;
    [SerializeField] LayerMask resourceLayer;

    public Weapon mainWeapon;
    public Weapon offWeapon;

    GameObject targetToFind;
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
        //stockTarget = null;
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
                    if (resourceBeingGathered == null)
                        SendToHarvest(lastResource);
                    //else
                    //{
                    //    if (!unit.isHarvesting)
                    //        targetToFind = resourceBeingGathered;
                    //}
                }
                //else
                //{
                //    if (stockTarget != null)
                //        //FindTarget(stockTarget);
                //        targetToFind = stockTarget;
                //    else
                //    {
                //        unit.isHarvesting = false;
                //        SetHarvestAnimation(false);
                //        unit.isGatheringRoutine = false;
                //        resourceBeingGathered = null;
                //        StopAllCoroutines();
                //    }
                //}
            }

            if (targetToFind != null)// TODO
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
    /// Activate/Deactivate Harvest Animation
    /// </summary>
    /// <param name="isHarvest"></param>
    public void SetHarvestAnimation(bool isHarvest)
    {
        anm.SetBool("isHarvesting", isHarvest);
    }
    /// <summary>
    /// Check For User Command Over Unit, Move Unit Accordingly
    /// </summary>
    private void CheckForUserInput()
    {
        if (!SelectionManager.instance.isSetTargetMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                MoveUnit();
            }
        }
        if (Input.GetKey(KeyCode.Space))
            CameraMovement.instance.FollowUnit();
    }
    /// <summary>
    /// Move Unit Towards Clicked location, if clicked on game object act accordingly
    /// </summary>
    public void MoveUnit()
    {
        if (!SelectionManager.instance.IsMouseOverUI())
        {
            CancelCurrentAction();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.transform.tag)
                {
                    case "Tree":
                        Instantiate(resourceCommand, hit.transform.position, resourceCommand.transform.rotation);
                        GoToTarget(hit.transform.gameObject);
                        break;
                    case "Stone":
                        Instantiate(resourceCommand, hit.transform.position, resourceCommand.transform.rotation);
                        GoToTarget(hit.transform.gameObject);
                        break;
                    case "StockPile":
                        SendToStockPile(hit.transform);
                        break;
                    case "Ground":
                        Instantiate(moveCommand, new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z), resourceCommand.transform.rotation);
                        agent.SetDestination(hit.point);
                        break;
                }

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
        GameObject stockTarget = ResourceManager.instance.FindNearesStockPile(transform.position, type)[0];
        if (stockTarget != null)
            GoToTarget(stockTarget.transform.Find("UnloadSpot").gameObject);
    }
    /// <summary>
    /// Manually Send a Unit to stockpile by user input
    /// </summary>
    /// <param name="stockPile">stockpile the user has clicked on</param>
    public void SendToStockPile(Transform stockPile)
    {
        GoToTarget(stockPile.Find("UnloadSpot").gameObject);
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
        agent.SetDestination(target.transform.position);
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
        }      
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
                        c.GetComponent<Resource>().Gather(this);
                        break;
                    case 11: //Building Layer
                        switch (target.tag)
                        {
                            case "StockPile":
                                GameObject stockPile = target.transform.parent.gameObject;
                                ResourceManager.instance.Unload(unit, stockPile.GetComponent<StockEngine>().type, stockPile);
                                SendToHarvest(stockPile.GetComponent<StockEngine>().type);
                                break;
                            case "Construction":
                                if (mainWeapon.canBuild)
                                {
                                    //anm.SetBool("isBuilding", true);
                                }
                                break;
                        }
                        break;
                }


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
        agent.ResetPath();
        if (unit.isGatheringRoutine) // Disable Gathering if unit has been moved during Process
        {
            if (unit.isHarvesting)
                resourceBeingGathered.GetComponent<Resource>().StopGathering(this); //stops coroutine only on this unit
            unit.isGatheringRoutine = false;
            resourceBeingGathered = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}

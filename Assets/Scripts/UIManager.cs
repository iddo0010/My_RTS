using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //Singleton
    public static UIManager instance;

    //Selected Units Panel
    [SerializeField] GameObject singleUnitImage;
    public GameObject multipleUnitContent;
    [SerializeField] GameObject multipleUnitOption;

    //Unit Action Panel
    [SerializeField] Transform unitCommands;
    [SerializeField] Transform actionList;

    public bool isBluePrintEnabled;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        isBluePrintEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Opens a specific Action Panel by int
    /// </summary>
    /// <param name="i"></param>
    public void OpenActionsPanel(int i)
    {
        foreach (Transform panel in actionList)
        {
            if (panel.gameObject.activeInHierarchy)
                panel.gameObject.SetActive(false);
        }
        actionList.GetChild(i).gameObject.SetActive(true);
    }
    /// <summary>
    /// Reset the UI for no selection
    /// </summary>
    public void DeSelectUI()
    {
        if (singleUnitImage.activeInHierarchy) //Reset Single unit image
        {
            foreach (Transform child in singleUnitImage.transform)
            {
                if (child.gameObject.activeInHierarchy)
                    child.gameObject.SetActive(false);
            }
            singleUnitImage.SetActive(false);
        }
        if(multipleUnitContent.activeInHierarchy) // Reset Mulitple Units Images
        {
            foreach(Transform child in multipleUnitContent.transform)
            {
                Destroy(child.gameObject);
            }
            multipleUnitContent.SetActive(false);
        }
        OpenActionsPanel(0); // open the defalt panel
        foreach (Transform command in unitCommands) // Reset all unit Commands
        {
            if(command.childCount > 0)
                command.GetChild(0).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Update UI for a single Unit
    /// </summary>
    /// <param name="selectedUnit"></param>
    public void UpdateSelectedUnit(GameObject selectedUnit)
    {
        singleUnitImage.SetActive(true);
        UnitEngine engine = selectedUnit.GetComponent<UnitEngine>();
        foreach (Transform child in singleUnitImage.transform)
        {
            if (child.name == engine.mainWeapon.type.ToString())
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
        }

        if (!engine.unit.isInWorkshop) //if the unit not currently in a workshop, updates the general unit actions
            UpdateUnitActions(engine.mainWeapon.canBuild);

        else // if the unit is in a workshop, opens the tool selection bar from this specific workshop
        {
            //TODO extract to a new overload method of UpdateSelectedUnit(GameObject SelectedUnit, ToolsProduction workshop)
            //FindObjectOfType<ActionsListenLogic>().SetToolsSelection(workshop);
            ActionsListenLogic.instance.SetToolsSelection(engine.currentWorkshop);
            OpenActionsPanel(3); //ToolsSelection panel
        }
    }
    /// <summary>
    /// Updates the UI for Multiple Units
    /// </summary>
    /// <param name="selectedUnits"></param>
    public void UpdateSelectedUnit(List<GameObject> selectedUnits)
    {
        bool canBuild = false;
        if (selectedUnits.Count == 1)
            UpdateSelectedUnit(selectedUnits[0]);
        if (selectedUnits.Count > 1)
        {
            multipleUnitContent.SetActive(true);
            foreach (GameObject unit in selectedUnits)
            {
                GameObject temp = Instantiate(multipleUnitOption, multipleUnitContent.transform);
                foreach (Transform child in temp.transform)
                {
                    if (child.name == unit.GetComponent<UnitEngine>().mainWeapon.type.ToString())
                        child.gameObject.SetActive(true);
                }
                if (unit.GetComponent<UnitEngine>().mainWeapon.canBuild)
                    canBuild = true;
            }
            UpdateUnitActions(canBuild);
        }
    }
    /// <summary>
    /// Updates the General Unit actions
    /// </summary>
    /// <param name="canBuild"></param>
    private void UpdateUnitActions(bool canBuild)
    {
        OpenActionsPanel(0);
        int index = 1;
        foreach(Transform child in unitCommands)
        {
            if (index == 5)//if the current button is the building option menu
            {
                if (canBuild) // if the unit can build, adds the building option menu button
                    child.GetChild(0).gameObject.SetActive(true);
                break;
            }
            else
                child.GetChild(0).gameObject.SetActive(true);
            index++;
        }
    }
    /// <summary>
    /// Cancel Unit Action Button
    /// </summary>
    public void CancelUnitAction()
    {
        foreach(GameObject unit in SelectionManager.instance.selectedUnits)
        {
            unit.GetComponent<UnitEngine>().CancelCurrentAction();
        }
    }
    /// <summary>
    /// Set Unit Path Button
    /// </summary>
    public void SetUnitPath()
    {
        SelectionManager.instance.isSetTargetMode = true;
    }
    /// <summary>
    /// Updates the action bar to show the correct building actions by transform given
    /// </summary>
    /// <param name="transform"></param>
    public void BuildingActionsUI(Transform transform)
    {
        switch (transform.tag)
        {
            case "Workshop":
                OpenActionsPanel(2);
                break;
            case "Camp":
                OpenActionsPanel(4);
                break;
            case "Tower":
                OpenActionsPanel(5);
                break;
        }
        ICreator creator;
        if (transform.TryGetComponent<ICreator>(out creator)) //if the building has a ICreator (Can create objects)
            UpdateBuildingInfo(creator);
    }
    /// <summary>
    /// Updates the info pannel to show the given building current creation Proccess
    /// </summary>
    /// <param name="creator"></param>
    public void UpdateBuildingInfo(ICreator creator)
    {
        multipleUnitContent.SetActive(true); //info pannel
        foreach(GameObject icon in creator.GetIconQueue()) //run on the building current icon queue 
        {
            GameObject newIcon = Instantiate(icon, multipleUnitContent.transform);
            newIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate { UIManager.instance.UnQueueUnit(newIcon.transform.GetSiblingIndex()); });
        }
    }
    /// <summary>
    /// Starts a new object creation process
    /// </summary>
    /// <param name="icon"></param>
    public void QueueUnit(GameObject icon)
    {
        ICreator currentCreator = SelectionManager.instance.selectedBuilding.GetComponent<ICreator>(); // current ICreator building selected 
        ICommand c = new CreationCommand(currentCreator, icon.name); // New Creation Command with the current ICreator
        currentCreator.commandHandler.AddCommand(c); // adds the new command to the current building queue handler
        currentCreator.AddToQueue(icon); // adds the icon given to the current building icon queue (to show on the UI)
    }
    /// <summary>
    /// Removes a specific command from queue by index
    /// </summary>
    /// <param name="index"></param>
    public void UnQueueUnit(int index)
    {
        ICreator currentCreator = SelectionManager.instance.selectedBuilding.GetComponent<ICreator>(); // current ICreator building selected 
        RemoveFromQueue.RemoveAt(currentCreator.GetIconQueue(), index);
        Destroy(multipleUnitContent.transform.GetChild(index).gameObject); //removes icon from the info panel
        UpdateQueueListeners(index);
        if (index == 0) // if the command is first in line
        {
            currentCreator.Stop();
            //if(UnitAllowance.instance.CanBuildUnits()) 
            //    currentCreator.Stop(); // stop running coroutine from current ICreator
            //else
            //    currentCreator.commandHandler.RemoveCommand(index); //if the coroutine didnt start yet, remove the first command from the command handler
        }
        else
            currentCreator.commandHandler.RemoveCommand(index - 1);
    }

    public void UpdateQueueListeners(int index)
    {
        foreach (Transform icon in multipleUnitContent.transform) //Delegates a new listener to each icon after the removed icon with a correct value
        {
            if (icon.GetSiblingIndex() >= index)
            {
                Button button = icon.GetComponentInChildren<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { UIManager.instance.UnQueueUnit(icon.GetSiblingIndex()); });
            }
        }
    }

    /// <summary>
    /// Activates a bluePrint object by building given
    /// </summary>
    /// <param name="building"></param>
    public void ActivateBluePrint(BuildingSettings building)
    {
        if (!isBluePrintEnabled && ResourceManager.instance.CanBuild(building.woodCost, building.stoneCost, building.goldCost))
        {
            isBluePrintEnabled = true;
            Transform newBuilding = Instantiate(building.bluePrint);
            newBuilding.GetComponent<Blueprint>().building = building;
        }
    }
    /// <summary>
    /// Activate the SetTargetMode, letting user choose a spawn point while building is selected
    /// </summary>
    public void SpawnPointButton()
    {
        SelectionManager.instance.isSetTargetMode = true;
    }
}

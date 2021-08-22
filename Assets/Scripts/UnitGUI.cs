using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitGUI : MonoBehaviour
{
    //Singleton
    public static UnitGUI instance;

    //Selected Units Panel
    [SerializeField] GameObject singleUnitImage;
    [SerializeField] GameObject multipleUnitContent;
    [SerializeField] GameObject multipleUnitOption;

    //Unit Action Panel
    Transform itemList;
    [SerializeField] Transform unitCommands;
    [SerializeField] Transform buildingOptions;
    [SerializeField] Transform buildingActions;
    Transform SelectionTool;

    public bool isBluePrintEnabled;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        isBluePrintEnabled = false;
        SelectionTool = GameObject.Find("Item List").transform.GetChild(3);
        itemList = GameObject.Find("Item List").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateSelectedUnit()
    {
        if (singleUnitImage.activeInHierarchy)
        {
            foreach (Transform child in singleUnitImage.transform)
            {
                if (child.gameObject.activeInHierarchy)
                    child.gameObject.SetActive(false);
            }
            singleUnitImage.SetActive(false);
        }
        if(multipleUnitContent.activeInHierarchy)
        {
            foreach(Transform child in multipleUnitContent.transform)
            {
                Destroy(child.gameObject);
            }
            multipleUnitContent.SetActive(false);
        }
        //if (buildingOptions.gameObject.activeInHierarchy || buildingActions.gameObject.activeInHierarchy)
            OpenActionsPanel(0);
        foreach (Transform command in unitCommands)
        {
            if(command.childCount > 0)
                command.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void UpdateSelectedUnit(GameObject selectedUnit)
    {
        if(selectedUnit.layer == 11) //Building Layer
        {
            UpdateBuildingActions();
        }
        else
        {
            singleUnitImage.SetActive(true);
            UnitEngine engine = selectedUnit.GetComponent<UnitEngine>();
            foreach(Transform child in singleUnitImage.transform)
            {
                if (child.name.Contains(engine.mainWeapon.type.ToString()))
                    child.gameObject.SetActive(true);
            }

            if(!engine.unit.isInWorkshop)
                UpdateUnitActions(engine.mainWeapon.canBuild);
            
            else
            {
                OpenActionsPanel(3);
            }
            
        }
    }
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
                    if (child.name.Contains(unit.GetComponent<UnitEngine>().mainWeapon.type.ToString()))
                        child.gameObject.SetActive(true);
                }
                if (unit.GetComponent<UnitEngine>().mainWeapon.canBuild)
                    canBuild = true;
            }
            UpdateUnitActions(canBuild);
        }
    }
    private void UpdateUnitActions(bool canBuild)
    {
        int index = 1;
        foreach(Transform child in unitCommands)
        {
            if (index == 5)
            {
                if (canBuild)
                    child.GetChild(0).gameObject.SetActive(true);
                break;
            }
            else
                child.GetChild(0).gameObject.SetActive(true);
            index++;
        }
    }

    private void UpdateBuildingActions()
    {
        OpenActionsPanel(2);
        foreach (Transform child in buildingActions)
        {   
            if(child.childCount != 0)
                child.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void CancelUnitAction()
    {
        foreach(GameObject unit in SelectionManager.instance.selectedUnits)
        {
            unit.GetComponent<UnitEngine>().CancelCurrentAction();
        }
    }
    public void SetUnitPath()
    {
        SelectionManager.instance.isSetTargetMode = true;
    }

    public void OpenActionsPanel(int i)
    {
        foreach(Transform panel in itemList)
        {
            if (panel.gameObject.activeInHierarchy)
                panel.gameObject.SetActive(false);
        }
            itemList.GetChild(i).gameObject.SetActive(true);           
    }
    //public void CloseBuildingPanel()
    //{
    //    unitCommands.gameObject.SetActive(true);
    //    buildingOptions.gameObject.SetActive(false);
    //    buildingActions.gameObject.SetActive(false);

    //}
    //public void OpenBuildingActionsPanel()
    //{
    //    unitCommands.gameObject.SetActive(false);
    //    buildingOptions.gameObject.SetActive(false);
    //    buildingActions.gameObject.SetActive(true);
    //}

    //public void OpenBuildingPanel()
    //{
    //    unitCommands.gameObject.SetActive(false);
    //    buildingOptions.gameObject.SetActive(true);
    //    buildingActions.gameObject.SetActive(false);
    //}

    //public void OpenToolsSelection()
    //{
    //    unitCommands.gameObject.SetActive(false);
    //    buildingOptions.gameObject.SetActive(false);
    //    buildingActions.gameObject.SetActive(false);
    //    SelectionTool.gameObject.SetActive(true);
    //}
    public void ActivateBluePrint(BuildingSettings building)
    {
        if (!isBluePrintEnabled && ResourceManager.instance.CanBuild(building.woodCost, building.stoneCost))
        {
            isBluePrintEnabled = true;
            Transform newBuilding = Instantiate(building.bluePrint);
            newBuilding.GetComponent<Blueprint>().building = building;
        }
    }
}

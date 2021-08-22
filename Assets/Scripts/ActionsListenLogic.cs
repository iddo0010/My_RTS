using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ActionsListenLogic : MonoBehaviour
{
    GameObject unitCommands, buildOptions, buildingActions;
    UnitGUI canvas;
    ToolsProduction workshopScript;
    [SerializeField] BuildingSettings[] buildings;
    string[] tools = { "HarvestingAxe", "WarAxe", "Hammer", "Club", "MiningHoe", "Spear", "Shield" };
    void Start()
    {

        unitCommands = transform.GetChild(0).gameObject;
        buildOptions = transform.GetChild(1).gameObject;
        buildingActions = transform.GetChild(2).gameObject;
        canvas = FindObjectOfType<UnitGUI>();
        workshopScript = FindObjectOfType<ToolsProduction>();
        SetUnitCommands();
        SetBuildOptions();
        SetBuildingActions();
    }

    //Refactor by inserting all strings into an array and changing to a for-loop
    private void SetBuildingActions()
    {
        for(int index = 0; index < tools.Length; index++)
        {
            string weaponName = tools[index];
            buildingActions.transform.GetChild(index).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { workshopScript.CreateTool(weaponName); });
        }
    }

    private void SetBuildOptions()
    {
        for(int index =0; index < buildings.Length; index++)
        {
            BuildingSettings building = buildings[index];
            buildOptions.transform.GetChild(index).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { canvas.ActivateBluePrint(building); });
        }
        buildOptions.transform.GetChild(14).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { canvas.OpenActionsPanel(0); });
        //tent = AssetDatabase.LoadAssetAtPath<BuildingSettings>("Assets/Prefabs/Buildings/ScriptableObj/Tent.asset");
        //workshop = AssetDatabase.LoadAssetAtPath<BuildingSettings>("Assets/Prefabs/Buildings/ScriptableObj/Workshop.asset");
        //buildOptions.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { canvas.ActivateBluePrint(tent); });
        //buildOptions.transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { canvas.ActivateBluePrint(workshop); });
    }

    private void SetUnitCommands()
    {
        unitCommands.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(FindObjectOfType<SelectionManager>().DeSelect);
        unitCommands.transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(canvas.SetUnitPath);
        unitCommands.transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(canvas.CancelUnitAction);
        unitCommands.transform.GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(FindObjectOfType<CameraMovement>().FollowUnit);
        unitCommands.transform.GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { canvas.OpenActionsPanel(1); }) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

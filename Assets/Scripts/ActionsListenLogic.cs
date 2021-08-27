using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ActionsListenLogic : MonoBehaviour
{
    GameObject unitCommands, buildOptions, workshopActions, toolsSelection;
    UnitGUI canvas;
    ToolsProduction workshopScript;
    [SerializeField] BuildingSettings[] buildings;
    string[] tools = { "HarvestingAxe", "WarAxe", "Hammer", "Club", "MiningHoe", "Spear", "Shield" };
    void Start()
    {
        unitCommands = transform.GetChild(0).gameObject;
        buildOptions = transform.GetChild(1).gameObject;
        workshopActions = transform.GetChild(2).gameObject;
        toolsSelection = transform.GetChild(3).gameObject;
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
            workshopActions.transform.GetChild(index).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { workshopScript.CreateTool(weaponName); });
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
    }

    private void SetUnitCommands()
    {
        unitCommands.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(FindObjectOfType<SelectionManager>().DeSelect);
        unitCommands.transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(canvas.SetUnitPath);
        unitCommands.transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(canvas.CancelUnitAction);
        unitCommands.transform.GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(FindObjectOfType<CameraMovement>().FollowUnit);
        unitCommands.transform.GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { canvas.OpenActionsPanel(1); }) ;
    }

    public void SetToolsSelection(GameObject workshop)
    {
       List<GameObject> tools = workshop.GetComponent<ToolsProduction>().GetTools();

        for (int i = 0; i < tools.Count; i++)
        {
            string tool = tools[i].name;
            //toolsSelection.transform.GetChild(i).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { workshopScript.CreateTool(weaponName); });
            print(tool);
            tool = tool.Replace("(Clone)", "_Icon");
            print(tool);

            toolsSelection.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Images/" + tool+".png");
            if(toolsSelection.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite.name == "None")
            {
                toolsSelection.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

        //foreach(GameObject t in workshop.GetComponent<ToolsProduction>().GetTools())
        //{
        //    toolsSelection.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick
        //    print(t);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

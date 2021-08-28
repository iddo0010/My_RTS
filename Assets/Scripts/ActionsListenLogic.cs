using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ActionsListenLogic : MonoBehaviour
{
    //Singelton 
    public static ActionsListenLogic instance;

    GameObject unitCommands, buildOptions, toolsSelection;
    [SerializeField] BuildingSettings[] buildings; //Array of all building to be built

    void Awake()
    {
        instance = this;
        unitCommands = transform.GetChild(0).gameObject;
        buildOptions = transform.GetChild(1).gameObject;
        toolsSelection = transform.GetChild(3).gameObject;
        SetUnitCommands();
        SetBuildOptions();
    }

    private void SetBuildOptions()
    {
        for (int index = 0; index < buildings.Length; index++)
        {
            BuildingSettings building = buildings[index];
            buildOptions.transform.GetChild(index).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { UIManager.instance.ActivateBluePrint(building); });
        }
        buildOptions.transform.GetChild(14).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { UIManager.instance.OpenActionsPanel(0); });
    }

    private void SetUnitCommands()
    {
        unitCommands.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(SelectionManager.instance.DeSelect);
        unitCommands.transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(UIManager.instance.SetUnitPath);
        unitCommands.transform.GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(UIManager.instance.CancelUnitAction);
        unitCommands.transform.GetChild(3).GetChild(0).GetComponent<Button>().onClick.AddListener(FindObjectOfType<CameraMovement>().FollowUnit);
        unitCommands.transform.GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { UIManager.instance.OpenActionsPanel(1); }) ;
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

    public void SetUpgradeButton(UpgradeBuilding currentBuilding)
    {
        foreach (Transform panel in transform)
        {
            if (panel.gameObject.activeInHierarchy)
            {
                Transform upgradeButton = panel.GetChild(13).GetChild(0);
                upgradeButton.gameObject.SetActive(true);
                upgradeButton.GetComponentInChildren<Button>().onClick.AddListener(currentBuilding.UpgradeButton);
            }
        }
    }
    public void RemoveUpgradeListener()
    {
        foreach (Transform panel in transform)
        {
            if (panel.gameObject.activeInHierarchy)
            {
                Transform upgradeButton = panel.GetChild(13).GetChild(0);
                upgradeButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                upgradeButton.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ToolsProduction : MonoBehaviour
{

    List<GameObject> tools = new List<GameObject>();


    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTool(string toolName)
    {
        GameObject tool = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/" + toolName + ".prefab");
        Transform rack = SelectionManager.instance.selectedBuilding.transform.Find("Rack");
        foreach(Transform slot in rack)
        {
            if(slot.childCount == 0)
            {
                tool = Instantiate(tool, slot);
                ResetToolTransform(tool);
                break;
            }
        }       
        
    }

    private static void ResetToolTransform(GameObject tool)
    {
        tool.transform.localRotation = Quaternion.Euler(Vector3.zero);
        tool.transform.localPosition = Vector3.zero;
        tool.transform.localScale = Vector3.one;
    }

    public void ChooseTool(UnitEngine unit, GameObject workshop)
    {
        unit.transform.position = transform.GetChild(1).position + (Vector3.forward * 1.5f);
        unit.transform.LookAt(transform.GetChild(1));
        unit.unit.isInWorkshop = true;
        FindObjectOfType<ActionsListenLogic>().SetToolsSelection(workshop);
        UnitGUI.instance.UpdateSelectedUnit(unit.gameObject);
    }

    public List<GameObject> GetTools()
    {
        tools.Clear();
        for (int i = 0; i < 4; i++)
        {
            if(transform.GetChild(1).GetChild(i).childCount != 0)
            {
                //tools[i] = transform.GetChild(1).GetChild(i).GetChild(0).gameObject;
                tools.Add(transform.GetChild(1).GetChild(i).GetChild(0).gameObject);
            }       
        }
 
        return tools;
    }
}

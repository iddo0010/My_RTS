using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ToolsProduction : MonoBehaviour
{

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTool(string toolName)
    {
        GameObject tool = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/" + toolName + ".prefab");
        Transform rack = SelectionManager.instance.selectedUnits[0].transform.GetChild(1);
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

    public void ChooseTool(UnitEngine unit)
    {
        unit.transform.position = transform.GetChild(1).position + (Vector3.forward * 1.5f);
        unit.transform.LookAt(transform.GetChild(1));
        unit.unit.isInWorkshop = true;
        UnitGUI.instance.UpdateSelectedUnit(unit.gameObject);
    }
}

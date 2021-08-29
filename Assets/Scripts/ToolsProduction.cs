using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ToolsProduction : MonoBehaviour
{

    List<GameObject> tools = new List<GameObject>();
    Transform rack;

    // Start is called before the first frame update
    void Awake()
    {
    }

    private void Start()
    {
        rack = transform.GetChild(1);
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

    public void ChooseTool(UnitEngine unit)
    {
        unit.transform.position = transform.GetChild(1).position + (Vector3.forward * 1.5f);
        unit.transform.LookAt(transform.GetChild(1));
        unit.unit.isInWorkshop = true;
        UIManager.instance.UpdateSelectedUnit(unit.gameObject);
    }

    public List<GameObject> GetTools()
    {
        tools.Clear();
        for (int i = 0; i < 4; i++)
        {
            if (rack.GetChild(i).childCount != 0)
                tools.Add(rack.GetChild(i).GetChild(0).gameObject);
            else
                tools.Add(null);
        }
 
        return tools;
    }

 


}
//if(Input.GetKeyDown(KeyCode.Alpha4))
//       {
//           int num = 0;
//           foreach (Transform child in rack)
//           {
//               if (child.childCount != 0)
//                   num++;
//           }
//           print(num);
//       }

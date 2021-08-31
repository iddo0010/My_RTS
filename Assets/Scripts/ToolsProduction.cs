using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ToolsProduction : MonoBehaviour, ICreator
{

    List<GameObject> tools = new List<GameObject>();
    Transform rack;

    [SerializeField] int weaponCreationDelay;
    [SerializeField] int weaponCreationSteps;

    Transform multipleUnitContent;

    Queue<GameObject> currentQueue;
    public CommandHandler commandHandler { get { return GetComponent<CommandHandler>(); } }

    public bool canBuild; // check if the building is during a creation coroutine
    public int toolsInRack;
    // Start is called before the first frame update
    void Awake()
    {
        toolsInRack = 0;
        currentQueue = new Queue<GameObject>();
        multipleUnitContent = GameObject.Find("MultipleUnits").transform.Find("UnitList/Viewport/Content");
        canBuild = true;
    }

    private void Start()
    {
        rack = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (canBuild && toolsInRack < 4)
            commandHandler.ExecuteCommand(); // execute a command from the building command handler
    }

    //public void CreateTool(string toolName)
    //{
    //    Transform rack = SelectionManager.instance.selectedBuilding.transform.Find("Rack");
    //    foreach(Transform slot in rack)
    //    {
    //        if(slot.childCount == 0)
    //        {
    //            tool = Instantiate(tool, slot);
    //            ResetToolTransform(tool);
    //            break;
    //        }
    //    }             
    //}

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
        if(unit.unit.isSelected)
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
    //public int ToolsInRack()
    //{
    //    int toolCount = 0;
    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (rack.GetChild(i).childCount != 0)
    //            toolCount++;
    //    }
    //    return toolCount;
    //}

    public Queue<GameObject> GetIconQueue()
    {
        return currentQueue;
    }

    public void AddToQueue(GameObject icon)
    {
        int iconIndex = currentQueue.Count;
        GameObject newIcon = Instantiate(icon, multipleUnitContent); //instantiates the icon in the info panel
        newIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate { UIManager.instance.UnQueueUnit(iconIndex); });
        currentQueue.Enqueue(icon);
    }

    public void Create(string name)
    {
        StartCoroutine(StartCreation(weaponCreationDelay, weaponCreationSteps, name));
        canBuild = false;
    }

    public void Stop()
    {
        if (canBuild)
            commandHandler.RemoveCommand(0);
        else
        {
            StopAllCoroutines();
            canBuild = true;
        }
    }
    private IEnumerator StartCreation(int delay, int steps, string name)
    {
        GameObject tool = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/" + name + ".prefab");
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Tick");
        }
        currentQueue.Dequeue(); // removes icon from queu
        foreach (Transform slot in rack)
        {
            if (slot.childCount == 0)
            {
                tool = Instantiate(tool, slot);
                toolsInRack++;
                ResetToolTransform(tool);
                break;
            }
        }
        canBuild = true;
        if (SelectionManager.instance.selectedBuilding == gameObject) // if the building is currently selected
            Destroy(multipleUnitContent.transform.GetChild(0).gameObject); //removes icon from the info panel
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

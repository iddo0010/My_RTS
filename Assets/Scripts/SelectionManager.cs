using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class SelectionManager : MonoBehaviour
{
    //Singelton
    public static SelectionManager instance;

    public RectTransform selectionBox;

    Vector2 startPos;

    public List<GameObject> unitList; // All Friendly Units in Game
    public List<GameObject> selectedUnits = new List<GameObject>(); //Units Selected

    public bool isSetTargetMode;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GameObject[] temp = GameObject.FindGameObjectsWithTag("FriendlyUnit");
        unitList = temp.ToList<GameObject>();
        isSetTargetMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetTargetMode)
        {
            if (Input.GetMouseButtonDown(0))//Single Selection
                SingleSelection();
            if (Input.GetMouseButton(0))// Box Selection
                UpdateSelectionBox(Input.mousePosition);
            if (Input.GetMouseButtonUp(0))// Release Selection
                ReleaseSelectionBox();
        }
        else
        {
            if(Input.GetMouseButtonDown(0))
            {
                foreach (GameObject unit in selectedUnits)
                {
                    unit.GetComponent<UnitEngine>().MoveUnit();
                }
            }
            if(Input.GetMouseButtonDown(1))
            {
                isSetTargetMode = false;
            }
        }
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
    /// <summary>
    /// Checks if mouse is over an UI element
    /// </summary>
    /// <returns>true for over UI</returns>
    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
        //PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        //pointerEventData.position = Input.mousePosition;

        //List<RaycastResult> raycastResults = new List<RaycastResult>();
        //EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        //for (int i = 0; i < raycastResults.Count; i++) 
        //{
        //    if (raycastResults[i].gameObject.layer == 5)
        //        return true;
        //}
        //return false;
    }
    /// <summary>
    /// Single Click Selection
    /// </summary>
    private void SingleSelection()
    {
        if (!IsMouseOverUI())
        {
            startPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(startPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                DeSelect();
                switch (hit.collider.gameObject.layer)
                {
                    case 8: //FriendlyUnit
                        hit.collider.GetComponent<UnitEngine>().SelectUnit();
                        selectedUnits.Add(hit.collider.gameObject);
                        UnitGUI.instance.UpdateSelectedUnit(selectedUnits);
                        break;
                    case 11: //Building
                        hit.collider.GetComponent<BuildingEngine>().SelectUnit();
                        selectedUnits.Add(hit.collider.gameObject);
                        UnitGUI.instance.UpdateSelectedUnit(hit.collider.gameObject);
                        break;

                }
            }
        }
    }
    /// <summary>
    /// Updates Box Selection
    /// </summary>
    /// <param name="curMousePos">Current mouse posision on screen</param>
    private void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (isSetTargetMode)
            return;
        float width;
        float hight;
        if (!IsMouseOverUI())
        {
            if (!selectionBox.gameObject.activeInHierarchy)
                selectionBox.gameObject.SetActive(true);
            width = curMousePos.x - startPos.x;
            hight = curMousePos.y - startPos.y;

            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(hight));
            selectionBox.anchoredPosition = startPos + new Vector2(width / 2, hight / 2);
        }
        else
        {
            if (selectionBox.gameObject.activeInHierarchy)
            {
                width = curMousePos.x - startPos.x;
                hight = curMousePos.y - startPos.y;
                selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(hight));
                selectionBox.anchoredPosition = startPos + new Vector2(width / 2, hight / 2);
            }
            else
                startPos = Input.mousePosition;
        }


    }
    /// <summary>
    /// Release Long Selection, Selects all units inside the selection box
    /// </summary>
    private void ReleaseSelectionBox()
    {
        if (selectionBox.gameObject.activeInHierarchy)
        {
            selectionBox.gameObject.SetActive(false);
            Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
            Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

            foreach (GameObject unit in unitList) //Checks all units in game
            {
                Vector2 screenPos = gameObject.GetComponent<Camera>().WorldToScreenPoint(unit.transform.position); // converts each unit pos to screen pos
                if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
                {
                    selectedUnits.Add(unit);
                    unit.GetComponent<UnitEngine>().SelectUnit();
                }
            }
            UnitGUI.instance.UpdateSelectedUnit(selectedUnits);
        }
    }
    /// <summary>
    /// Clears all objects selected
    /// </summary>
    public void DeSelect()
    {
        if (selectedUnits.Count > 0)
        {
            if(selectedUnits[0].GetComponent<UnitEngine>())
            {
                foreach (GameObject unit in selectedUnits)
                {
                    unit.GetComponent<UnitEngine>().DeSelectUnit();                    
                }
            }
            else if(selectedUnits[0].GetComponent<BuildingEngine>())
                foreach (GameObject unit in selectedUnits)
                {
                    unit.GetComponent<BuildingEngine>().DeSelectUnit();
                }
           
            selectedUnits.Clear();
            UnitGUI.instance.UpdateSelectedUnit();
        }

    }

}


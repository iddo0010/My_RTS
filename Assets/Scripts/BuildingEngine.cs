using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingEngine : MonoBehaviour
{
    bool isSelected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectUnit()
    {
        isSelected = true;
        //unitSelectionCircle.SetActive(true);
    }
    public void DeSelectUnit()
    {
        isSelected = false;
        //unitSelectionCircle.SetActive(false);
    }

}

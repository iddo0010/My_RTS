﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingEngine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectBuilding()
    {
        SelectionManager.instance.selectedBuilding = gameObject;
        BuildingActionsUI();
    }
    public void DeSelectBuilding()
    {
        SelectionManager.instance.selectedBuilding = null;
    }
    public void BuildingActionsUI()
    {
        switch(gameObject.tag)
        {
            case "Workshop":
                UIManager.instance.OpenActionsPanel(2);
                break;
            case "Camp":
                UIManager.instance.OpenActionsPanel(4);
                break;
            case "Tower":
                UIManager.instance.OpenActionsPanel(5);
                break;
        }
        UpgradeBuilding upgrade;
        if (TryGetComponent<UpgradeBuilding>(out upgrade) && gameObject.tag != "Construction")
            ActionsListenLogic.instance.SetUpgradeButton(upgrade);
    }


}

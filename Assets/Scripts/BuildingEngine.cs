using System.Collections;
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
        UIManager.instance.BuildingActionsUI(transform);
        UpgradeBuilding upgrade;
        if (TryGetComponent<UpgradeBuilding>(out upgrade) && gameObject.tag != "Construction")
            ActionsListenLogic.instance.SetUpgradeButton(upgrade);
    }
    public void DeSelectBuilding()
    {
        SelectionManager.instance.selectedBuilding = null;
    }


}

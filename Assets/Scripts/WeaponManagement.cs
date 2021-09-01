using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagement : MonoBehaviour
{
    //Manage Unit Weapons
    UnitEngine engine;
    [SerializeField] GameObject mainWeaponDir;

    private void Start()
    {
        CheckIfWeaponEquipped();
    }


    // Start is called before the first frame update
    void Awake()
    {
        engine = GetComponent<UnitEngine>();

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// When Game starts, Checks on existing units if they have any weapon equipped, as new units instantiate wihtout weapons
    /// </summary>
    public void CheckIfWeaponEquipped()
    {
        foreach(Transform child in mainWeaponDir.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                engine.mainWeapon = new Weapon(child.name);
            }
        }

    }

    public void SetWeapon(string weapon)
    {
        weapon = weapon.Replace("(Clone)", "");
        
        foreach (Transform child in mainWeaponDir.transform)
        {
            
            if (child.name == weapon)
            {
                child.gameObject.SetActive(true);
                engine.mainWeapon = new Weapon(child.name);
                UIManager.instance.UpdateSelectedUnit(gameObject);
            }
            else
                child.gameObject.SetActive(false);
        }

    }
}

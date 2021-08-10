using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum weaponType
{
    None,HarvestingAxe, Axe, Hammer, Club, MiningHoe, Spear, Shield
}
[System.Serializable]
public class Weapon 
{
    public weaponType type;
    public float dmg;
    public int level;
    public bool canBuild;

    public Weapon(string name)
    {
        switch (name)
        {
            case "HarvestingAxe":
                type = weaponType.HarvestingAxe;
                canBuild = false;
                //dmg = 0; TODO - Decide on weapons DMG
                level = 1;
                break;
            case "Axe":
                type = weaponType.Axe;
                canBuild = false;
                //dmg = 0; TODO - Decide on weapons DMG
                level = 1;
                break;
            case "Hammer":
                type = weaponType.Hammer;
                canBuild = false;
                //dmg = 0; TODO - Decide on weapons DMG
                level = 1;
                break;
            case "Club":
                type = weaponType.Club;
                canBuild = false;
                //dmg = 0; TODO - Decide on weapons DMG
                level = 1;
                break;
            case "MiningHoe":
                type = weaponType.MiningHoe;
                canBuild = false;
                //dmg = 0; TODO - Decide on weapons DMG
                level = 1;
                break;
            case "Shield":
                type = weaponType.Shield;
                canBuild = false;
                //dmg = 0; TODO - Decide on weapons DMG
                level = 1;
                break;
            case "Spear":
                type = weaponType.Spear;
                canBuild = false;
                //dmg = 0; TODO - Decide on weapons DMG
                level = 1;
                break;
        }
    }
    public Weapon()
    {
        type = weaponType.None;
        dmg = 0.05f;
        canBuild = true;
    }
    //public void UpgradeWeapon()
    //{
    //    level++;
    //    dmg += 0; //dmg = 0; TODO - Decide on weapons DMG
    //}
}

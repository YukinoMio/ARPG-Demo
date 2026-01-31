using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageManager : MonoBehaviour
{
    private static PackageLocalData _instance;
    public static PackageLocalData Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new PackageLocalData(); 
            }
            return _instance;
        }
    }
    public List<WeaponInstance> weaponInventory = new List<WeaponInstance>();
}

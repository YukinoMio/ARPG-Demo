using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class WeaponInstance 
{
    public string uid;
    string weaponId;
    public int currentLevel;
    public bool isEquipped;
    public bool isNew;
    public List<DataModify> additionalAttributes=new List<DataModify>();
}

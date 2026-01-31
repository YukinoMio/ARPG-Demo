using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : PlayerComponentBase
{
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private Transform weaponHolder;
    private GameObject equippedWeaponInstance;
    public WeaponData CurrentWeapon=>currentWeapon;
    public WeaponType CurrentWeaponType => currentWeapon?.weaponType ?? WeaponType.Empty;

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        //自动查找武器挂载点
        if (weaponHolder == null)
            weaponHolder = transform.Find("WeaponHolder");
    }
    public void EquipWeapon(WeaponData weaponData)
    {
        //移除旧武器
        if (equippedWeaponInstance != null)
        {
            Destroy(equippedWeaponInstance);
        }
        currentWeapon=weaponData;   
        //实例化新武器
        if(weaponData.weaponPrefab != null&&weaponHolder!=null)
        {
            equippedWeaponInstance=Instantiate(weaponData.weaponPrefab,weaponHolder);
            equippedWeaponInstance.transform.localPosition = weaponData.equipPosition;
            equippedWeaponInstance.transform.localEulerAngles = weaponData.equipRotation;

        }
        ApplyWeaponAttributes();
        //发布装备事件
        EventCenter.Instance.Publish(new EquipmentEvents.WeaponEquipped
        {
            WeaponId = weaponData.weaponId,
            CharacterId = player.gameObject.name
        });
    }

    private void ApplyWeaponAttributes()
    {
        if (currentWeapon == null || player.AdvancedAttributeSystem == null)
        {
            return;
        }
        foreach(var modify in currentWeapon.baseAttributes)
        {
            player.AdvancedAttributeSystem.ApplyModify(modify, "WeaponEquip");
        }
    }
}

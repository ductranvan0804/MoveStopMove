using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skin : GameUnit
{
    [SerializeField] PantData pantData;

    [SerializeField] Transform head;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    [SerializeField] Renderer pant;

    [SerializeField] bool isCanChange = false;

    Weapon currentWeapon;
    Accessory currentAccessory;
    Hat currentHat;

    [SerializeField] Animator anim;
    public Animator Anim => anim;

    public Weapon Weapon => currentWeapon;

    public void ChangeWeapon(WeaponType weaponType)
    {
        currentWeapon = SimplePool.Spawn<Weapon>((PoolType)weaponType, rightHand);
    }

    public void ChangeAccessory(AccessoryType accessoryType)
    {
        if (isCanChange && accessoryType != AccessoryType.ACC_None)
        {
            currentAccessory = SimplePool.Spawn<Accessory>((PoolType)accessoryType, leftHand);
        }
    }

    public void ChangeHat(HatType hatType)
    {
        if (isCanChange && hatType != HatType.HAT_None)
        {
            currentHat = SimplePool.Spawn<Hat>((PoolType)hatType, head);
        }
    }

    public void ChangePant(PantType pantType)
    {
        pant.material = pantData.GetPant(pantType);
    }


    public void OnDespawn()
    {
        SimplePool.Despawn(currentWeapon);
        if (currentAccessory) SimplePool.Despawn(currentAccessory);
        if (currentHat) SimplePool.Despawn(currentHat);
    }

    public void DespawnHat()
    {
        if (currentHat)
        {
            SimplePool.Despawn(currentHat);
        }
            
    }
    public void DespawnAccessory()
    {
        if (currentAccessory) 
        { 
            SimplePool.Despawn(currentAccessory); 
        }
    }

    internal void DespawnWeapon()
    {
        if (currentWeapon) 
        { 
            SimplePool.Despawn(currentWeapon); 
        }
    }
}

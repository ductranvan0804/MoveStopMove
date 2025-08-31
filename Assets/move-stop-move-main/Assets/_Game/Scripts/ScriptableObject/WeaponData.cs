using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField] List<WeaponItem> weaponItems;
    
    public List<WeaponItem> WeaponItems => weaponItems;

    public WeaponItem GetWeaponItem(WeaponType weaponType)
    {
        return weaponItems.FirstOrDefault(w => w.type == weaponType);
    }

    public WeaponType GetNextWeaponType(WeaponType currentType)
    {
        int currentIndex = weaponItems.FindIndex(w => w.type == currentType);
        if (currentIndex == -1)
        {
            return currentType;
        }

        int nextIndex = currentIndex + 1;
        if (nextIndex >= weaponItems.Count)
        {
            nextIndex = 0;
        }

        return weaponItems[nextIndex].type;
    }

    public WeaponType GetPreviousWeaponType(WeaponType currentType)
    {
        int currentIndex = weaponItems.FindIndex(w => w.type == currentType);
        if ( currentIndex == -1)
        {
            return currentType;
        }

        int previousIndex = currentIndex - 1;
        if ( previousIndex < 0)
        {
            previousIndex = weaponItems.Count - 1;
        }

        return weaponItems[previousIndex].type;
    }

    [System.Serializable]
    public class WeaponItem
    {
        public string name;
        public WeaponType type;
        public int cost;
    }
}

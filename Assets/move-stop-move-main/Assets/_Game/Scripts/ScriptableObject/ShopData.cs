using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Scriptable Objects/ShopData")]
public class ShopData : ScriptableObject
{
    public ShopCategory<HatType> hats;
    public ShopCategory<PantType> pants;
    public ShopCategory<AccessoryType> accessories;
    public ShopCategory<SkinType> skins;

}

[System.Serializable]
public class ShopCategory<T> where T : System.Enum
{
    [SerializeField] private List<ShopItemData<T>> items;
    public List<ShopItemData<T>> Items => items;

    public ShopItemData<T> GetItemByType(T type)
    {
        return items.SingleOrDefault(item => item.type.Equals(type));
    }
}

[System.Serializable]
public class ShopItemData<T> : ShopItemData where T : System.Enum
{
    public T type;
}



[System.Serializable]
public class ShopItemData
{
    public Sprite icon;
    public int cost;
}

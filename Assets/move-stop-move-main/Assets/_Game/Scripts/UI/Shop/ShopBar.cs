using UnityEngine;

public class ShopBar : MonoBehaviour
{
    [SerializeField] GameObject shopBG;
    [SerializeField] UIShop.ShopType shopType;

    UIShop shop;

    public UIShop.ShopType Type => shopType;


    public void SetShop(UIShop shop)
    {
        this.shop = shop;
    }

    public void SelectShopBar()
    {
        shop.SelectBar(this);
    }

    public void Active (bool active)
    {
        shopBG.SetActive(active);
    }
}

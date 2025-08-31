using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UIShop : UICanvas
{
    public enum ShopType { Hat, Pant, Accessory, Skin, Weapon }

    [SerializeField] ShopData shopData;
    [SerializeField] ShopItem shopItem;
    [SerializeField] Transform content;
    [SerializeField] ShopBar[] shopBars;

    [SerializeField] ButtonState buttonState;
    [SerializeField] TextMeshProUGUI playerCoinTxt;
    [SerializeField] Text priceCoinTxt;

    MiniPool<ShopItem> miniPool = new MiniPool<ShopItem>();

    private ShopItem currentItem;
    private ShopBar currentBar;

    private ShopItem equippedItem;

    public ShopType shopType => currentBar.Type;

    private void Awake()
    {
        miniPool.OnInit(shopItem, 10, content);

        for (int i = 0; i < shopBars.Length; i++)
        {
            shopBars[i].SetShop(this);
        }
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();
        UIManager.Ins.OpenUI<UIMainMenu>();

        LevelManager.Ins.player.TakeOffClothes();
        LevelManager.Ins.player.OnTakeClothsData();
        LevelManager.Ins.player.WearClothes();
    }

    public override void Open()
    {
        base.Open();
        SelectBar(shopBars[0]);
        CameraFollower.Ins.ChangeState(CameraFollower.State.Shop);

        playerCoinTxt.SetText(UserData.Ins.coin.ToString());
    }

    internal void SelectBar(ShopBar shopBar)
    {
        if (currentBar != null)
        {
            currentBar.Active(false);
        }

        currentBar = shopBar;
        currentBar.Active(true);

        miniPool.Collect();
        equippedItem = null;

        switch (currentBar.Type)
        {
            case ShopType.Hat:
                InitShopItems(shopData.hats.Items, ref equippedItem);
                break;
            case ShopType.Pant:
                InitShopItems(shopData.pants.Items, ref equippedItem);
                break;
            case ShopType.Accessory:
                InitShopItems(shopData.accessories.Items, ref equippedItem);
                break;
            case ShopType.Skin:
                InitShopItems(shopData.skins.Items, ref equippedItem);
                break;
            default:
                break;
        }

    }
    public ShopItem.State GetState(Enum t) => UserData.Ins.GetEnumData(t.ToString(), ShopItem.State.Buy);
    internal void SelectItem(ShopItem item)
    {
        if (currentItem != null)
        {
            currentItem.SetState(GetState(currentItem.itemType));
        }

        currentItem = item;

        switch (currentItem.state)
        {
            case ShopItem.State.Buy:
                buttonState.SetState(ButtonState.State.Buy);
                break;
            case ShopItem.State.Bought:
                buttonState.SetState(ButtonState.State.Equip);
                break;
            case ShopItem.State.Equipped:
                buttonState.SetState(ButtonState.State.Equipped);
                break;
            case ShopItem.State.Selecting:
                break;
            default:
                break;
        }

        LevelManager.Ins.player.TryCloth(shopType, currentItem.itemType);
        currentItem.SetState(ShopItem.State.Selecting);

        //check data
        priceCoinTxt.text = item.data.cost.ToString();
    }

    private void InitShopItems<T>(List<ShopItemData<T>> items, ref ShopItem itemEquiped) where T : Enum
    {
        for (int i = 0; i < items.Count; i++)
        {
            ShopItem.State state = UserData.Ins.GetEnumData(items[i].type.ToString(), ShopItem.State.Buy);
            ShopItem item = miniPool.Spawn();
            item.SetData(i, items[i], this);
            item.SetState(state);

            if (state == ShopItem.State.Equipped)
            {
                SelectItem(item);
                itemEquiped = item;
            }

        }
    }

    public void BuyButton()
    {
        if (currentItem != null)
        {
            int itemCost = currentItem.data.cost;
            if (UserData.Ins.coin >= itemCost)
            {
                // tru coin
                UserData.Ins.StoreCoin(-itemCost);

                // luu trang thai da mua
                UserData.Ins.SetEnumData(currentItem.itemType.ToString(), ShopItem.State.Bought);

                // cap nhat giao dien 
                SelectItem(currentItem);

                // cap nhat so coin tren UI
                playerCoinTxt.SetText(UserData.Ins.coin.ToString());

                Debug.Log("Buy successful");
            }
            else
            {
                Debug.Log("Not enough coin to buy");
            }
        }

        
        

    }

    public void EquipButton()
    {
        if (currentItem != null)
        {
            UserData.Ins.SetEnumData(currentItem.itemType.ToString(), ShopItem.State.Equipped);

            switch (shopType)
            {
                case ShopType.Hat:
                    //reset trang thai do dang deo ve bought
                    UserData.Ins.SetEnumData(UserData.Ins.playerHat.ToString(), ShopItem.State.Bought);
                    //save id do moi vao player
                    UserData.Ins.SetEnumData(UserData.Key_Player_Hat, ref UserData.Ins.playerHat, (HatType)currentItem.itemType);
                    break;
                case ShopType.Pant:
                    UserData.Ins.SetEnumData(UserData.Ins.playerPant.ToString(), ShopItem.State.Bought);
                    UserData.Ins.SetEnumData(UserData.Key_Player_Pant, ref UserData.Ins.playerPant, (PantType)currentItem.itemType);
                    break;
                case ShopType.Accessory:
                    UserData.Ins.SetEnumData(UserData.Ins.playerAccessory.ToString(), ShopItem.State.Bought);
                    UserData.Ins.SetEnumData(UserData.Key_Player_Accessory, ref UserData.Ins.playerAccessory, (AccessoryType)currentItem.itemType);
                    break;
                case ShopType.Skin:
                    UserData.Ins.SetEnumData(UserData.Ins.playerSkin.ToString(), ShopItem.State.Bought);
                    UserData.Ins.SetEnumData(UserData.Key_Player_Skin, ref UserData.Ins.playerSkin, (SkinType)currentItem.itemType);
                    break;
                case ShopType.Weapon:
                    break;
                default:
                    break;
            }

        }

        if (equippedItem != null)
        {
            equippedItem.SetState(ShopItem.State.Bought);
        }

        currentItem.SetState(ShopItem.State.Equipped);
        SelectItem(currentItem);
    }


}

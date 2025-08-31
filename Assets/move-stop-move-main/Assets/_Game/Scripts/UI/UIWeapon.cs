using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using static WeaponData;

public class UIWeapon : UICanvas
{
    public Transform weaponPoint;

    [SerializeField] WeaponData weaponData;
    [SerializeField] ButtonState buttonState;
    [SerializeField] TextMeshProUGUI weaponNameTxt;
    [SerializeField] TextMeshProUGUI playerCoinTxt;
    [SerializeField] Text weaponPriceTxt;

    private Weapon currentWeapon;
    private WeaponType weaponType;

    public override void Setup()
    {
        base.Setup();
        ChangeWeapon(UserData.Ins.playerWeapon);
        playerCoinTxt.SetText(UserData.Ins.coin.ToString());
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        this.weaponType = weaponType;

        if (currentWeapon != null)
        {
            SimplePool.Despawn(currentWeapon);
        }
        currentWeapon = SimplePool.Spawn<Weapon>((PoolType) weaponType, Vector3.zero, Quaternion.identity, weaponPoint);

        // get the current status of the weapon 
        ButtonState.State state = (ButtonState.State)UserData.Ins.GetDataState(weaponType.ToString(), 0);
        buttonState.SetState(state);

        // get the weapon info and set name and price according to that weapon
        WeaponItem item = weaponData.GetWeaponItem(weaponType);
        weaponNameTxt.SetText(item.name);
        weaponPriceTxt.text = item.cost.ToString();
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();

        if (currentWeapon != null)
        {
            SimplePool.Despawn(currentWeapon);
            currentWeapon = null;
        }

        UIManager.Ins.OpenUI<UIMainMenu>();
    }

    public void NextWeaponButton()
    {
        ChangeWeapon(weaponData.GetNextWeaponType(weaponType));
    }

    public void PreviousWeaponButton()
    {
        ChangeWeapon(weaponData.GetPreviousWeaponType(weaponType));
    }

    public void BuyWeaponButton()
    {
        WeaponItem item = weaponData.GetWeaponItem(weaponType);

        if (UserData.Ins.coin >= item.cost)
        {
            // Tru tien
            UserData.Ins.StoreCoin(-item.cost);

            // Luu trang thai da mua
            UserData.Ins.SetDataState(weaponType.ToString(), (int)ShopItem.State.Bought);

            // Cap nhat lai giao dien
            ChangeWeapon(weaponType);

            // cap nhat UI so coin con lai
            playerCoinTxt.SetText(UserData.Ins.coin.ToString());
        }
        else
        {
            Debug.Log("Not enough coin to buy this weapon");
        }
    }

    public void EquipWeaponButton()
    {
        UserData.Ins.SetEnumData(weaponType.ToString(), ShopItem.State.Equipped);
        UserData.Ins.SetEnumData(UserData.Ins.playerWeapon.ToString(), ShopItem.State.Bought);
        UserData.Ins.SetEnumData(UserData.Key_Player_Weapon, ref UserData.Ins.playerWeapon, weaponType);
        ChangeWeapon(weaponType);
        LevelManager.Ins.player.TryCloth(UIShop.ShopType.Weapon, weaponType);
    }
}

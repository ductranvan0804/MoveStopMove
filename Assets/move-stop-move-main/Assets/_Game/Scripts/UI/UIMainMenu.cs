using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMainMenu : UICanvas
{
    private const string ANIM_OPEN = "Open";
    private const string ANIM_CLOSE = "Close";
    [SerializeField] TextMeshProUGUI playerCoinTxt;
    [SerializeField] RectTransform coinPoint;
    [SerializeField] Animation anim;
    public RectTransform CoinPoint => coinPoint;

    public override void Open()
    {
        base.Open();
        GameManager.Ins.ChangeState(GameState.MainMenu);
        CameraFollower.Ins.ChangeState(CameraFollower.State.MainMenu);

        playerCoinTxt.SetText(UserData.Ins.coin.ToString());

        anim.Play(ANIM_OPEN);
    }


    public void SettingButton()
    {

    }


    public void PlayButton()
    {
        LevelManager.Ins.OnPlay();
        UIManager.Ins.OpenUI<UIGameplay>();
        CameraFollower.Ins.ChangeState(CameraFollower.State.Gameplay);

        Close(0.5f);
        anim.Play(ANIM_CLOSE);
    }

    public void WeaponButton()
    {
        UIManager.Ins.OpenUI<UIWeapon>();
        Close(0);
    }

    public void ShopButton()
    {
        UIManager.Ins.OpenUI<UIShop>();
        Close(0);
    }
}


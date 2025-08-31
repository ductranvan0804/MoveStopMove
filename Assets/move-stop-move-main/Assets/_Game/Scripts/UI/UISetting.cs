using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UICanvas
{

    public override void Setup()
    {
        base.Setup();
        GameManager.Ins.ChangeState(GameState.Setting);
        UIManager.Ins.CloseUI<UIGameplay>();
    }

    public void SoundButton()
    {

    }

    public void VibrateButton()
    {

    }

    public void ContinueButton()
    {
        GameManager.Ins.ChangeState(GameState.GamePlay);
        UIManager.Ins.OpenUI<UIGameplay>();
        Close(0);
    }

    public void HomeButton()
    {
        LevelManager.Ins.Home();
    }

}

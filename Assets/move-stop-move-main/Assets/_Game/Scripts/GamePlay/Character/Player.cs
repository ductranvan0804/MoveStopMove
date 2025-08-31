using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float moveSpeed = 5f;

    private bool isMoving = false;

    private Coroutine coroutine;

    private bool IsCanUpdate => GameManager.Ins.IsState(GameState.GamePlay) || GameManager.Ins.IsState(GameState.Setting);

    SkinType skinType = SkinType.SKIN_Normal;
    WeaponType weaponType = WeaponType.W_Candy_1;
    HatType hatType = HatType.HAT_Cap;
    AccessoryType accessoryType = AccessoryType.ACC_Headphone;
    PantType pantType = PantType.Pant_8;

    [SerializeField] ParticleSystem reviveVFX;

    public int Coin => Score * 10;

    // Update is called once per frame
    void Update()
    {
        if (IsCanUpdate && !IsDead)
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
            }

            if (Input.GetMouseButton(0) && JoystickControl.direct != Vector3.zero)
            {
                rb.MovePosition(rb.position + JoystickControl.direct * moveSpeed * Time.deltaTime);
                TF.position = rb.position;

                TF.forward = JoystickControl.direct;

                ChangeAnim(Constant.ANIM_RUN);
                isMoving = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isMoving = false;
                OnMoveStop();
                OnAttack();

            }
        }
    }

    public override void OnInit()
    {
        OnTakeClothsData();

        base.OnInit();

        TF.rotation = Quaternion.Euler(Vector3.up * 180);
        SetSize(MIN_SIZE);

        indicator.SetName("YOU");
    }

    IEnumerator ThrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Throw();
    }


    public override void WearClothes()
    {
        base.WearClothes(); 

        ChangeSkin(skinType);
        ChangeWeapon(weaponType);
        ChangeHat(hatType);
        ChangeAccessory(accessoryType);
    }

    public override void OnMoveStop()
    {
        base.OnMoveStop();
        rb.linearVelocity = Vector3.zero;
        ChangeAnim(Constant.ANIM_IDLE);
    }

    public override void AddTarget(Character target)
    {
        base.AddTarget(target);

        if (!target.IsDead && !IsDead)
        {
            target.SetTargetMask(true);
            if (coroutine == null && !isMoving)
            {
                OnAttack();
            }
        }
    }

    public override void RemoveTarget(Character target)
    {
        base.RemoveTarget(target);
        target.SetTargetMask(false);
    }

    public override void OnAttack()
    {
        base.OnAttack();
        if (target != null && currentSkin.Weapon.IsCanAttack)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(ThrowAfterDelay(TIME_DELAY_THROW));

            ChangeAnim(Constant.ANIM_ATTACK);
        }
    }


    protected override void SetSize(float size)
    {
        base.SetSize(size);
        CameraFollower.Ins.SetRateOffset((this.size - MIN_SIZE) / (MAX_SIZE - MIN_SIZE));
    }

    internal void OnRevive()
    {
        ChangeAnim(Constant.ANIM_IDLE);
        IsDead = false;
        ClearTarget();

        reviveVFX.Play();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void TryCloth(UIShop.ShopType shopType, Enum type)
    {
        switch (shopType)
        {
            case UIShop.ShopType.Hat:
                currentSkin.DespawnHat();
                ChangeHat((HatType)type);
                break;

            case UIShop.ShopType.Pant:
                ChangePant((PantType)type);
                break;

            case UIShop.ShopType.Accessory:
                currentSkin.DespawnAccessory();
                ChangeAccessory((AccessoryType)type);
                break;

            case UIShop.ShopType.Skin:
                TakeOffClothes();
                skinType = (SkinType)type;
                WearClothes();
                break;
            case UIShop.ShopType.Weapon:
                currentSkin.DespawnWeapon();
                ChangeWeapon((WeaponType)type);
                break;
            default:
                break;
        }

    }

    //take cloth from data
    internal void OnTakeClothsData()
    {
        // take old cloth data
        skinType = UserData.Ins.playerSkin;
        weaponType = UserData.Ins.playerWeapon;
        hatType = UserData.Ins.playerHat;
        accessoryType = UserData.Ins.playerAccessory;
        pantType = UserData.Ins.playerPant;
    }
}

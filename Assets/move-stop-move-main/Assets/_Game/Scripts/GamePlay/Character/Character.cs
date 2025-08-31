using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : AbstractCharacter, IHit
{
    public const float TIME_DELAY_THROW = 0.4f;

    public const float ATT_RANGE = 5f;

    public const float MAX_SIZE = 4f;
    public const float MIN_SIZE = 1f;

    private string animName;

    [SerializeField] Transform indicatorPoint;
    protected TargetIndicator indicator;

    [SerializeField] GameObject targetMask;

    private List<Character> targets = new List<Character>();
    protected Character target;

    private Vector3 targetPoint;

    private int score;

    protected float size = 1;

    public int Score => score;
    public float Size => size;
    public bool IsDead { get; protected set; }

    public bool IsCanAttack => currentSkin.Weapon.IsCanAttack;

    //khoi tao trang thai
    public override void OnInit()
    {
        IsDead = false;
        score = 0;

        WearClothes();
        ClearTarget();

        indicator = SimplePool.Spawn<TargetIndicator>(PoolType.TargetIndicator);
        indicator.SetTarget(indicatorPoint);
    }

    public virtual void WearClothes()
    {

    }

    public virtual void TakeOffClothes()
    {
        currentSkin?.OnDespawn();
        SimplePool.Despawn(currentSkin);
    }

    //bat dau trang tai tan cong -> chuyen anim
    public override void OnAttack()
    {
        target = GetTargetInRange();

        if (IsCanAttack && target != null && !target.IsDead)
        {
            targetPoint = target.TF.position;
            TF.LookAt(targetPoint + (TF.position.y - targetPoint.y) * Vector3.up);
            ChangeAnim(Constant.ANIM_ATTACK);
        }

    }

    //ban ra vien dan
    public void Throw()
    {
        currentSkin.Weapon.Throw(this, targetPoint, size);
    }

    //chuyen trang thai die
    public override void OnDeath()
    {
        ChangeAnim(Constant.ANIM_DIE);
        LevelManager.Ins.CharecterDeath(this);
    }

    //dung lai
    public override void OnMoveStop()
    {

    }

    //ket thuc viec su dung nhan vat nay
    public override void OnDespawn()
    {
        //tra ve tat ca nhung object pool
        TakeOffClothes();
        SimplePool.Despawn(indicator);
    }

    //va cham voi dan
    public void OnHit(UnityAction hitAction)
    {
        if (!IsDead)
        {
            IsDead = true;
            hitAction?.Invoke();
            OnDeath();
        }
    }

    //danh dau muc tieu
    public void SetTargetMask(bool active)
    {
        targetMask.SetActive(active);
    }


    public void AddScore(int amount = 1)
    {
        SetScore(score + amount);
        ParticlePool.Play(Utilities.RandomInMember(ParticleType.LevelUp_1, ParticleType.LevelUp_2, ParticleType.LevelUp_3), TF.position);
    }

    public void SetScore(int score)
    {
        this.score = score > 0 ? score : 0;
        indicator.SetScore(this.score);
        SetSize(1 + this.score * 0.1f);
    }

    //thay doi kich thuoc
    protected virtual void SetSize(float size)
    {
        size = Mathf.Clamp(size, MIN_SIZE, MAX_SIZE);
        this.size = size;
        TF.localScale = size * Vector3.one;
    }

    //them muc tieu
    public virtual void AddTarget(Character target)
    {
        targets.Add(target);
    }

    //xoas muc tieu
    public virtual void RemoveTarget(Character target)
    {
        targets.Remove(target);
        this.target = null;
    }

    //tim muc tieu trong tam danh
    public Character GetTargetInRange()
    {
        Character closestTarget = null;
        float closestSqrDist = float.MaxValue;
        Vector3 myPos = TF.position;

        foreach (Character t in targets)
        {
            if (t == null || t == this || t.IsDead) continue;

            float range = ATT_RANGE * size + t.size;
            float sqrDist = (t.TF.position - myPos).sqrMagnitude;

            if (sqrDist <= range * range && sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closestTarget = t;
            }
        }

        return closestTarget;
    }


    public void ChangeAnim(string animName)
    {
        if (this.animName != animName)
        {
            currentSkin.Anim.ResetTrigger(this.animName);
            this.animName = animName;
            currentSkin.Anim.SetTrigger(this.animName);
        }
    }


    [SerializeField] protected Skin currentSkin;

    public void ChangeWeapon(WeaponType weaponType)
    {
        currentSkin.ChangeWeapon(weaponType);
    }

    public void ChangeSkin(SkinType skinType)
    {
        currentSkin = SimplePool.Spawn<Skin>((PoolType)skinType, TF);
    }

    public void ChangeAccessory(AccessoryType accessoryType)
    {
        currentSkin.ChangeAccessory(accessoryType);
    }

    public void ChangeHat(HatType hatType)
    {
        currentSkin.ChangeHat(hatType);
    }

    public void ChangePant(PantType pantType)
    {
        currentSkin.ChangePant(pantType);
    }

    protected void ClearTarget()
    {
        targets.Clear();
    }
}

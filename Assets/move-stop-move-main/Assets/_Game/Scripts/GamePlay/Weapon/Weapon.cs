using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : GameUnit
{
    public const float TIME_WEAPON_RELOAD = 0.5f;

    [SerializeField] GameObject child;
    [SerializeField] BulletType bulletType;

    public bool IsCanAttack => child.activeSelf;

    public void SetActive(bool active)
    {
        child.SetActive(active);
    }

    private void OnEnable()
    {
        SetActive(true);
    }

    public void Throw(Character character, Vector3 target, float size)
    {
        Bullet bullet = SimplePool.Spawn<Bullet>((PoolType)bulletType, TF.position, Quaternion.identity);
        bullet.OnInit(character, target, size);
        bullet.TF.localScale = size * Vector3.one;
        SetActive(false);

        Invoke(nameof(OnEnable), TIME_WEAPON_RELOAD);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRotate : Bullet
{
    public const float TIME_ALIVE = 1f;
    [SerializeField] Transform child;

    private Coroutine coroutine;

    public override void OnInit(Character character, Vector3 target, float size)
    {
        base.OnInit(character, target, size);
        if (coroutine != null )
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(AutoDespawnCoroutine(TIME_ALIVE * size));
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            TF.Translate(TF.forward * moveSpeed * Time.deltaTime, Space.World);
            child.Rotate(Vector3.up * -6, Space.Self);
        }
    }

    private IEnumerator AutoDespawnCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        OnDespawn();
    }

    protected override void OnStop()
    {
        base.OnStop();
        isRunning = false;
    }
}

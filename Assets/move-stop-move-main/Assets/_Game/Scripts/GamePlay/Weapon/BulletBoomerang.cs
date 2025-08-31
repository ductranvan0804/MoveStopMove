using System.Collections;
using UnityEngine;

public class BulletBoomerang : Bullet
{
    private enum State { Forward, Return }
    private State state = State.Forward;

    private Vector3 targetPoint;
    private bool hasHit = false;

    [SerializeField] Transform child;
    [SerializeField] float returnDelay = 0.5f;
    [SerializeField] float closeDistance = 0.5f;

    public override void OnInit(Character character, Vector3 target, float size)
    {
        base.OnInit(character, target, size);
        // Tính ?i?m bay ra xa theo h??ng target
        Vector3 dir = (target - character.TF.position).normalized;
        targetPoint = character.TF.position + dir * (Character.ATT_RANGE + 1f) * size;
        state = State.Forward;
        hasHit = false;
    }

    void Update()
    {
        if (!isRunning) return;

        switch (state)
        {
            case State.Forward:
                TF.position = Vector3.MoveTowards(TF.position, targetPoint, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(TF.position, targetPoint) < 0.1f && !hasHit)
                {
                    state = State.Return;
                }
                break;

            case State.Return:
                if (character == null || character.IsDead)
                {
                    OnDespawn();
                    return;
                }

                TF.position = Vector3.MoveTowards(TF.position, character.TF.position, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(TF.position, character.TF.position) < closeDistance)
                {
                    OnDespawn();
                }
                break;
        }

        child.Rotate(Vector3.up * -6, Space.Self);
    }

    protected override void OnStop()
    {
        base.OnStop();
        state = State.Return;
    }
}

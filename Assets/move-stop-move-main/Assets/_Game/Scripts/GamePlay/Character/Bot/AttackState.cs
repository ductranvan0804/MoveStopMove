using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState<Bot>
{
    public void OnEnter(Bot t)
    {
        t.OnMoveStop();
        t.OnAttack();
        if (t.IsCanAttack)
        {
            t.StartTimer(() =>
            {
                t.Throw();
                t.StartTimer(() =>
                {
                    t.ChangeState(Utilities.Chance(50, 100) ? new IdleState() : new PatrolState());
                }, Character.TIME_DELAY_THROW);
            }, Character.TIME_DELAY_THROW);

        }
    }

    public void OnExecute(Bot t)
    {
        
    }

    public void OnExit(Bot t)
    {
    }

}

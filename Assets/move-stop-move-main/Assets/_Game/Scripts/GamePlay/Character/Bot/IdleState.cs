using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState<Bot>
{
    public void OnEnter(Bot t)
    {
        t.OnMoveStop();
        t.StartTimer(() => t.ChangeState(new PatrolState()), Random.Range(0f, 2f));
    }

    public void OnExecute(Bot t)
    {
        
    }

    public void OnExit(Bot t)
    {

    }

}

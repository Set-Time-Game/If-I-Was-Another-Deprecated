using UnityEngine;

public class Gobba : Enemy
{
    private Transform _transform;

    protected override void Awake()
    {
        base.Awake();
        _transform = transform;
        RandomPatrolling = StartCoroutine(RandomPatrol());
    }
}
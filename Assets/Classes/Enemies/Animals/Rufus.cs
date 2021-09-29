using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Classes.Enemies;
using DefaultNamespace;
using UnityEngine;
using static Classes.Characters.Slime.Character;
using Debug = UnityEngine.Debug;

public class Rufus : Enemy
{
    private static readonly int attack = Animator.StringToHash("Attack");
    private static readonly int stopAttack = Animator.StringToHash("StopAttack");

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private EnemyCollector collector;

    protected override void Awake()
    {
        base.Awake();
        CurrentState = States.None;
        Immortal = false;
        RandomPatrolling = StartCoroutine(RandomPatrol());
    }

    protected override void Update()
    {
        switch (CurrentState)
        {
            case States.Attacking:
                if (Hunting != null)
                {
                    StopCoroutine(Hunting);
                    Hunting = null;
                }

                if (RandomPatrolling != null)
                {
                    StopCoroutine(RandomPatrolling);
                    RandomPatrolling = null;
                }

                targetSetter.target = target;
                
                break;

            case States.None:
                if (Vector2.Distance(transform.position, target.position) <= .4f && Enemies.Count >= 1)
                {
                    Debug.Log("Change to attack");
                    CurrentState = States.Attacking;
                    Flip(ref sr, target.localPosition, ref anim, fixd: true);
                    
                    anim.StopPlayback();
                    anim.SetTrigger(attack);
                }
                else if (Vector2.Distance(transform.position, target.position) > .4f && Enemies.Count >= 1)
                {
                    Hunting ??= StartCoroutine(Hunt());
                    if (RandomPatrolling != null)
                    {
                        StopCoroutine(RandomPatrolling);
                        RandomPatrolling = null;
                    }
                }
                else
                {
                    RandomPatrolling ??= StartCoroutine(RandomPatrol());
                    if (Hunting != null)
                    {
                        StopCoroutine(Hunting);
                        Hunting = null;
                    }
                }

                break;
        }
    }
    
    private IEnumerator StopHunt(Collider2D collision)
    {
        yield return new WaitForSeconds(5);
        if (Vector2.Distance(transform.position, collision.transform.position) <= 2) yield break;

        collector.OnTriggerExit2D(collision);
    }

    private void Attack()
    {
        if (Enemies.Count <= 0) return;

        var targetPosition = target.localPosition;
        Flip(ref sr, targetPosition, ref anim, fixd: true);

        var targets = new LinkedList<IDamageable>();
        var hitEnemies = new Collider2D[Enemies.Count + 1];
        var size = Physics2D.OverlapCircleNonAlloc(
            target.transform.position,
            defaultAtkRange,
            hitEnemies,
            enemyLayer);

        if (size <= 0) return;

        foreach (var i in hitEnemies)
            if (i.TryGetComponent<IDamageable>(out var dmg) 
                && !i.TryGetComponent<Rufus>(out _) 
                && !targets.Contains(dmg))
                targets.AddLast(dmg);


        if (targets.Count <= 0) return;
        ClosestFrom(targets, transform.position).TakeDamage(defaultDamage);
    }

    private void StopAttack()
    {
        anim.StopPlayback();
        anim.SetTrigger(stopAttack);

        CurrentState = States.None;
        aiPath.canMove = true;

        if (Enemies.Count >= 1)
        {
            if (Vector2.Distance(transform.position, target.position) > .4f)
            {
                Hunting ??= StartCoroutine(Hunt());
            }
        }
        else
        {
            RandomPatrolling ??= StartCoroutine(RandomPatrol());
        }
    }
}
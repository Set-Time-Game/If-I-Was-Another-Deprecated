using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Classes.Enemies;
using Pathfinding;
using UnityEngine;
using static Classes.Characters.Slime.Character;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    public delegate void OnDieDelegate();

    protected static readonly int speed = Animator.StringToHash("Speed");

    [Space] [SerializeField] protected float defaultSpeed;
    [SerializeField] protected float defaultArmor;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float defaultDamage;
    [SerializeField] protected float defaultAtkRate;
    [SerializeField] protected float defaultAtkRange;

    [Space] [SerializeField] public Transform target;

    [SerializeField] protected Animator anim;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] public AIDestinationSetter targetSetter;
    [SerializeField] protected AIPath aiPath;

    protected States CurrentState;

    public LinkedList<IDamageable> Enemies = new LinkedList<IDamageable>();
    public Coroutine Hunting;
    public Coroutine RandomPatrolling;

    public float Health { get; protected set; }
    public bool Immortal { get; protected set; }

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(target.position, defaultAtkRange);
    }
    
    protected virtual void Awake()
    {
        Health = maxHealth;
        OnDie += Death;
    }

    protected virtual void Update()
    {
        var pos = transform.position;
        if (CurrentState != States.None) return;

        var targetPosition = target.localPosition;

        if (targetPosition.x == 0 && targetPosition.y == 0) return;
        if (Vector2.Distance(pos, targetPosition) <= .4f) anim.SetFloat(speed, 0);
    }

    protected virtual void LateUpdate()
    {
        var targetPosition = target.localPosition;
        Flip(ref sr, targetPosition, ref anim, fixd: true);
        
        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
        
        anim.SetFloat(speed, aiPath.canMove ? 1 : 0);
        
        if (CurrentState == States.None)
            aiPath.canMove = Vector2.Distance(transform.position, target.position) >= .4f;
    }

    public Transform Transform => transform;

    public virtual void TakeDamage(float damage = 0)
    {
        if (Health >= 1 && damage > 0)
        {
            damage *= 1 - defaultArmor;
            Health -= damage > 0 ? damage : 0;
        }

        if (!Immortal && Health <= 0)
            OnDie?.Invoke();
        
    }

    public event OnDieDelegate OnDie;

    protected virtual void Death()
    {
        Instantiate(this, new Vector3(), new Quaternion());
        Destroy(gameObject);
    }

    //TODO: change to OnTriggerStay2D
    public virtual IEnumerator Hunt()
    {
        var timer = new WaitForFixedUpdate();

        yield return null;
        
        while (true)
        {
            target.position = ClosestFrom(Enemies, transform.position).Transform.position;

            yield return timer;
        }
    }

    public virtual IEnumerator RandomPatrol()
    {
        var timer = new WaitForSeconds(10);
        
        yield return null;

        while (true)
        {
            int x, y;
            
            do
            {
                x = Random.Range(-1, 1);
                y = Random.Range(-1, 1);
                
            } while (x == 0 || y == 0);

            var position = transform.position;
            target.position = new Vector3(position.x + x, position.y + y, 0);

            yield return timer;

            position = transform.position;
            var isHorizontal = Mathf.Abs(x) >= Mathf.Abs(y);
            const float dist = .3000000f;
            target.position = new Vector3(
                position.x + (isHorizontal 
                    ? (x > 0 
                        ? dist 
                        : -dist) 
                    : 0),
                position.y + (!isHorizontal 
                    ? (y > 0 
                        ? dist 
                        : -dist) 
                    : 0),
                0);

            yield return timer;
        }
    }
}
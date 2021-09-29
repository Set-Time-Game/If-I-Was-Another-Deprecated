using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Classes.Enemies;
using Classes.Tiles;
using Classes.UI;
using Scripts.characters;
using UI;
using UnityEngine;
using static Classes.Characters.Slime.Character.AttackType;

namespace Classes.Characters.Slime
{
    public class Character : MonoBehaviour, IDamageable
    {
        private StaminaBar _staminaBar;
        private HealthBar _healthBar;
        
        [Flags]
        public enum AttackType
        {
            Melee = 0,
            Range = 1,
            Mage = 2
        }

        [Flags]
        public enum States
        {
            None = 0,
            Attacking = 1,
            Rolling = 2
        }

        private static readonly Dictionary<int, Coroutine> Regenerations = new Dictionary<int, Coroutine>();

        public static Character Singleton { get; private set; }

        private void Awake()
        {
            _staminaBar = StaminaBar.Singleton;
            _healthBar = HealthBar.Singleton;
            
            Health = maxHealth;
            Alt = maxAlt;
            Stamina = maxStamina;
            CurrentState = States.None;
            CurrentAttackType = Melee;
            atkJoystick.Attacking += StartAttack;

            TagWhiteList = new[] {"Enemy", "ResourceSource", "Obstacle"};
            resourceSourcesList = new LinkedList<IGenerable>();
            enemiesList = new LinkedList<IDamageable>();

            Singleton = this;
        }

        private void FixedUpdate()
        {
            if (resourceSourcesList == null || resourceSourcesList.Count <= 0 || !resourceSourcesList.Any()) return;

            var minTarget = ClosestFrom(resourceSourcesList.ToArray(), transform.position);

            minTarget?.SetHighlight(true);
            _closest ??= minTarget;

            if (_closest == minTarget) return;
            _closest.SetHighlight(false);
            _closest = minTarget;
        }

        [Conditional("UNITY_EDITOR")]
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(attackPoint.tr.position, AttackRange);
        }

        public virtual void TakeDamage(float damage = 0)
        {
            if (Health > 0 && damage > 0)
            {
                damage *= 1 - Armor;
                damage = damage > 0 ? damage : 0;


                Health -= damage;

                OnTakeDamage?.Invoke(damage);
            }
            else
            {
                Instantiate(this, new Vector3(), new Quaternion());
                Destroy(this);
                OnDie?.Invoke();
                return;
            }

            var percent = (float) Math.Round(Health / maxHealth, 4);
            _healthBar.SetPointer(percent);
            anim.SetFloat(Health1, percent <= .25f ? .25f : percent <= .50f ? .50f : percent <= .75f ? .75f : 1);
        }

        public void ChangeState(States newState)
        {
            CurrentState = newState;

            switch (newState)
            {
                case States.Rolling:
                    Stamina -= 20;
                    var percent = Stamina / maxStamina;
                    _staminaBar.SetPointer(percent);

                    if (Regenerations.ContainsKey(1))
                        StopCoroutine(Regenerations[1]);

                    Regenerations[1] = StartCoroutine(Regenerate(1));
                    break;
            }
        }

        private IEnumerator Regenerate(int attribute)
        {
            // 0 - HP
            // 1 - ST
            var timer = new WaitForFixedUpdate();

            yield return new WaitForSeconds(3);
            
            switch (attribute)
            {
                case 1:
                    while (true)
                    {
                        yield return timer;

                        Stamina += 0.03f;
                        //var percent = (float) Math.Round(Stamina / maxStamina, 1);
                        var percent = Stamina / maxStamina;

                        if (percent >= 100)
                        {
                            Stamina = maxStamina;
                            percent = 100;
                            _staminaBar.SetPointer(percent);

                            break;
                        }

                        _staminaBar.SetPointer(percent);
                    }

                    break;
            }
        }

        public static T ClosestFrom<T>(IEnumerable<T> list, Vector2 target)
            where T : IEntity
        {
            var generables = list as T[] ?? list.ToArray();
            var minValue = Vector2.Distance(target, generables.First().Transform.position);
            var closest = generables.Where(res => Vector2.Distance(target, res.Transform.position) < minValue).ToArray();

            return closest.Length >= 1 ? closest.First() : generables.First();
        }

        public static void Flip(ref SpriteRenderer spriteRenderer, Vector2 direction, ref Animator anim,
            bool normalized = true, bool fixd = false)
        {
            var isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
            var flipX = spriteRenderer.flipX;

            flipX = isHorizontal switch
            {
                false when flipX => false,
                true => direction.x < 0,
                _ => false
            };
            spriteRenderer.flipX = flipX;

            anim.SetFloat(Horizontal,
                normalized
                    ? isHorizontal
                        ? direction.x > 0
                            ? 1
                            : -1
                        : 0
                    : fixd
                        ? 0
                        : direction.x);
            anim.SetFloat(Vertical,
                normalized
                    ? !isHorizontal
                        ? direction.y > 0
                            ? 1
                            : -1
                        : 0
                    : fixd
                        ? 0
                        : direction.y);
        }

        #region private

        [SerializeField] private float maxHealth;
        [SerializeField] private float maxStamina;
        [SerializeField] private float maxAlt;

        [Space] [SerializeField] private float defaultSpeed;
        [SerializeField] private float defaultArmor;
        [SerializeField] private float defaultDamage;
        [SerializeField] private float defaultAtkRange;

        [Space] [SerializeField] public Rigidbody2D rb;

        private IGenerable _closest;
        //private Dictionary<string, DateTime> _cooldowns;

        private static readonly int StartAttack1 = Animator.StringToHash("StartAttack");
        private static readonly int StopAttack1 = Animator.StringToHash("StopAttack");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Health1 = Animator.StringToHash("Health");

        #endregion

        #region public stats get-set`ters

        public LinkedList<IDamageable> enemiesList;
        public LinkedList<IGenerable> resourceSourcesList;
        public IReadOnlyList<string> TagWhiteList;

        public float Health { get; private set; }
        public float Stamina { get; private set; }
        public float Alt { get; private set; }

        public float Armor => defaultArmor;
        public float Damage => defaultDamage;
        public float AttackRange => defaultAtkRange;
        public float Speed => defaultSpeed;

        public States CurrentState { get; private set; }
        public AttackType CurrentAttackType { get; private set; }
        public Transform Transform => transform;

        #endregion

        #region public fields

        [Space] public AttackPoint attackPoint;
        public SpriteRenderer sr;
        public LayerMask enemyLayer;
        public Bullet bullet;
        public AttackJoystick atkJoystick;
        public Movement moveJoystick;

        [Space] public Animator anim;
        private static readonly int AttackMode = Animator.StringToHash("AttackMode");
        private static readonly int Rolling = Animator.StringToHash("StopRolling");

        public delegate void OnWeaponSwapDelegate(AttackType type);

        public event OnWeaponSwapDelegate OnWeaponSwap;

        public delegate void OnTakeDamageDelegate(float damage);

        public event OnTakeDamageDelegate OnTakeDamage;

        public delegate void OnDieDelegate();

        public event OnDieDelegate OnDie;

        #endregion

        #region Attack

        private void StartAttack()
        {
            CurrentState = States.Attacking;

            Flip(ref sr, attackPoint.tr.localPosition, ref anim, fixd: true);

            anim.StopPlayback();
            anim.SetTrigger(StartAttack1);
        }

        private void StopAttack()
        {
            if (moveJoystick.joystick.Direction.y != 0 || moveJoystick.joystick.Direction.x != 0)
                Flip(ref sr, moveJoystick.joystick.Direction, ref anim, fixd: true);

            anim.StopPlayback();
            anim.SetTrigger(StopAttack1);
            CurrentState = States.None;
        }

        public void Attack()
        {
            switch (CurrentAttackType)
            {
                case Range:
                {
                    RangeAttack();

                    break;
                }
                case Melee:
                {
                    if (enemiesList.Count <= 0) break;
                    
                    MeleeAttack();

                    break;
                }
                case Mage:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        private void RangeAttack()
        {
            var bulletInstance = Instantiate(bullet, transform.position, attackPoint.tr.rotation);
            bulletInstance.rb.AddForce(-bulletInstance.transform.right * bulletInstance.Speed * Time.fixedDeltaTime,
                ForceMode2D.Impulse);
            var percent = (float) Math.Round(Health / maxHealth, 4);
            bulletInstance.SetHealthPercent(percent <= .25f ? .25f : percent <= .50f ? .50f : percent <= .75f ? .75f : 1);
        }
        private void MeleeAttack()
        {
            var hitEnemies = new Collider2D[enemiesList.Count];
            var size = Physics2D.OverlapCircleNonAlloc(attackPoint.tr.position, defaultAtkRange,
                hitEnemies, enemyLayer);

            if (size <= 0) return;

            var enemies = new LinkedList<Enemy>();
            foreach (var e in hitEnemies.Select(x => x.GetComponent<Enemy>()))
                if (e != null && !enemies.Contains(e))
                    enemies.AddLast(e);

            var closest = ClosestFrom(enemies, transform.position);
            closest.TakeDamage(Damage);
            enemies.Remove(closest);

            foreach (var e in enemies)
                e.TakeDamage(Damage * .75f);
        }

        #region AttackType Swap

        public void AttackTypeSwap(AttackType type)
        {
            if (CurrentAttackType == type)
            {
                SwapMelee(type);
                return;
            }

            CurrentAttackType = type;
            anim.SetFloat(AttackMode, (float) type);
            OnWeaponSwap?.Invoke(CurrentAttackType);
        }

        public void SwapMelee(AttackType previous)
        {
            if (CurrentAttackType == Melee)
            {
                switch (previous)
                {
                    case Melee:
                        break;
                    //case Range:
                    //case Mage:
                    default:
                        AttackTypeSwap(previous);
                        break;
                }

                return;
            }

            CurrentAttackType = Melee;
            anim.SetFloat(AttackMode, (float) Melee);
            OnWeaponSwap?.Invoke(CurrentAttackType);
        }

        #endregion
    }
}
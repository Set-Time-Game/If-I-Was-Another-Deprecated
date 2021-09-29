using System;
using System.Collections;
using System.Linq;
using Classes.Enemies;
using Classes.UI;
using UnityEngine;

public class Bullet : MonoBehaviour, IProjectiles
{
    private static readonly int Health = Animator.StringToHash("Health");
    public Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private int health;
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private EnemyTag[] enemyTags;
    [SerializeField] private string[] destroyTags;
    private Coroutine _move;

    private void Awake()
    {
        LifeTime = lifeTime;
        Speed = speed;
        Damage = damage;

        _move = StartCoroutine(Moving());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTags.All(x => !collision.CompareTag(x.name))) return;
        var enemyTag = enemyTags.First(x => collision.CompareTag(x.name));
        health -= enemyTag.hp;

        if (health <= 0)
        {
            StopCoroutine(_move);
            Destroy(gameObject);
        }

        if (collision.TryGetComponent<IDamageable>(out var target)) target.TakeDamage(damage);

        if (destroyTags.Contains(collision.tag)) Destroy(collision.gameObject);
    }

    public float LifeTime { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }

    public void SetHealthPercent(float percent)
    {
        anim.SetFloat(Health, percent > 0 ? percent : 0);
    }


    private IEnumerator Moving()
    {
        yield return new WaitForSeconds(LifeTime);

        Destroy(gameObject);
    }

    [Serializable]
    private struct EnemyTag
    {
        public string name;
        public int hp;
    }
}
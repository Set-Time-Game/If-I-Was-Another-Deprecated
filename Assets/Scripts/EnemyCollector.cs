using Classes.Enemies;
using UnityEngine;

namespace DefaultNamespace
{
    public class EnemyCollector : MonoBehaviour
    {
        [SerializeField] private Enemy parent;
        
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            if (!collision.TryGetComponent<IDamageable>(out var character)) return;
            parent.Enemies.AddLast(character);
            parent.targetSetter.target = collision.transform;
            parent.Hunting ??= StartCoroutine(parent.Hunt());

            if (parent.RandomPatrolling == null) return;
            StopCoroutine(parent.RandomPatrolling);
            parent.RandomPatrolling = null;
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            if (!collision.TryGetComponent<IDamageable>(out var character)) return;
            if (parent.Enemies.Contains(character))
                parent.Enemies.Remove(character);

            parent.targetSetter.target = parent.target;
            parent.RandomPatrolling ??= StartCoroutine(parent.RandomPatrol());

            if (parent.Hunting == null) return;
            StopCoroutine(parent.Hunting);
            parent.Hunting = null;
        }
    }
}
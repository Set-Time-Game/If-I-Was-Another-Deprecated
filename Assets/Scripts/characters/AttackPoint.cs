using System;
using System.Linq;
using Classes.Characters.Slime;
using Classes.UI;
using UnityEngine;

namespace Scripts.characters
{
    public class AttackPoint : MonoBehaviour
    {
        public Transform tr;
        public float coefficient = 0.25f;

        [SerializeField] private Character player;
        [SerializeField] private AttackJoystick joystick;

        public void Update()
        {
            if (player.CurrentState != Character.States.None) return;

            var verticalPosition = joystick.Direction.y;
            var horizontalPosition = joystick.Direction.x;
            var yNormalized = verticalPosition > 0 ? 1 : -1;
            var xNormalized = horizontalPosition > 0 ? 1 : -1;
            var isHorizontal = Math.Abs(horizontalPosition) > Math.Abs(verticalPosition);

            if (verticalPosition != 0 && horizontalPosition != 0)
                tr.localPosition = player.CurrentAttackType switch
                {
                    Character.AttackType.Melee => new Vector3
                    (isHorizontal ? coefficient * xNormalized : 0,
                        !isHorizontal ? coefficient * yNormalized : 0, 0),

                    Character.AttackType.Range => new Vector3(coefficient * horizontalPosition,
                        coefficient * verticalPosition, 0),

                    _ => tr.localPosition
                };

            Rotate(player.transform, tr);
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!player.TagWhiteList.Contains(collision.tag)) return;
            if (collision.TryGetComponent<Enemy>(out var enemy) && !player.enemiesList.Contains(enemy))
                player.enemiesList.AddLast(enemy);
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (!player.TagWhiteList.Contains(collision.tag)) return;
            if (collision.TryGetComponent<Enemy>(out var enemy))
                player.enemiesList.Remove(enemy);
        }

        private static void Rotate(Transform pointer, Transform target)
        {
            var diff = pointer.localPosition - target.position;
            diff.Normalize();

            target.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        }
    }
}
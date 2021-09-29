using System;
using System.Collections;
using Classes.Characters.Slime;
using UnityEngine;
using UnityEngine.UI;
using static Classes.Characters.Slime.Character;

namespace Scripts.characters
{
    public class Movement : MonoBehaviour
    {
        private static readonly int StartRolling = Animator.StringToHash("StartRolling");
        private static readonly int StopRolling1 = Animator.StringToHash("StopRolling");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Speed = Animator.StringToHash("Speed");

        public Animator anim;
        public Transform tr;
        public SpriteRenderer sr;
        public AnimationCurve RollingMultiplier;
        public Button RollButton;

        public VariableJoystick joystick;
        public Character player;

        private Coroutine Rolling;

        private TimeSpan RollCD = TimeSpan.FromSeconds(3);
        private DateTime RollUsage;

        private void Start()
        {
            if (joystick == null)
                joystick = VariableJoystick.Singleton;
            if (player == null)
                player = Singleton;
        }

        private void FixedUpdate()
        {
            if (player.Stamina < 21)
                RollButton.interactable = false;
            
            if (player.CurrentState != States.None) return;

            var direction = joystick.Direction;
            if (joystick.Direction.y != 0 || joystick.Direction.x != 0)
            {
                Flip(ref sr, direction, ref anim, false);
                anim.SetFloat(Speed, 1);
/*
                var absDir = new Vector2(Math.Abs(direction.x), Math.Abs(direction.y));
                var isHorizontal = absDir.x > absDir.y;
                var sum = absDir.x + absDir.y;
                var replace = new Vector2(1 - absDir.y, 1 - absDir.x);

                direction.x = sum != 1.0f
                    ? isHorizontal
                        ? direction.x
                        : direction.x > 0
                            ? replace.x
                            : -replace.x
                    : direction.x;

                direction.y = sum != 1.0f
                    ? isHorizontal
                        ? direction.y > 0
                            ? replace.y
                            : -replace.y
                        : direction.y
                    : direction.y;
*/
            }
            else
            {
                anim.SetFloat(Speed, 0);
            }

            tr.Translate(new Vector3(
                direction.x * player.Speed * Time.fixedDeltaTime,
                direction.y * player.Speed * Time.fixedDeltaTime,
                0
            ));
        }

        public void Roll()
        {
            if (Singleton.CurrentState != States.None || Rolling != null) return;
            Rolling = StartCoroutine(Roll(new Vector2(anim.GetFloat(Horizontal), anim.GetFloat(Vertical))));
        }

        private IEnumerator Roll(Vector2 direction)
        {
            RollButton.interactable = false;
            RollUsage = DateTime.UtcNow;
            
            player.ChangeState(States.Rolling);
            anim.SetTrigger(StartRolling);

            var timer = new WaitForFixedUpdate();

            while (true)
            {
                const float additionMultiplier = 1.25f;
                var multiplier = RollingMultiplier.Evaluate(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                var pos = transform.position;
                var x = multiplier * ((direction.x != 0 ? direction.x : 0) * Time.fixedDeltaTime);
                var y = multiplier * ((direction.y != 0 ? direction.y : 0) * Time.fixedDeltaTime);

                player.rb.MovePosition(new Vector2(pos.x + (x * additionMultiplier), pos.y + (y * additionMultiplier)));

                yield return timer;
            }
        }

        private IEnumerator StopRolling()
        {
            if (Rolling != null)
            {
                StopCoroutine(Rolling);
                Rolling = null;
            }

            anim.SetTrigger(StopRolling1);
            player.ChangeState(States.None);

            yield return new WaitUntil(() => DateTime.UtcNow - RollUsage >= RollCD);
            
            if (player.Stamina < 21)
                yield return new WaitUntil(() => player.Stamina >= 21);
            
            RollButton.interactable = true;
        }
    }
}
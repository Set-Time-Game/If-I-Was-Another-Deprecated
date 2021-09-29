using System.Collections.Generic;
using System.Linq;
using Classes.Characters.Slime;
using Classes.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WeaponChanger : MonoBehaviour
    {
        public Image image;
        public Character.AttackType buttonType;
        public List<WeaponChanger> otherButtons;

        [SerializeField] private Character player;
        [SerializeField] private AttackJoystick attackJoystick;
        private Buttons.Button _defaultButton;
        private Buttons.Button _meleeButton;

        public Sprite DefaultSprite { get; private set; }

        private void Start()
        {
            _defaultButton = Buttons.singletone.list.First(x => x.type == buttonType);
            _meleeButton = Buttons.singletone.list.First(x => x.type == Character.AttackType.Melee);
            player = Character.Singleton;
            //_attackJoystick = AttackJoystick.singletone;
            player.OnWeaponSwap += AttackSwapped;

            DefaultSprite = _defaultButton.button;
        }

        private void AttackSwapped(Character.AttackType type)
        {
            if (type == buttonType)
            {
                image.sprite = _meleeButton.button;
                attackJoystick.backgroundImage.sprite = _defaultButton.background;
                attackJoystick.handleImage.sprite = _defaultButton.handle;

                foreach (var button in otherButtons) button.image.sprite = button.DefaultSprite;
            }
            else if (type == Character.AttackType.Melee)
            {
                image.sprite = DefaultSprite;
                attackJoystick.backgroundImage.sprite = _meleeButton.background;
                attackJoystick.handleImage.sprite = _meleeButton.handle;
            }
        }

        public void Change()
        {
            player.AttackTypeSwap(buttonType);
        }
    }
}
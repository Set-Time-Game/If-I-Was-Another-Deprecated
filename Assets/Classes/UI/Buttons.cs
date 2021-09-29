using System;
using System.Collections.Generic;
using Classes.Characters.Slime;
using UnityEngine;

namespace Classes.UI
{
    public class Buttons : MonoBehaviour
    {
        public List<Button> list = new List<Button>();
        public static Buttons singletone { get; private set; }

        private void Awake()
        {
            singletone = this;
        }

        [Serializable]
        public struct Button
        {
            public Character.AttackType type;
            public Sprite background;
            public Sprite handle;
            public Sprite button;
        }
    }
}
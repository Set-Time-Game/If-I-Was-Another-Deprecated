using System;
using Classes.Characters.Slime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Classes.UI
{
    public class AttackJoystick : Joystick
    {
        public delegate void Attack();

        private static float _scaler;

        public Image backgroundImage;
        public Image handleImage;
        public RectTransform rectTransform;

        [SerializeField] private float moveThreshold = 1;
        [SerializeField] private JoystickType joystickType = JoystickType.Fixed;
        [SerializeField] private Vector2 fixedPosition = Vector2.zero;

        private int _prefs;

        public float MoveThreshold
        {
            get => moveThreshold;
            set => moveThreshold = Mathf.Abs(value);
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            backgroundImage = background.GetComponent<Image>();
            handleImage = handle.GetComponent<Image>();
            canvas = GetComponentInParent<Canvas>();
        }

        protected override void Start()
        {
            //_prefs = PlayerPrefs.GetInt("JoystickScale");
            //ChangeScale(8);

            //base.Start();
            HandleRange = handleRange;
            DeadZone = deadZone;
            fixedPosition = background.anchoredPosition;
            SetMode(joystickType);
        }

        public event Attack Attacking;

        private void SetMode(JoystickType type)
        {
            joystickType = type;
            if (type == JoystickType.Fixed)
            {
                background.anchoredPosition = fixedPosition;
                background.gameObject.SetActive(true);
            }
            else
            {
                background.gameObject.SetActive(false);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (joystickType != JoystickType.Fixed)
            {
                background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
                background.gameObject.SetActive(true);
            }

            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (joystickType != JoystickType.Fixed)
                background.gameObject.SetActive(false);

            base.OnPointerUp(eventData);

            var player = Character.Singleton;
            if (player.CurrentState != Character.States.None) return;

            //player.attackPoint.RotateToJoystick();
            Attacking?.Invoke();
        }

        protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera camera)
        {
            if (joystickType == JoystickType.Dynamic && magnitude > moveThreshold)
            {
                var difference = normalised * (magnitude - moveThreshold) * radius;
                background.anchoredPosition += difference;
            }

            base.HandleInput(magnitude, normalised, radius, camera);
        }

        public void ChangeScale(float sc)
        {
            _scaler = sc > 0 ? sc : _prefs;
            var sz = Screen.width / _scaler;
            //background.anchoredPosition = new Vector3(sz, sz, 0);
            background.localScale = new Vector3(sz, sz, 0);
        }

        public void Save()
        {
            if (Math.Abs(_prefs - _scaler) == 0) return;
            _prefs = (int) _scaler;
            PlayerPrefs.SetInt("JoystickScale", (int) _scaler);
        }
    }
}
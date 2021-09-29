using System.Collections;
using Classes.Characters.Slime;
using Classes.UI;
using UnityEngine;

public class SkillCombiner : MonoBehaviour
{
    [SerializeField] private string combo;
    [SerializeField] private Character player;
    [SerializeField] private GameObject buttons;
    [SerializeField] private RectTransform rect;
    [SerializeField] private AttackJoystick attackJoystick;
    private readonly WaitForSeconds wipeTimeout = new WaitForSeconds(30);

    private Coroutine _wiperCoroutine;
    public static SkillCombiner singletone { get; set; }

    private void Awake()
    {
        singletone = this;
    }
    //private Classes.UI.Buttons.Button _mageButton;

    private void Start()
    {
        //attackJoystick = AttackJoystick.singletone;

        player = Character.Singleton;
        player.OnWeaponSwap += MagePanelSwitch;

        if (player.CurrentAttackType == Character.AttackType.Mage)
        {
            buttons.SetActive(false);
            if (_wiperCoroutine != null)
                StopCoroutine(_wiperCoroutine);
        }
        else
        {
            _wiperCoroutine = StartCoroutine(ComboWiper());
        }

        //attackJoystick = AttackJoystick.singletone;
        rect.localScale = attackJoystick.rectTransform.localScale;
    }

    public void MagePanelSwitch(Character.AttackType type)
    {
        if (type != Character.AttackType.Mage)
        {
            if (buttons.activeSelf)
                buttons.SetActive(false);

            return;
        }

        if (!buttons.activeSelf)
            buttons.SetActive(true);

        //attackJoystick.handleImage.sprite = _mageButton.handle;
        //attackJoystick.backgroundImage.sprite = _mageButton.background;

        var scale = attackJoystick.rectTransform.localScale;
        rect.localScale = new Vector3(scale.x, scale.y, 0);
    }

    public void OnButton(string buttonName)
    {
        if (_wiperCoroutine != null)
            StopCoroutine(_wiperCoroutine);

        combo += $"{buttonName}, ";
        _wiperCoroutine = StartCoroutine(ComboWiper());
    }

    private IEnumerator ComboWiper()
    {
        while (true)
        {
            yield return wipeTimeout;
            combo = "";
        }
    }
}
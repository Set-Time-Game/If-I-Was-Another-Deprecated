using UnityEngine;
using UnityEngine.UI;

public class SliderSetter : MonoBehaviour
{
    public Slider sl;

    private void Awake()
    {
        sl.value = PlayerPrefs.GetInt(gameObject.name);
    }
}
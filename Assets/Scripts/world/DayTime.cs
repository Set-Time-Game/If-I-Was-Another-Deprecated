using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DayTime : MonoBehaviour
{
    private double _current;

    private float _end;
    private float _first;
    public Image img;
    public AnimationCurve timings;

    private IEnumerator Start()
    {
        _end = timings.keys.Last().time;
        _first = timings.keys.First().time;
        _current = -1;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            
            if (_current > _end)
                _current = _first;

            var value = timings.Evaluate((float) _current);
            var color = img.color;

            //color.a = value * 255;
            color.a = value;
            img.color = color;

            _current += 0.001;
        }
    }
}
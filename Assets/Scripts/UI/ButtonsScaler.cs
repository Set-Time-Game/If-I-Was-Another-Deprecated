using UnityEngine;

namespace UI
{
    public class ButtonsScaler : MonoBehaviour
    {
        public float scaler;
        public RectTransform rt;
        private int _prefs;

        /*private void Awake()
        {
            if (rt == null)
                rt = GetComponent<RectTransform>();

            _prefs = PlayerPrefs.GetInt("ButtonsScale");
            ChangeScale(_prefs);
        }*/

        public void ChangeScale(float scale = 0)
        {
            var sz = Screen.width / (scale != 0 ? scale : _prefs);

            if (scaler != 0)
                sz /= scaler;

            rt.localScale = new Vector3(sz, sz, 0);
        }

        public void Save(int scale)
        {
            if (_prefs == scale) return;
            _prefs = scale;
            PlayerPrefs.SetInt("ButtonsScale", scale);
        }
    }
}
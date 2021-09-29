using UnityEngine;

namespace UI
{
    public class BarsScaler : MonoBehaviour
    {
        private const int Prefs = 864;
        public RectTransform[] rt;

        /*private void Awake()
        {
            //_prefs = PlayerPrefs.GetInt("ButtonsScale");
            ChangeScale(864);
        }*/

        private void ChangeScale(float scale = 0)
        {
            var sc = Screen.width / (scale != 0 ? scale : Prefs);

            foreach (var target in rt)
                target.localScale = new Vector3(sc, sc, 0);
        }
    }
}
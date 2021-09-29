using UnityEngine;
using UnityEngine.UI;

namespace Classes.UI
{
    public class StatBars : MonoBehaviour
    {
        public Image sl;

        private void Start()
        {
            sl.fillAmount = 1;
        }

        public void SetPointer(float percent)
        {
            sl.fillAmount = percent;
        }
    }
}
using Classes.UI;

namespace UI
{
    public class AltBar : StatBars
    {
        public static AltBar Singletone { get; set; }

        private void Awake()
        {
            Singletone = this;
        }
    }
}
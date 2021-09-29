using Classes.UI;

namespace UI
{
    public class StaminaBar : StatBars
    {
        public static StaminaBar Singleton { get; set; }

        private void Awake()
        {
            Singleton = this;
        }
    }
}
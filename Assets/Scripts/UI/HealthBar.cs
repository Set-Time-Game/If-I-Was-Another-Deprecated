using Classes.UI;

namespace UI
{
    public class HealthBar : StatBars
    {
        public static HealthBar Singleton { get; set; }

        private void Awake()
        {
            Singleton = this;
        }
    }
}
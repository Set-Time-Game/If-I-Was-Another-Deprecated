namespace Classes.Enemies.man
{
    public class Knight : Enemy
    {
        protected override void Awake()
        {
            base.Awake();
            Immortal = true;
        }
    }
}
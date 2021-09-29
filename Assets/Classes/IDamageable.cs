namespace Classes.Enemies
{
    public interface IDamageable : IEntity
    {
        public void TakeDamage(float amount);
    }
}
using UnityEngine;

namespace Classes.Tiles
{
    public interface IObstacle : IGenerable
    {
        public int CountLimit { get; set; }
        public Collider2D CollisionCollider2D { get; }
    }
}
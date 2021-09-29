using UnityEngine;
using world;

namespace Classes.Tiles
{
    public interface IGenerable : IEntity
    {
        public bool CanPlace { get; set; }
        public bool BigObject { get; set; }
        public SpriteRenderer SpriteRenderer { get; }
        public GameObject GameObject { get; }
        public World.Layer WorldLayer { get; set; }

        public void Generate();
        public void SetHighlight(bool enable);
    }
}
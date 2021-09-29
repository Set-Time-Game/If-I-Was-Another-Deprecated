using System.Collections.Generic;
using UnityEngine;

namespace Classes.Tiles
{
    public interface IResourceSource : IGenerable
    {
        public int CountLimit { get; set; }
        public Sprite DefaultSprite { get; }
        public Sprite PickedSprite { get; }
        public List<Resource> Resources { get; }
    }
}
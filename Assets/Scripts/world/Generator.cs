using System;
using System.Collections.Generic;
using System.Linq;
using Classes.Tiles;
using UnityEngine;

namespace world
{
    public class Generator : MonoBehaviour
    {
        public bool IsGenerated;

        public TexturesLayers[] groundTextures;

        //[Space] public List<Resource> resourceVariablesList;
        [Space] public Resource[] resourceVariablesList;

        public GroundVariable[] groundVariablesList;
        public ResourceSourceVariable[] resourceSourceVariablesList;
        public ObstacleVariable[] obstacleVariablesList;
        public DecoVariable[] decoVariablesList;

        public readonly Dictionary<Vector2, DecoVariable> MapDecoVariable =
            new Dictionary<Vector2, DecoVariable>();

        public readonly Dictionary<Vector2, GroundVariable> MapGroundVariable =
            new Dictionary<Vector2, GroundVariable>();

        public readonly Dictionary<Vector2, ObstacleVariable> MapObstacleVariable =
            new Dictionary<Vector2, ObstacleVariable>();

        public readonly Dictionary<Vector2, ResourceSourceVariable> MapResourceSourceVariable =
            new Dictionary<Vector2, ResourceSourceVariable>();

        private void Start()
        {
            // yield return new WaitUntil(() => World.Singleton != null && World.Singleton.Map != null && World.Singleton.MapDeco != null);

            var map = World.Singleton.Map;
            var mapDeco = World.Singleton.MapDeco;
            var zeroPoint = World.Singleton.zeroPoint;

            SetTo<Ground>(map, zeroPoint);
            SetTo<Obstacle>(mapDeco, zeroPoint, true);
            SetTo<ResourceSource>(mapDeco, zeroPoint, true);
            SetTo<Deco>(mapDeco, zeroPoint, true);

            IsGenerated = true;
        }

        private void SetTo<T>(float[,] map, Vector2 zeroPoint, bool destroyIfLess = false) where T : IGenerable
        {
            var childrenArray = GetComponentsInChildren<T>();
            var layer = groundTextures.First(l => l.layer == childrenArray.First().WorldLayer);

            foreach (var unite in childrenArray)
            {
                var pos = unite.Transform.position;
                var target = map[(int) -(zeroPoint.x - pos.x), (int) (zeroPoint.y - pos.y)];

                if (layer.heightFrom <= target && layer.heightTo >= target &&
                    !MapObstacleVariable.ContainsKey(unite.Transform.position) &&
                    !MapResourceSourceVariable.ContainsKey(unite.Transform.position))
                {
                    unite.Generate();
                    continue;
                }

                if (!destroyIfLess) continue;

                Destroy(unite.GameObject);
                //if (childrenArray.First() is Obstacle)
                //    Debug.Log($"destroy in set");
            }
        }

        public IEnumerable<Vector2> GenerateDirections(Vector2 pos)
        {
            var x = pos.x;
            var y = pos.y;

            return new[]
            {
                new Vector2(x + 1, y),
                new Vector2(x - 1, y),

                new Vector2(x, y + 1),
                new Vector2(x, y - 1),

                new Vector2(x + 1, y + 1),
                new Vector2(x - 1, y - 1),

                new Vector2(x + 1, y - 1),
                new Vector2(x - 1, y + 1)
            };
        }

        [Serializable]
        public struct DecoVariable
        {
            public sbyte id;
            public Sprite[] textures;
        }

        [Serializable]
        public struct TexturesLayers
        {
            public string name;
            public float heightFrom;
            public float heightTo;
            public World.Layer layer;
        }

        [Serializable]
        public struct GroundVariable
        {
            public sbyte id;
            public bool canPlacing;
            public bool hasCollision;
            public Sprite[] textures;
            public Ground instantiate;
        }

        [Serializable]
        public struct ObstacleVariable
        {
            public sbyte id;
            public sbyte[] resources;
            public ObstacleVariableTextures[] textures;
        }

        [Serializable]
        public struct ObstacleVariableTextures
        {
            public Sprite outline;
            public Sprite texture;
            public Sprite pickedTexture;
        }

        [Serializable]
        public struct ResourceSourceVariable
        {
            public sbyte id;
            public sbyte[] resources;
            public ResourceSourceVariableTextures[] textures;
        }

        [Serializable]
        public struct ResourceSourceVariableTextures
        {
            public Sprite outline;
            public Sprite texture;
            public Sprite pickedTexture;
        }
    }
}
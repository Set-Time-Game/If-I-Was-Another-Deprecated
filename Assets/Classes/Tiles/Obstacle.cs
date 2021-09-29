using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using world;
using Random = UnityEngine.Random;

namespace Classes.Tiles
{
    public class Obstacle : MonoBehaviour, IObstacle, IResourceSource
    {
        [SerializeField] protected Generator biome;
        [SerializeField] protected Collider2D collisionCollider;
        [SerializeField] protected bool isbig;
        [SerializeField] protected int limit;
        [SerializeField] protected bool placeable;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        [SerializeField] private World.Layer generatorLayer;
        public Generator.ObstacleVariable obsVariable;

        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public bool CanPlace
        {
            get => placeable;
            set => placeable = value;
        }

        public bool BigObject
        {
            get => isbig;
            set => isbig = value;
        }

        public World.Layer WorldLayer
        {
            get => generatorLayer;
            set
            {
                if (!Enum.IsDefined(typeof(World.Layer), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int) value, typeof(World.Layer));

                generatorLayer = value;
            }
        }

        public void Generate()
        {
            biome = GetComponentInParent<Generator>();
            if (!biome.MapGroundVariable[Transform.position].canPlacing)
            {
                if (biome.MapObstacleVariable.ContainsKey(Transform.position))
                    biome.MapObstacleVariable.Remove(Transform.position);

                Destroy(gameObject);

                //Debug.Log($"destroy in Generate");

                return;
            }

            var t = biome.obstacleVariablesList;
            var variable = t[Random.Range(0, t.Length)];
            var textures = variable.textures[Random.Range(0, variable.textures.Length)];

            variable.textures = new[] {textures};
            obsVariable = variable;

            if (textures.texture)
                spriteRenderer.sprite = textures.texture;

            biome.MapObstacleVariable[Transform.position] = variable;

            var dirs = biome.GenerateDirections(GetComponentInParent<Ground>().transform.position);
            //var grounds = biome.GetComponentsInChildren<Ground>().Where(x => dirs.Contains(x.transform.position));

            foreach (var dir in dirs)
            {
                if (!biome.MapGroundVariable.ContainsKey(dir)) continue;

                var gvar = biome.MapGroundVariable[dir];

                gvar.instantiate.CanPlace = false;
                gvar.canPlacing = false;
                biome.MapGroundVariable[dir] = gvar;
            }
        }

        public void SetHighlight(bool enable)
        {
            if (obsVariable.textures.Length <= 0) return;
            var texture = obsVariable.textures[0];

            if (!texture.texture) return;
            SpriteRenderer.sprite = enable && texture.outline ? texture.outline : texture.texture;
        }

        public Collider2D CollisionCollider2D => collisionCollider;

        public int CountLimit
        {
            get => limit;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                limit = value;
            }
        }

        public Sprite DefaultSprite { get; }
        public Sprite PickedSprite { get; }
        public List<Resource> Resources { get; }
    }
}
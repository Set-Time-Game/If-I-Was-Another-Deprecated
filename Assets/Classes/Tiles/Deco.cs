using System;
using System.ComponentModel;
using UnityEngine;
using world;
using Random = UnityEngine.Random;

namespace Classes.Tiles
{
    public class Deco : MonoBehaviour, IGenerable
    {
        [SerializeField] protected int limit;
        [SerializeField] protected bool placeable;
        [SerializeField] protected bool isbig;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected World.Layer generatorLayer;
        [SerializeField] protected Generator.DecoVariable decoVariable;
        [SerializeField] protected Generator biome;

        public int CountLimit
        {
            get => limit;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                limit = value;
            }
        }

        public Generator Biome => biome;

        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public void Generate()
        {
            biome = GetComponentInParent<Generator>();

            if (!biome.MapGroundVariable[Transform.position].canPlacing)
            {
                if (biome.MapDecoVariable.ContainsKey(Transform.position))
                    biome.MapDecoVariable.Remove(Transform.position);

                Destroy(gameObject);

                return;
            }

            var t = biome.decoVariablesList;
            var variable = t[Random.Range(0, t.Length)];
            decoVariable = variable;

            var texture = variable.textures[Random.Range(0, variable.textures.Length)];

            if (texture)
                spriteRenderer.sprite = texture;

            biome.MapDecoVariable[Transform.position] = decoVariable;
        }

        public void SetHighlight(bool enable)
        {
        }

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
    }
}
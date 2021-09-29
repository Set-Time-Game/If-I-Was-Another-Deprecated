using System;
using System.ComponentModel;
using UnityEngine;
using world;
using Random = UnityEngine.Random;

namespace Classes.Tiles
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Ground : MonoBehaviour, IGenerable
    {
        [SerializeField] protected bool placeable;
        [SerializeField] protected bool isbig;
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] protected World.Layer generatorLayer;
        [SerializeField] protected Generator.GroundVariable groundVariable;
        [SerializeField] protected Generator biome;

        public Generator Biome => biome;

        public void Generate()
        {
            biome = GetComponentInParent<Generator>();

            var t = biome.groundVariablesList;
            var variable = t[Random.Range(0, t.Length)];
            groundVariable = variable;

            var texture = variable.textures[Random.Range(0, variable.textures.Length)];

            placeable = variable.canPlacing;

            if (texture)
                sprite.sprite = texture;

            groundVariable.instantiate = this;
            biome.MapGroundVariable[Transform.position] = groundVariable;
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


        public GameObject GameObject
        {
            get => gameObject;
            private set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                GameObject = value;
            }
        }

        public Transform Transform => transform;

        public SpriteRenderer SpriteRenderer
        {
            get => sprite;

            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                sprite = value;
            }
        }
    }
}
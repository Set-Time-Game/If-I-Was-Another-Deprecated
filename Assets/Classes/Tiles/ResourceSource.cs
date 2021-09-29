using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using world;
using Random = UnityEngine.Random;

namespace Classes.Tiles
{
    public class ResourceSource : MonoBehaviour, IResourceSource
    {
        [SerializeField] protected int limit;
        [SerializeField] protected bool placeable;
        [SerializeField] protected bool isbig;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected Sprite picked;
        [SerializeField] protected World.Layer generatorLayer;
        [SerializeField] protected List<Resource> resource;
        [SerializeField] public Generator.ResourceSourceVariable resVariable;
        [SerializeField] protected Generator biome;
        public Generator Biome => biome;

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

        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public void Generate()
        {
            biome = GetComponentInParent<Generator>();
            if (!biome.MapGroundVariable[Transform.position].canPlacing)
            {
                if (biome.MapResourceSourceVariable.ContainsKey(Transform.position))
                    biome.MapResourceSourceVariable.Remove(Transform.position);

                Destroy(gameObject);
                return;
            }

            var t = biome.resourceSourceVariablesList;
            var variable = t[Random.Range(0, t.Length)];
            var textures = variable.textures[Random.Range(0, variable.textures.Length)];

            variable.textures = new[] {textures};
            resVariable = variable;

            if (biome.resourceVariablesList.Length != 0)
                resource.AddRange(biome.resourceVariablesList.Where(x => variable.resources.Contains(x.id)));

            if (textures.texture)
            {
                spriteRenderer.sprite = textures.texture;
                sprite = textures.texture;
            }

            if (textures.pickedTexture)
                picked = textures.pickedTexture;

            biome.MapResourceSourceVariable[Transform.position] = resVariable;
        }

        public void SetHighlight(bool enable)
        {
            if (resVariable.textures.Length <= 0) return;
            var texture = resVariable.textures[0];
            if (!texture.texture || !texture.outline) return;

            SpriteRenderer.sprite = enable && texture.outline ? texture.outline : texture.texture;
        }

        public bool CanPlace
        {
            get => placeable;
            set => placeable = value;
        }

        public
            bool BigObject
        {
            get => isbig;
            set => isbig = value;
        }

        public Sprite DefaultSprite => sprite;
        public Sprite PickedSprite => picked;
        public List<Resource> Resources => resource;

        public int CountLimit
        {
            get => limit;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                limit = value;
            }
        }
    }
}
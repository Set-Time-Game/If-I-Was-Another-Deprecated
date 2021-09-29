using System.Linq;
using Classes.Characters.Slime;
using Classes.Tiles;
using UnityEngine;

namespace Scripts.characters
{
    public class ResourceCollector : MonoBehaviour
    {
        [SerializeField] private Character player;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!player.TagWhiteList.Contains(collision.tag)) return;
            if (collision.TryGetComponent<IGenerable>(out var res) && !player.resourceSourcesList.Contains(res) &&
                HasHighlight(res))
                player.resourceSourcesList.AddLast(res);
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (!player.TagWhiteList.Contains(collision.tag) || !collision.TryGetComponent<IGenerable>(out var res) &&
                !player.resourceSourcesList.Contains(res)) return;
            player.resourceSourcesList.Remove(res);
            res.SetHighlight(false);
        }

        private bool HasHighlight(IGenerable target)
        {
            if (target.GameObject.TryGetComponent<ResourceSource>(out _)) return true;
            if (target.GameObject.TryGetComponent<Obstacle>(out var obs))
                return obs.obsVariable.textures.Length >= 1 && obs.obsVariable.textures[0].outline;

            return false;
        }
    }
}
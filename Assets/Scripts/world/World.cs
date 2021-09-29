using System.Collections;
using UnityEngine;

namespace world
{
    public class World : MonoBehaviour
    {
        public enum Layer
        {
            Empty,
            Ground,
            Deco,
            Resource,
            Obstacle,
            Trigger
        }

        public AstarPath pathfinder;

        [Space] public Vector2 zeroPointDeco;
        public Vector2Int sizeDeco;
        public Vector2Int offsetsDeco;
        public int seedDeco;
        public int octavesDeco;
        public float scaleDeco;
        public float persistanceDeco;
        public float lacunarityDeco;

        [Space] public Vector2 zeroPoint;
        public Vector2Int size;
        public Vector2Int offsets;
        public int seed;
        public int octaves;
        public float scale;
        public float persistance;
        public float lacunarity;

        public float[,] Map;
        public float[,] MapDeco;
        public static World Singleton { get; private set; }

        private void Awake()
        {
            if (seed != 0)
                Random.InitState(seed);

            Map = NoiseMap.GenerateNoiseMap(size, offsets, octaves, scale, persistance, lacunarity);
            MapDeco = NoiseMap.GenerateNoiseMap(sizeDeco, offsetsDeco, octavesDeco, scaleDeco, persistanceDeco,
                lacunarityDeco);

            Singleton = this;
        }

        private IEnumerator Start()
        {
            foreach (var biome in GetComponentsInChildren<Generator>(true))
            {
                biome.gameObject.SetActive(true);
                yield return new WaitUntil(() => biome.IsGenerated);
            }

            pathfinder.Scan();
        }
    }
}
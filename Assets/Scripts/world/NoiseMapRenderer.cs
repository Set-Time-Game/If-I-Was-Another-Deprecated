using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using world;
using Random = System.Random;

public class NoiseMapRenderer : MonoBehaviour
{
    public World world;
    public bool waitForFrames = true;

    public List<TerrainLevel> terrainLevel = new List<TerrainLevel>();
    public List<ClearLevel> clearLevel = new List<ClearLevel>();

    public IEnumerator GenerateNoise(Vector2Int size, int startX = 0, int startY = 0)
    {
        //yield return new WaitUntil(() => World.singletone != null);
        if (waitForFrames)
            yield return null;
        var width = size.x;
        var height = size.y;

        var rnd = new Random();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var target = World.Singleton.Map[x, y];

                foreach (var level in terrainLevel.AsParallel()
                    .Where(level => target <= level.height && level.textures.Count >= 1))
                {
                    if (clearLevel.Count >= 1)
                    {
                        var cl = clearLevel[0];

                        if (cl.ClearHeight.AsParallel().Any(x => target <= x.from && target >= x.to))
                            continue;
                    }

                    break;
                }
            }

            if (waitForFrames && x % 2 == 0)
                yield return null;
        }
    }

    public Texture2D DrawNoise(float[,] map)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var texture = new Texture2D(width, height) {filterMode = FilterMode.Point};

        var colorMap = new Color[width * height];

        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var target = map[x, y];
            var res = Color.Lerp(Color.black, Color.white, target);
            foreach (var level in terrainLevel.AsParallel().Where(level => target <= level.height))
            {
                res = level.color;
                break;
            }

            colorMap[y * width + x] = res;
        }

        //TODO: change to SetPixels32

        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    [Serializable]
    public struct TerrainLevel
    {
        public string name;
        public float height;
        public Color color;
        public List<Tile> textures;
    }

    [Serializable]
    public struct ClearLevel
    {
        public string name;
        public Chunck layer;
        public List<ClearHeightFromTo> ClearHeight;
    }

    [Serializable]
    public struct ClearHeightFromTo
    {
        public float from;
        public float to;
    }
}
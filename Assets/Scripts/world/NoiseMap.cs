using UnityEngine;

namespace world
{
    public static class NoiseMap
    {
        public static float[,] GenerateNoiseMap(Vector2Int size, Vector2Int offset, int octaves, float scale,
            float persistance, float lacunarity)
        {
            var width = size.x;
            var height = size.y;
            var noiseMap = new float[width, height];

            if (scale <= 0) scale = 0.0001f;
            var octavesOffset = new Vector2[octaves];
            for (var i = 0; i < octaves; i++)
            {
                var xOffset = (float) (Random.Range(-100000, 100000) + offset.x);
                var yOffset = (float) (Random.Range(-100000, 100000) + offset.y);
                octavesOffset[i] = new Vector2(xOffset / size.x, yOffset / size.y);
            }

            var max = float.MinValue;
            var min = float.MaxValue;

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var amplitude = 1f;
                var frequency = 1f;
                var noiseHeight = 0f;

                for (var i = 0; i < octaves; i++)
                {
                    noiseHeight += (Mathf.PerlinNoise(x / scale * frequency + octavesOffset[i].x,
                        y / scale * frequency + octavesOffset[i].y) * 2 - 1) * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > max)
                    max = noiseHeight;
                else if (noiseHeight < min) min = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }


            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                noiseMap[x, y] = Mathf.InverseLerp(min, max, noiseMap[x, y]);

            return noiseMap;
        }
    }
}
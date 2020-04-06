using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
   
   public enum NormalizeMode {Local, Global};
    public static float[,] GenerateNoiseMap(
        int mapWidth, 
        int mapHeight, 
        int seed, 
        float noiseScale, 
        int octaves,
        float persistence,
        float lacunarity,
        Vector2 offset,
        NormalizeMode normalizeMode) {

        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Pseudo Random Number Generator
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistence;
        }

        if (noiseScale <= 0) {
            noiseScale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2f;
        float halfHeight = mapHeight/2f;

        for (int y = 0; y<mapHeight; y++) {
            for (int x = 0; x<mapWidth; x++) {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i<octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x)/noiseScale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y)/noiseScale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                } else if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x,y] = noiseHeight;
            }
        }

        for (int y = 0; y<mapHeight; y++) {
            for (int x = 0; x<mapWidth; x++) {
                if (normalizeMode == NormalizeMode.Local) {
                    // Normalize the noise map, so if value is less than minNoiseHeight, return minNoiseHeight.
                    // Same goes for maxLocalNoiseHeight.
                    noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x,y]);
                } else {
                    // Multiply maxPossibleHeight by 2 and divide by some factor 1~2 to mess with max peak height
                    float normalizedHeight = (noiseMap[x,y] + 1) /(maxPossibleHeight);
                    noiseMap[x,y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColorMap, Mesh};
    public DrawMode drawMode;
    public Noise.NormalizeMode normalizeMode;
    public const int mapChunkSize = 241;
    // i Values for detail levels, possible vals of 2,4,6,8,10,12
    [Range(0,6)]
    public int editorPreviewLOD;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistence;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
   public TerrainType[] regions;

   Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

   Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor() {
       MapData mapData = GenerateMapData(Vector2.zero);

       MapDisplay display = FindObjectOfType<MapDisplay>();
       if (drawMode == DrawMode.NoiseMap) {
        display.DrawTexture (TextureGenerator.TextureFromHeightMap(mapData.heightMap));
       } else if (drawMode == DrawMode.ColorMap) {
        display.DrawTexture (TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
       } else if (drawMode == DrawMode.Mesh) {
           display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD),
           TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
       }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callBack) {
        ThreadStart threadStart = delegate {
            MapDataThread(center, callBack);
        };

        new Thread(threadStart).Start();
    }

    // same callback as the one in requestMapData... I think
    void MapDataThread(Vector2 center, Action<MapData> callBack) {
        // This is running on a separate thread than the main
        MapData mapData = GenerateMapData(center);
        // Lock means that ONLY one thread at a time can execute the following code
        lock(mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callBack, mapData));
        }
    }

     public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callBack) {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, lod, callBack);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callBack) {
        // This is running on a separate thread than the main
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        // Lock means that ONLY one thread at a time can execute the following code
        lock(meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callBack, meshData));
        }
    }

    void Update() {
        if (mapDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);

            }
        }

        if (meshDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }


    MapData GenerateMapData(Vector2 center) {
       float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale,
                                                octaves, persistence, lacunarity, center + offset, normalizeMode);
                                    
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];                            

        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                // Due to global normalization we can no longer assume height ranges from 0 to 1
                float currentHeight = noiseMap[x,y];
                for (int i = 0; i<regions.Length; i++) {
                    if (currentHeight >= regions[i].height) {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                    } else {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
   }

   void OnValidate() {
       if (lacunarity < 1f) {
           lacunarity = 1f;
       }

       if (octaves < 0) {
           octaves = 0;
       }
   }

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}


[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}

public struct MapData {
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap) {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}

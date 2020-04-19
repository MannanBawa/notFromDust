using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, Mesh, FalloffMap};
    public DrawMode drawMode;

    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0,6)]
    public int editorPreviewLOD;

    public bool autoUpdate;
   static MapGenerator instance;

    float[,] falloffMap;

   Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

   Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    void OnValuesUpdated() {
        if (!Application.isPlaying) {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated() {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    public int mapChunkSize {
        get {
            if (terrainData.useFlatShading) {
                return 95;
            } else {
                return 239;
            }
        }
    }

    public void DrawMapInEditor() {
       MapData mapData = GenerateMapData(Vector2.zero);

       MapDisplay display = FindObjectOfType<MapDisplay>();
       if (drawMode == DrawMode.NoiseMap) {
        display.DrawTexture (TextureGenerator.TextureFromHeightMap(mapData.heightMap));
       } else if (drawMode == DrawMode.Mesh) {
           display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLOD, terrainData.useFlatShading));
       } else if (drawMode == DrawMode.FalloffMap) {
           display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
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
       float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale,
                                                noiseData.octaves, noiseData.persistence, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);
        
        if (terrainData.useFallOff) {

            if (falloffMap == null) {
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
            }

            for (int y = 0; y < mapChunkSize + 2; y++) {
                for (int x = 0; x < mapChunkSize + 2; x++) {
                    if (terrainData.useFallOff) {
                        noiseMap[x,y] = Mathf.Clamp01(noiseMap[x,y] - falloffMap[x,y]);
                    }
                }
            }
        }
        
        return new MapData(noiseMap);
   }

   void OnValidate() {

       if (terrainData != null) {
           // Unsubscribe first, in case you already are subsribed (if already un subbed, first line will do nothing)
           terrainData.onValuesUpdated -= OnValuesUpdated;
           terrainData.onValuesUpdated += OnValuesUpdated;
       }

       if (noiseData != null) {
           noiseData.onValuesUpdated -= OnValuesUpdated;
           noiseData.onValuesUpdated += OnValuesUpdated;
       }

       if (textureData != null) {
           textureData.onValuesUpdated -= OnTextureValuesUpdated;
           textureData.onValuesUpdated += OnTextureValuesUpdated;
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


public struct MapData {
    public readonly float[,] heightMap;

    public MapData(float[,] heightMap) {
        this.heightMap = heightMap;
    }
}

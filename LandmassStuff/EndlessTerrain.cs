using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    public static float maxViewDistance;
    public LODInfo[] detailLevels;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;

        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst =  Mathf.RoundToInt(maxViewDistance / chunkSize);

        UpdateVisibleChunks();
    }

    void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks() {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    

                } else {
                    //Instantiate a new terrain chunk
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }
    

    public class TerrainChunk {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        bool mapDataReceived;
        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material) {
            this.detailLevels = detailLevels;
            
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("TerrainChunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;

            meshRenderer.material = material;

            SetVisible(false); 

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i< detailLevels.Length; i++) {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData) {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk() {
            if (mapDataReceived) {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDistance;

            if (visible) {
                int lodIndex = 0;
                for (int i = 0; i< detailLevels.Length - 1; i++) {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDistanceThreshold) {
                        lodIndex = i+1;
                    } else {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex) {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh) {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    } else if (!lodMesh.hasRequestedMesh) {
                        lodMesh.RequestMesh(mapData);
                    }
                }

                terrainChunksVisibleLastUpdate.Add(this);


            }
            SetVisible(visible);
            }
        }

        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }

        public bool IsVisible() {
            return meshObject.activeSelf;
        }


    }

    class LODMesh {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;

        System.Action updateCallBack;

        public LODMesh(int lod, System.Action updateCallBack) {
            this.lod = lod;
            this.updateCallBack = updateCallBack;
        }

        void OnMeshDataReceived(MeshData meshData) {
            mesh = meshData.CreateMesh();
            hasMesh = true; 

            updateCallBack();
        }
        
        public void RequestMesh(MapData mapdata) {
            hasRequestedMesh = true;

            mapGenerator.RequestMeshData(mapdata, lod, OnMeshDataReceived);
        }

    }

    [System.Serializable]
    public struct LODInfo {
        public int lod;
        public float visibleDistanceThreshold;
    }



}

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using Photon.Pun;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
[System.Serializable]
public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    const float _colliderGenerationDistanceThreshold = 5;
    public Vector2 _coord;

    public Vector2 _sampleCenter;
    public Bounds _bounds;
    Transform _viewer;

    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    ThresholdInfo[] _thresholdInfos;
    VisibleMesh _visibleMesh;

    HeightMap _heightMap;
    BlockData _blockData;

    bool _hasSetCollider;
    bool _heightMapReceived;
    bool _blockDataReceived;

    float _maxViewDst;

    int _prevVisibleMeshIndex;
    
    HeightMapSettings _heightMapSettings;
    BlockSettings _blockSettings;  //블럭 셋팅
    public GameObject _chunkObject;       //Chunk

    public BlockData BlockData
    {
        get
        {
            return _blockData;
        }
    }
    Vector3 ViewerPosition
    {
        get
        {
            return new Vector3(_viewer.position.x, _viewer.position.y, _viewer.position.z);
        }
    }

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, BlockSettings blockSettings, ThresholdInfo[] thresholdInfos, Transform parent, Transform viewer, Material material)
    {
        _coord = coord;
        _thresholdInfos = thresholdInfos;
        _heightMapSettings = heightMapSettings;
        _blockSettings = blockSettings;
        _viewer = viewer;

        _sampleCenter = _coord * _blockSettings.ChunkSize;

        Vector2 position = _coord * _blockSettings.ChunkSize;
        Vector3 boundCenter = new Vector3(position.x + 8f, 0f, position.y + 8f);
        Vector3 boundSize = new Vector3(_blockSettings.ChunkSize, _blockSettings.MaxHeight, _blockSettings.ChunkSize);

        _bounds = new Bounds(boundCenter, boundSize);

        _chunkObject = new GameObject("TerrainChunk");
        _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
        _meshFilter = _chunkObject.AddComponent<MeshFilter>();
        _meshCollider = _chunkObject.AddComponent<MeshCollider>();
        _meshRenderer.material = material;

        _chunkObject.transform.position = new Vector3(position.x, 0, position.y);
        _chunkObject.transform.parent = parent;
        _chunkObject.layer = 3;
        SetVisible(false);

        _visibleMesh = new VisibleMesh();
        _visibleMesh.UpdateCallback += UpdateTerrainChunk;
        _visibleMesh.UpdateCallback += UpdateCollisionMesh;

        _maxViewDst = _thresholdInfos[_thresholdInfos.Length - 1].visibleDstThreshold;
        _prevVisibleMeshIndex = -1;
    }

    void RequestBlockData()
    {
        DBManager.Instance.Reference
            .Child("Minecraft").Child(GameManager.Instance.CurTitle).Child("Chunks")
            .Child($"{_coord.x}_{_coord.y}").Reference.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                var snapShot = task.Result;
                var node = snapShot.Child("_blockTypes");

                if (!snapShot.Exists && !node.Exists)
                {
                    ThreadedDataRequester.RequestData(() =>
                        BlockDataGenerator.GenerateBlockData(_heightMap, _blockSettings, _heightMapSettings),
                        OnBlockDataReceived);
                }
                else
                {
                    var blockTypesJson = snapShot.Child("_blockTypes").GetRawJsonValue();

                    ThreadedDataRequester.RequestData(() =>
                    {
                        BlockData data = new BlockData(_blockSettings);
                        data._blockTypes = JsonConvert.DeserializeObject<BlockType[,,]>(blockTypesJson);
                        return data;
                    }, OnBlockDataReceived);
                }
            });
    }
    void OnBlockDataReceived(object blockData)
    {
        _blockData = (BlockData)blockData;
        _blockDataReceived = true;
        UpdateTerrainChunk();
    }
    void OnHeightMapReceived(object heightMapObject) //수신된 heightMap을 저장하는 함수
    {
        this._heightMap = (HeightMap)heightMapObject;
        _heightMapReceived = true;

        RequestBlockData();
    }
    void FillHaloData()
    {
        for (int y = 0; y < 31; y++)
        {
            if (TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.left, out TerrainChunk leftChunk))
            {
                for (int z = 1; z <= 16; z++)
                {
                    _blockData._blockTypes[0, y, z] = leftChunk._blockData._blockTypes[16, y, z];
                }
            }
            else
            {
                for (int z = 1; z <= 16; z++)
                {
                    _blockData._blockTypes[0, y, z] = BlockType.Air;
                }
            }

            if (TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.right, out TerrainChunk rightChunk))
            {
                for (int z = 1; z <= 16; z++)
                {
                    _blockData._blockTypes[17, y, z] = rightChunk._blockData._blockTypes[1, y, z];
                }
            }
            else
            {
                for (int z = 1; z <= 16; z++)
                {
                    _blockData._blockTypes[17, y, z] = BlockType.Air;
                }
            }

            if (TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.down, out TerrainChunk backChunk))
            {
                for (int x = 1; x <= 16; x++)
                {
                    _blockData._blockTypes[x, y, 0] = backChunk._blockData._blockTypes[x, y, 16];
                }
            }
            else
            {
                for (int x = 1; x <= 16; x++)
                {
                    _blockData._blockTypes[x, y, 0] = BlockType.Air;
                }
            }

            if (TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.up, out TerrainChunk ForwardChunk))
            {
                for (int x = 1; x <= 16; x++)
                {
                    _blockData._blockTypes[x, y, 17] = ForwardChunk._blockData._blockTypes[x, y, 1];
                }
            }
            else
            {
                for (int x = 1; x <= 16; x++)
                {
                    _blockData._blockTypes[x, y, 17] = BlockType.Air;
                }
            }

        }


    }
    void MarkDirty()
    {
        _meshFilter.mesh = MeshGenerator.GenerateTerrainMeshAsBlock(_blockData, _blockSettings).CreateMesh();
        _meshCollider.sharedMesh = _meshFilter.mesh;
    }
    public void Load()
    {
        ThreadedDataRequester.RequestData(() => 
            HeightMapGenerator.GenerateHeightMap(_blockSettings.width, _blockSettings.width, _heightMapSettings, _sampleCenter), 
            OnHeightMapReceived);
    }

    [PunRPC]
    public void UpdateTerrainChunk()  //청크가 시야안에 들어와있는지 체크
    {
        if (_heightMapReceived && _blockDataReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition)); //가장 가까운 모서리까지의 거리의 제곱값에 루트씌움

            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= _maxViewDst;

            if (visible)
            {
                int visibleMeshIndex = 0;

                if (viewerDstFromNearestEdge > _thresholdInfos[0].visibleDstThreshold)
                {
                    visibleMeshIndex = 1;
                }

                if(visibleMeshIndex != _prevVisibleMeshIndex)
                {
                    if (_visibleMesh.hasMesh)
                    {
                        if(visibleMeshIndex == 1 && _hasSetCollider)
                        {
                            _meshCollider.sharedMesh = null;
                            _hasSetCollider = false;
                        }

                        _prevVisibleMeshIndex = visibleMeshIndex;
                        _meshFilter.mesh = _visibleMesh.mesh;
                    }
                    else if (!_visibleMesh.hasRequestedMeshData)
                    {
                        _visibleMesh.RequestMeshData(_blockSettings, _blockData);
                    }
                }
            }



            if (wasVisible != visible)
            {
                SetVisible(visible);

                if (OnVisibilityChanged != null)
                {
                    OnVisibilityChanged(this, visible);
                }
            }

        }
    }
    [PunRPC]
    public void UpdateTerrainChunkByPlayer(int x = 0, int y = 0, int z = 0, BlockType blockType = BlockType.None)
    {
        if (blockType != BlockType.None) _blockData._blockTypes[x + 1, y, z + 1] = blockType;
        _meshFilter.mesh = MeshGenerator.GenerateTerrainMeshAsBlock(_blockData, _blockSettings).CreateMesh();
        _meshCollider.sharedMesh = _meshFilter.mesh;

        if (x == 0 && TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.left, out TerrainChunk leftChunk))
        {
            leftChunk._blockData._blockTypes[17, y, z + 1] = blockType;
            leftChunk.MarkDirty();
        }        

        if (x == 15 && TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.right, out TerrainChunk rightChunk))
        {
            rightChunk._blockData._blockTypes[0, y, z + 1] = blockType;
            rightChunk.MarkDirty();
        }
        

        if (z == 0 && TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.down, out TerrainChunk backChunk))
        {
            backChunk._blockData._blockTypes[x + 1, y, 17] = blockType;
            backChunk.MarkDirty();  
        }
        

        if (z == 15 && TerrainGenerator.m_terrainChunkDictionary.TryGetValue(_coord + Vector2.up, out TerrainChunk ForwardChunk))
        {
            ForwardChunk._blockData._blockTypes[x + 1, y, 0] = blockType;
            ForwardChunk.MarkDirty();
        }
    }
    [PunRPC]
    public void UpdateCollisionMesh()
    {
        if (!_hasSetCollider)
        {
            float sqrDstFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

            //모서리로부터 나의 거리 < 가시거리
            if (sqrDstFromViewerToEdge < _thresholdInfos[0].sqrVisibleDstThreshold)
            {
                if (!_visibleMesh.hasRequestedMeshData)
                {
                    FillHaloData();
                    _visibleMesh.RequestMeshData(_blockSettings, _blockData);
                }
            }

            //모서리로부터 나의 거리 < 콜라이더 생성 거리 임계값
            if (sqrDstFromViewerToEdge < _colliderGenerationDistanceThreshold * _colliderGenerationDistanceThreshold)
            {
                if (_visibleMesh.hasMesh)
                {
                    _meshCollider.sharedMesh = _visibleMesh.mesh;
                    _hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        _chunkObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return _chunkObject.activeSelf;
    }

    public BlockType GetBlockType(int x, int y, int z)
    {
        return _blockData._blockTypes[x + 1, y, z + 1];
    }

    
}

[System.Serializable]
public class VisibleMesh
{
    public Mesh mesh;
    public bool hasRequestedMeshData;
    public bool hasMesh;
    public event System.Action UpdateCallback;

    public VisibleMesh() { }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((BlockMeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        UpdateCallback();
    }

    public void RequestMeshData(BlockSettings blockSettings, BlockData blockData)
    {
        hasRequestedMeshData = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMeshAsBlock(blockData, blockSettings), OnMeshDataReceived);
    }
    
}

struct HaloInfo
{
    readonly public int x;
    readonly public int y;
    readonly public int z;
    readonly BlockType blockType;
}
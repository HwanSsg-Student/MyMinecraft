using UnityEngine;
using Firebase.Extensions;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEditor.Build;
[System.Serializable]
public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    const float _colliderGenerationDistanceThreshold = 5;
    public Vector2 _coord;

    Vector2 _sampleCenter;
    Bounds _bounds;
    Transform _viewer;

    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    ThresholdInfo[] _thresholdInfos;
    VisibleMesh[] _visibleMeshes;

    HeightMap _heightMap;
    BlockData _blockData;

    bool _hasSetCollider;
    bool _heightMapReceived;
    bool _blockDataReceived;

    int _previousLODIndex = -1;
    int _colliderIndex;

    float _maxViewDst;

    
    HeightMapSettings _heightMapSettings;
    BlockSettings _blockSettings;  //블럭 셋팅
    GameObject _chunkObject;       //Chunk

    public BlockData BlockData
    {
        get
        {
            return _blockData;
        }
    }
    Vector2 ViewerPosition
    {
        get
        {
            return new Vector2(_viewer.position.x, _viewer.position.z);
        }
    }

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, BlockSettings blockSettings, ThresholdInfo[] thresholdInfos, int colliderIndex, Transform parent, Transform viewer, Material material)
    {
        _coord = coord;
        _thresholdInfos = thresholdInfos;
        _heightMapSettings = heightMapSettings;
        _blockSettings = blockSettings;
        _colliderIndex = colliderIndex;
        _viewer = viewer;

        _sampleCenter = _coord * _blockSettings.WorldSize / _blockSettings.scale;
        Vector2 position = _coord * _blockSettings.WorldSize;
        _bounds = new Bounds(position, Vector2.one * _blockSettings.WorldSize);

        _chunkObject = new GameObject("TerrainChunk");
        _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
        _meshFilter = _chunkObject.AddComponent<MeshFilter>();
        _meshCollider = _chunkObject.AddComponent<MeshCollider>();
        _meshRenderer.material = material;

        _chunkObject.transform.position = new Vector3(position.x, 0, position.y);
        _chunkObject.transform.parent = parent;
        _chunkObject.layer = 3;
        SetVisible(false);

        _visibleMeshes = new VisibleMesh[_thresholdInfos.Length];
        for (int i = 0; i < _thresholdInfos.Length; i++)  // 모든 Chuck를 생성하고있는 반복문
        {
            _visibleMeshes[i] = new VisibleMesh();
            _visibleMeshes[i].updateCallback += UpdateTerrainChunk;

            if (i == colliderIndex)
            {
                _visibleMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
        _maxViewDst = _thresholdInfos[_thresholdInfos.Length - 1].visibleDstThreshold;
    }

    void RequestBlockData()
    {
        ThreadedDataRequester.RequestData(() =>
            BlockDataGenerator.GenerateBlockData(_heightMap, _blockSettings, _heightMapSettings, _viewer),
            OnBlockDataReceived);
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

                for (int i = 0; i < _thresholdInfos.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > _thresholdInfos[i].visibleDstThreshold)
                    {
                        visibleMeshIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (visibleMeshIndex != _previousLODIndex)
                {
                    VisibleMesh visibleMesh = _visibleMeshes[visibleMeshIndex];
                    if (visibleMesh.hasMesh)
                    {
                        _previousLODIndex = visibleMeshIndex;
                        _meshFilter.mesh = visibleMesh.mesh;
                    }
                    else if (!visibleMesh.hasRequestedMeshData)
                    {
                        visibleMesh.RequestMeshData(_blockSettings, _coord, _blockData);
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
        if (blockType != BlockType.None) _blockData.m_blockTypes[x, y, z] = blockType;
        _meshFilter.mesh = MeshGenerator.GenerateTerrainMeshAsBlock(_blockData, _blockSettings).CreateMesh();
        _meshCollider.sharedMesh = _meshFilter.mesh;
    }
    [PunRPC]
    public void UpdateCollisionMesh()
    {
        if (!_hasSetCollider)
        {
            float sqrDstFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

            //모서리로부터 나의 거리 < 가시거리
            if (sqrDstFromViewerToEdge < _thresholdInfos[_colliderIndex].sqrVisibleDstThreshold)
            {
                if (!_visibleMeshes[_colliderIndex].hasRequestedMeshData)
                {
                    _visibleMeshes[_colliderIndex].RequestMeshData(_blockSettings, _coord, _blockData);
                }
            }

            //모서리로부터 나의 거리 < 콜라이더 생성 거리 임계값
            if (sqrDstFromViewerToEdge < _colliderGenerationDistanceThreshold * _colliderGenerationDistanceThreshold)
            {
                if (_visibleMeshes[_colliderIndex].hasMesh)
                {
                    _meshCollider.sharedMesh = _visibleMeshes[_colliderIndex].mesh;
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
        return _blockData.m_blockTypes[x, y, z];
    }
}

[System.Serializable]
public class VisibleMesh
{
    public Mesh mesh;
    public bool hasRequestedMeshData;
    public bool hasMesh;
    public event System.Action updateCallback;


    public VisibleMesh() { }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((BlockMeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        updateCallback();
    }

    public void RequestMeshData(BlockSettings blockSettings, Vector2 coord, BlockData blockData)
    {
        hasRequestedMeshData = true;
        BlockData data = blockData;

        DBManager.Instance.Reference.Child("Minecraft").Child(GameManager.Instance.CurTitle)
            .Child("Chunks").Child(coord.x.ToString() + coord.y.ToString())
            .Reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("IsFault");
                return;
            }
            else if (task.IsCompleted)
            {
                var snapShot = task.Result;
                if (snapShot.Value != null)
                {
                    var heightJson = snapShot.Child("height").GetRawJsonValue();
                    var widthJson = snapShot.Child("width").GetRawJsonValue();
                    var blockTypesJson = snapShot.Child("m_blockTypes").GetRawJsonValue();
                    try
                    {
                        int height = JsonConvert.DeserializeObject<int>(heightJson);
                        int width = JsonConvert.DeserializeObject<int>(widthJson);
                        BlockType[,,] blockTypes = JsonConvert.DeserializeObject<BlockType[,,]>(blockTypesJson);

                        data.height = height;
                        data.width = width;
                        data.m_blockTypes = blockTypes;
                        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMeshAsBlock(data, blockSettings), OnMeshDataReceived);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e.Message);
                        if (e.InnerException != null)
                        {
                            Debug.Log("InnerException : " + e.InnerException.Message);
                        }
                        throw;
                    }
                }
                else
                {
                    ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMeshAsBlock(data, blockSettings), OnMeshDataReceived);
                }
            }

        });
    }
    
}

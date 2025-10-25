using UnityEngine;
using Firebase.Extensions;
using Newtonsoft.Json;
using Photon.Pun;
[System.Serializable]
public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    const float colliderGenerationDistanceThreshold = 5;

    public Vector2 m_coord;

    Vector2 sampleCenter;
    Bounds bounds; //��輱

    public BlockData m_blockData;


    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    ThresholdInfo[] m_thresholdInfos;
    VisibleMesh[] m_visibleMeshes;
    int m_colliderIndex;

    HeightMap heightMap;

    bool heightMapReceivec;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDst;

    Transform m_viewer;
    HeightMapSettings m_heightMapSettings;
    BlockSettings m_blockSettings;  //�� ����
    GameObject m_chunkObject;       //Chunk



    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, BlockSettings blockSettings, ThresholdInfo[] thresholdInfos, int colliderIndex, Transform parent, Transform viewer, Material material)
    {
        m_coord = coord;
        m_thresholdInfos = thresholdInfos;
        m_heightMapSettings = heightMapSettings;
        m_blockSettings = blockSettings;
        m_colliderIndex = colliderIndex;
        m_viewer = viewer;

        sampleCenter = m_coord * m_blockSettings.WorldSize / m_blockSettings.scale;
        Vector2 position = m_coord * m_blockSettings.WorldSize;
        bounds = new Bounds(position, Vector2.one * m_blockSettings.WorldSize);

        m_chunkObject = new GameObject("TerrainChunk");
        meshRenderer = m_chunkObject.AddComponent<MeshRenderer>();
        meshFilter = m_chunkObject.AddComponent<MeshFilter>();
        meshCollider = m_chunkObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        m_chunkObject.transform.position = new Vector3(position.x, 0, position.y);
        m_chunkObject.transform.parent = parent;
        m_chunkObject.layer = 3;
        SetVisible(false);

        m_visibleMeshes = new VisibleMesh[m_thresholdInfos.Length];
        for (int i = 0; i < m_thresholdInfos.Length; i++)  // ��� Chuck�� �����ϰ��ִ� �ݺ���
        {
            m_visibleMeshes[i] = new VisibleMesh();
            m_visibleMeshes[i].updateCallback += UpdateTerrainChunk;

            if (i == colliderIndex)
            {
                m_visibleMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
        maxViewDst = m_thresholdInfos[m_thresholdInfos.Length - 1].visibleDstThreshold;
    }
    void OnHeightMapReceived(object heightMapObject) //���ŵ� heightMap�� �����ϴ� �Լ�
    {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapReceivec = true;

        UpdateTerrainChunk();
    }
    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(m_viewer.position.x, m_viewer.position.z);
        }
    }
    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(m_blockSettings.width, m_blockSettings.width, m_heightMapSettings, sampleCenter), OnHeightMapReceived);
    }
    [PunRPC]
    public void UpdateTerrainChunk()  //ûũ�� �þ߾ȿ� �����ִ��� üũ
    {
        if (heightMapReceivec)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition)); //���� ����� �𼭸������� �Ÿ��� �������� ��Ʈ����

            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= maxViewDst;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 0; i < m_thresholdInfos.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > m_thresholdInfos[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    VisibleMesh visibleMesh = m_visibleMeshes[lodIndex];
                    if (visibleMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = visibleMesh.mesh;
                    }
                    else if (!visibleMesh.hasRequestedMesh)
                    {
                        m_blockData = visibleMesh.RequestMesh(heightMap, m_blockSettings, m_heightMapSettings, m_viewer, m_coord, m_blockData);
                    }
                }
            }

            if (wasVisible != visible)
            {
                SetVisible(visible);

                if (onVisibilityChanged != null)
                {
                    onVisibilityChanged(this, visible);
                }

            }
        }
    }
    [PunRPC]
    public void UpdateTerrainChunkByPlayer(int x = 0, int y = 0, int z = 0, BlockType blockType = BlockType.None)
    {
        if (blockType != BlockType.None) m_blockData.m_blockTypes[x, y, z] = blockType;
        meshFilter.mesh = MeshGenerator.GenerateTerrainMeshAsBlock(m_blockData, m_blockSettings).CreateMesh();
        meshCollider.sharedMesh = meshFilter.mesh;
    }
    [PunRPC]
    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            //�𼭸��κ��� ���� �Ÿ� < ���ðŸ�
            if (sqrDstFromViewerToEdge < m_thresholdInfos[m_colliderIndex].sqrVisibleDstThreshold)
            {
                if (!m_visibleMeshes[m_colliderIndex].hasRequestedMesh)
                {
                    m_blockData = m_visibleMeshes[m_colliderIndex].RequestMesh(heightMap, m_blockSettings, m_heightMapSettings, m_viewer, m_coord, m_blockData);
                }
            }

            //�𼭸��κ��� ���� �Ÿ� < �ݶ��̴� ���� �Ÿ� �Ӱ谪
            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (m_visibleMeshes[m_colliderIndex].hasMesh)
                {
                    meshCollider.sharedMesh = m_visibleMeshes[m_colliderIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        m_chunkObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return m_chunkObject.activeSelf;
    }

    public BlockType GetBlockType(int x, int y, int z)
    {
        return m_blockData.m_blockTypes[x, y, z];
    }
}

[System.Serializable]
public class VisibleMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    public event System.Action updateCallback;


    public VisibleMesh() { }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((BlockMeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        updateCallback();
    }

    public BlockData RequestMesh(HeightMap heightMap, BlockSettings blockSettings, HeightMapSettings heightMapSettings, Transform viwer, Vector2 coord, BlockData blockData)
    {
        hasRequestedMesh = true;
        BlockData data = BlockDataGenerator.GenerateBlockData(heightMap, blockSettings, heightMapSettings, viwer);
        
        if (blockData == null)
        {
            DBManager.Instance.Reference.Child("Minecraft").Child(GameManager.Instance.CurTitle).Child("Chunks").Child(coord.x.ToString() + coord.y.ToString()).Reference.GetValueAsync().ContinueWithOnMainThread(task =>
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
                            if(e.InnerException != null)
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
        else
        {
            data = blockData;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMeshAsBlock(data, blockSettings), OnMeshDataReceived);
        }
        return data;
    }
    
}

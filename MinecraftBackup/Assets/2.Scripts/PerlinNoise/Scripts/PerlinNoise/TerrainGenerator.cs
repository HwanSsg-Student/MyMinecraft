using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public int colliderIndex;

    public ThresholdInfo[] thresholdInfos;

    public HeightMapSettings heightMapSettings;

    public BlockSettings m_blockSettings;

    public Transform viewer;                //Viewer�� ��ġ
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float chunkWorldSize;                   //ûũ ũ��
    int chunksVisibleInViewDst;             //Viewer�� ���� �ִ� �Ÿ� �ȿ� ���̴� ûũ�� ����

    //ûũ ������ ��ǥ�� �����ϴ� Dictionary
    public static Dictionary<Vector2, TerrainChunk> m_terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    public static Dictionary<Vector2, TerrainChunk> m_changedTerrainChunkDic = new Dictionary<Vector2, TerrainChunk>();


    //ûũ�� �Ⱥ��̴��� Ȯ���ϴ� List
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    bool m_isCompleted;

    
    void Awake()
    {
        m_isCompleted = false;
        


    }

    void Update()
    {
        if(m_isCompleted)
        {
            viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

            if (viewerPosition != viewerPositionOld)
            {
                foreach (TerrainChunk chunk in visibleTerrainChunks)
                {
                    chunk.UpdateCollisionMesh();
                }
            }

            if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;
                UpdateVisibleChunks();
            }
        }
    }
    public void InitTerrainGenerator()
    {
        viewer = GameManager.Instance.Player.transform;
        m_isCompleted = true;
        float maxViewDst = thresholdInfos[thresholdInfos.Length - 1].visibleDstThreshold;   //�ִ� ���ðŸ�
        chunkWorldSize = m_blockSettings.WorldSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkWorldSize); //ûũ�� ���̴� ����
        UpdateVisibleChunks();
    }
    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].m_coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkWorldSize); //���� ��ġ x
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkWorldSize); //���� ��ġ y

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (m_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        m_terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, m_blockSettings, thresholdInfos, colliderIndex, transform, viewer, mapMaterial);
                        m_terrainChunkDictionary.Add(viewedChunkCoord, newChunk);

                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }

            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }
}

[System.Serializable]
public struct ThresholdInfo
{
    public float visibleDstThreshold; //���ðŸ� �Ӱ谪
    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}

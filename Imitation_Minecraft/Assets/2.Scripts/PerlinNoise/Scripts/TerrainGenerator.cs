using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 16f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public ThresholdInfo[] _thresholdInfos;

    public HeightMapSettings heightMapSettings;

    public BlockSettings m_blockSettings;

    public Transform _viewer;                //Viewer의 위치
    public Material _mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    int _chunkSize;                   //청크 크기
    int _chunksVisibleInViewDst;             //Viewer가 볼수 있는 거리 안에 보이는 청크의 개수

    //청크 각각의 좌표를 저장하는 Dictionary
    public static Dictionary<Vector2, TerrainChunk> m_terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    public static Dictionary<Vector2, TerrainChunk> m_changedTerrainChunkDic = new Dictionary<Vector2, TerrainChunk>();
    

    //청크가 안보이는지 확인하는 List
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    bool _isCompleted;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (TerrainChunk chunk in visibleTerrainChunks)
        {
            Gizmos.DrawWireCube(chunk._bounds.center, new Vector3(chunk._bounds.size.x, chunk._bounds.size.y, chunk._bounds.size.z));
        }

    }
#endif

    void Awake()
    {
        _isCompleted = false;
    }

    void Update()
    {
        if(_isCompleted)
        {
            viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

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
        _viewer = GameManager.Instance.Player.transform;
        _isCompleted = true;
        float maxViewDst = _thresholdInfos[_thresholdInfos.Length - 1].visibleDstThreshold;   

        _chunkSize = m_blockSettings.ChunkSize;
        _chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / _chunkSize); 
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i]._coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / _chunkSize); //현재 위치 x
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / _chunkSize); //현재 위치 y

        for (int yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++)
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
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, m_blockSettings, _thresholdInfos, transform, _viewer, _mapMaterial);
                        m_terrainChunkDictionary.Add(viewedChunkCoord, newChunk);

                        newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
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
    public float visibleDstThreshold; //가시거리 임계값
    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}

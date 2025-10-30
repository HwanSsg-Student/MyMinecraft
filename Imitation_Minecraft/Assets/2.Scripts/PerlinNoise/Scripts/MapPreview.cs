using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public enum DrawMode { NoiseMap, DefaultMap, FlatMap }
    #region [Variable]
    public DrawMode drawMode;

    public Renderer textureRender;
    public HeightMapSettings m_heightMapSettings;
    public BlockSettings m_blockSettings;
    public GameObject m_mapGenerator;
    public MeshFilter m_meshFilter;
    public MeshCollider _meshCollider;

    public bool autoUpdate;

    BlockData m_blockData;
    Bounds _bounds;

    #endregion
    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

    }
    public void DrawMesh(BlockMeshData blockMeshData)
    {
        m_meshFilter.mesh = blockMeshData.CreateMesh();
        _meshCollider.sharedMesh = m_meshFilter.sharedMesh;
        Vector3 v3 = m_meshFilter.gameObject.transform.position;
        _bounds = new Bounds(new Vector3(v3.x + 8, 0f, v3.z + 8), Vector3.one * 16f);
    }
   
    public void DrawMapInEditor()                   //맵 출력
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(m_blockSettings.width, m_blockSettings.width, m_heightMapSettings, new Vector2(m_heightMapSettings.noiseSettings.offset.x, m_heightMapSettings.noiseSettings.offset.y));
        m_blockData = BlockDataGenerator.GenerateBlockData(heightMap, m_blockSettings, m_heightMapSettings);

        if (drawMode == DrawMode.DefaultMap)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMeshAsBlock(m_blockData, m_blockSettings));
        }
        else if (drawMode == DrawMode.FlatMap)
        {
            return;
        }
        else if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void Start()
    {
        DrawMapInEditor();
    }
    void OnValidate()                       //스크립트가 로드되거나 값이 변결될 때마다 호출되는
    {
        if (m_blockSettings != null)
        {
            m_blockSettings.OnValuesUpdated -= OnValuesUpdated;
            m_blockSettings.OnValuesUpdated += OnValuesUpdated;
            
        }
        if(m_heightMapSettings != null)
        {
            m_heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            m_heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }

}

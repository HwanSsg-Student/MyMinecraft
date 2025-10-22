using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public enum DrawMode { NoiseMap, DefaultMap, FlatMap }
    #region [����]
    public DrawMode drawMode;

    public Renderer textureRender;
    public HeightMapSettings m_heightMapSettings;
    public BlockSettings m_blockSettings;
    public GameObject m_mapGenerator;
    public MeshFilter m_meshFilter;
    public Transform m_viewer;

    BlockData m_blockData;

    public bool autoUpdate;

    
    #endregion
    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

    }
    public void DrawMesh(BlockMeshData blockMeshData)
    {
        m_meshFilter.mesh = blockMeshData.CreateMesh();
    }
   
    public void DrawMapInEditor()                   //�� ���
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(m_blockSettings.width, m_blockSettings.width, m_heightMapSettings, Vector2.zero);
        m_blockData = BlockDataGenerator.GenerateBlockData(heightMap, m_blockSettings, m_heightMapSettings, m_viewer);

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

    private void Start()
    {
        DrawMapInEditor();
    }
    void OnValidate()                       //��ũ��Ʈ�� �ε�ǰų� ���� ����� ������ ȣ��Ǵ�
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
}

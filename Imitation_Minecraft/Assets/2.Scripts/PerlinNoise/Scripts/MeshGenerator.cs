using UnityEngine;

public static class MeshGenerator
{
    public static BlockMeshData GenerateTerrainMeshAsBlock(BlockData blockData, BlockSettings blockSettings)
    {
        int numVertsPerline = blockSettings.width;
        int maxHeight = blockSettings.MaxHeight;

        BlockType[,,] blockTypes = blockData._blockTypes;

        BlockMeshData blockMesh = new BlockMeshData(numVertsPerline, maxHeight, blockSettings);

        // 0 ~ 17 : Halo 
        // 1 ~ 16 : Main

        for (int z = 1; z < numVertsPerline - 1; z++)
        {
            for (int x = 1; x < numVertsPerline - 1; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    if (blockTypes[x, y, z] != BlockType.Air)
                    {
                        if (blockTypes[x, y, z + 1] == BlockType.Air)
                        {
                            blockMesh.AddVertex(new Vector3(x - 1, y, z - 1), DirTypeData.Forward, blockTypes[x, y, z]);
                        }
                        if (blockTypes[x, y, z - 1] == BlockType.Air)
                        {
                            blockMesh.AddVertex(new Vector3(x - 1, y, z - 1), DirTypeData.Back, blockTypes[x, y, z]);
                        }
                        if (blockTypes[x - 1, y, z] == BlockType.Air)
                        {
                            blockMesh.AddVertex(new Vector3(x - 1, y, z - 1), DirTypeData.Left, blockTypes[x, y, z]);
                        }
                        if (blockTypes[x + 1, y, z] == BlockType.Air)
                        {
                            blockMesh.AddVertex(new Vector3(x - 1, y, z - 1), DirTypeData.Right, blockTypes[x, y, z]);
                        }
                        if (y < maxHeight - 1 && blockTypes[x, y + 1, z] == BlockType.Air)
                        {
                            blockMesh.AddVertex(new Vector3(x - 1, y, z - 1), DirTypeData.Up, blockTypes[x, y, z]);
                        }
                        if (y > 0 && blockTypes[x, y - 1, z] == BlockType.Air)
                        {
                            blockMesh.AddVertex(new Vector3(x - 1, y, z - 1), DirTypeData.Down, blockTypes[x, y, z]);
                        }
                    }
                }

            }
        }

        return blockMesh;
    }
}
[System.Serializable]
public class BlockMeshData
{
    BlockSettings _blockSettings;

    Vector3[] _vertices;    
    int[] _triangles;           
    Vector2[] _uvs;

    int _vertexIndex;
    int _triangleIndex;
    int _uvIndex;

    public BlockMeshData(int width, int maxHeight, BlockSettings blockSettings)
    {
        _vertices = new Vector3[width * width * maxHeight * 24];
        _triangles = new int[width * width * maxHeight * 36];
        _uvs = new Vector2[_vertices.Length];

        _vertexIndex = 0;
        _triangleIndex = 0;
        _uvIndex = 0;

        _blockSettings = blockSettings;
    }


    public void AddVertex(Vector3 vertexPos, DirTypeData dir, BlockType blockType)
    {
        var data = _blockSettings.blockDatas;
        int typeIndex = 0;

        for (int i = 0; i < data.Length; i++)
        {
            if (blockType == data[i].m_blockType)
            {
                typeIndex = i;
                break;
            }
        }

        switch (dir)
        {
            case DirTypeData.Forward:
                _vertices[_vertexIndex] = vertexPos + new Vector3(1, 1, 1);         //왼쪽 위
                _vertices[_vertexIndex + 1] = vertexPos + new Vector3(0, 1, 1);     //오른쪽 위
                _vertices[_vertexIndex + 2] = vertexPos + new Vector3(1, 0, 1);     //왼쪽 아래
                _vertices[_vertexIndex + 3] = vertexPos + new Vector3(0, 0, 1);     //오른쪽 아래

                AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                _vertexIndex += 4;
                break;
            case DirTypeData.Back:
                _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 0);         //왼쪽 위
                _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);     //오른쪽 위
                _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 0);     //왼쪽 아래
                _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                _vertexIndex += 4;
                break;
            case DirTypeData.Right:
                _vertices[_vertexIndex] = vertexPos + new Vector3(1, 1, 0);         //왼쪽 위
                _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);     //오른쪽 위
                _vertices[_vertexIndex + 2] = vertexPos + new Vector3(1, 0, 0);     //왼쪽 아래
                _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 1);     //오른쪽 아래

                AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                _vertexIndex += 4;
                break;
            case DirTypeData.Left:
                _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                _vertices[_vertexIndex + 1] = vertexPos + new Vector3(0, 1, 0);     //오른쪽 위
                _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);     //왼쪽 아래
                _vertices[_vertexIndex + 3] = vertexPos + new Vector3(0, 0, 0);     //오른쪽 아래

                AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                _vertexIndex += 4;
                break;
            case DirTypeData.Up:
                _vertices[_vertexIndex] = vertexPos + new Vector3(1, 1, 0);         //왼쪽 위
                _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);     //오른쪽 위
                _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 1, 0);     //왼쪽 아래
                _vertices[_vertexIndex + 3] = vertexPos + new Vector3(0, 1, 1);     //오른쪽 아래

                AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 2);
                AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 1);
                AddUV(data[typeIndex].m_top[0], data[typeIndex].m_top[1], data[typeIndex].m_top[2], data[typeIndex].m_top[3]);
                _vertexIndex += 4;
                break;
            case DirTypeData.Down:
                _vertices[_vertexIndex] = vertexPos + new Vector3(1, 0, 1);         //왼쪽 위
                _vertices[_vertexIndex + 1] = vertexPos + new Vector3(0, 0, 1);     //오른쪽 위
                _vertices[_vertexIndex + 2] = vertexPos + new Vector3(1, 0, 0);     //왼쪽 아래
                _vertices[_vertexIndex + 3] = vertexPos + new Vector3(0, 0, 0);     //오른쪽 아래

                AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                AddUV(data[typeIndex].m_bottom[0], data[typeIndex].m_bottom[1], data[typeIndex].m_bottom[2], data[typeIndex].m_bottom[3]);
                _vertexIndex += 4;
                break;

        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        _triangles[_triangleIndex] = a;
        _triangles[_triangleIndex + 1] = b;
        _triangles[_triangleIndex + 2] = c;
        _triangleIndex += 3;
    }
    public void AddUV(Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4)
    {
        _uvs[_uvIndex] = pos1;
        _uvs[_uvIndex + 1] = pos2;
        _uvs[_uvIndex + 2] = pos3;
        _uvs[_uvIndex + 3] = pos4;
        _uvIndex += 4;
    }
    
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
        mesh.uv = _uvs;

        mesh.RecalculateNormals();
        
        return mesh;
    }
}
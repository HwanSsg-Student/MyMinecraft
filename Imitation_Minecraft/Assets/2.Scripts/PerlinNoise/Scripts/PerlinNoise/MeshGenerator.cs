using UnityEngine;

public static class MeshGenerator
{
    public static BlockMeshData GenerateTerrainMeshAsBlock(BlockData blockData, BlockSettings blockSettings)
    {
        int width = blockSettings.width;
        int maxHeight = blockSettings.MaxHeight;

        BlockType[,,] blockTypes = blockData._blockTypes;

        BlockMeshData blockMesh = new BlockMeshData(width, maxHeight, blockSettings);

        int boundaryIndex = 0;        
        int temp = 0;
        int boundTemp = 0;
        bool countStart = false;

        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x >= 16 || z >= 16) //청크 경계선 블럭
                {
                    int boundaryX;
                    int boundaryZ;
                    if (boundaryIndex < 36)
                    {
                        boundaryX = boundaryIndex % width - 1;      // -1 ~ 16
                        boundaryZ = boundaryIndex < 18 ? -1 : 16;
                    }
                    else //36부터 
                    {
                        boundaryX = boundaryIndex % 2 == 0 ? -1 : 16;
                        boundaryZ = boundTemp++ / 2;
                    }

                    
                    for (int y = 0; y < maxHeight; y++)
                    {
                        //각 꼭짓점 제외
                        if ((boundaryX == -1 && boundaryZ == -1) || (boundaryX == 16 && boundaryZ == -1) || (boundaryX == -1 && boundaryZ == 16) || (boundaryX == 16 && boundaryZ == 16))
                        {
                            break;
                        }

                        if (blockTypes[x, y, z] == BlockType.Air)
                        {
                            if (boundaryIndex < 18) // 아래
                            {
                                if (blockTypes[boundaryX, y, 0] != BlockType.Air)
                                {
                                    blockMesh.AddVertex(new Vector3(boundaryX, y, boundaryZ), DirTypeData.Forward, blockTypes[boundaryX, y, 0], true);
                                }
                            }
                            else if (boundaryIndex < 36)
                            {
                                if (blockTypes[boundaryX, y, 15] != BlockType.Air)
                                {
                                    blockMesh.AddVertex(new Vector3(boundaryX, y, boundaryZ), DirTypeData.Back, blockTypes[boundaryX, y, 15], true);
                                }
                            }
                            else
                            {
                                if (!countStart) countStart = true;
                                if (blockTypes[temp % 2 == 0 ? 0 : 15, y, temp / 2] != BlockType.Air)
                                {
                                    if (temp % 2 == 0)
                                        blockMesh.AddVertex(new Vector3(boundaryX, y, boundaryZ), DirTypeData.Right, blockTypes[temp % 2 == 0 ? 0 : 15, y, temp / 2], true);
                                    else if (temp % 2 != 0)
                                        blockMesh.AddVertex(new Vector3(boundaryX, y, boundaryZ), DirTypeData.Left, blockTypes[temp % 2 == 0 ? 0 : 15, y, temp / 2], true);

                                }
                            }
                        }
                    }
                    if (countStart) temp++;

                    boundaryIndex++;
                }
                else //청크
                {
                    for (int y = 0; y < maxHeight; y++)
                    {
                        if (blockTypes[x, y, z] != BlockType.Air)
                        {
                            if (z < width - 3 && blockTypes[x, y, z + 1] == BlockType.Air)
                            {
                                blockMesh.AddVertex(new Vector3(x, y, z), DirTypeData.Forward, blockTypes[x, y, z]);
                            }
                            if (z > 0 && blockTypes[x, y, z - 1] == BlockType.Air)
                            {
                                blockMesh.AddVertex(new Vector3(x, y, z), DirTypeData.Back, blockTypes[x, y, z]);
                            }

                            if (x < width - 3 && blockTypes[x + 1, y, z] == BlockType.Air)
                            {
                                blockMesh.AddVertex(new Vector3(x, y, z), DirTypeData.Right, blockTypes[x, y, z]);
                            }

                            if (x > 0 && blockTypes[x - 1, y, z] == BlockType.Air)
                            {
                                blockMesh.AddVertex(new Vector3(x, y, z), DirTypeData.Left, blockTypes[x, y, z]);
                            }

                            if (y < maxHeight - 1 && blockTypes[x, y + 1, z] == BlockType.Air)
                            {
                                blockMesh.AddVertex(new Vector3(x, y, z), DirTypeData.Up, blockTypes[x, y, z]);
                            }

                            if (y > 0 && blockTypes[x, y - 1, z] == BlockType.Air)
                            {
                                blockMesh.AddVertex(new Vector3(x, y, z), DirTypeData.Down, blockTypes[x, y, z]);
                            }

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

    Vector3[] _vertices; //꼭짓점
    int[] _triangles;    //삼각형을 구성하는 배열
    Vector2[] _uvs;      //UV 배열

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


    public void AddVertex(Vector3 vertexPos, DirTypeData dir, BlockType blockType, bool isBoundary = false)
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

        if (!isBoundary)
        {
            switch (dir)
            {
                case DirTypeData.Forward:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 1);     //오른쪽 아래

                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 2);
                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Back:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 0);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 0);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Right:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(1, 1, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(1, 0, 1);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 2);
                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Left:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(0, 1, 0);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(0, 0, 0);     //오른쪽 아래

                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Up:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 1, 0);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 1, 0);     //오른쪽 아래

                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_top[0], data[typeIndex].m_top[1], data[typeIndex].m_top[2], data[typeIndex].m_top[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Down:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0, 0, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 0, 1);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 0);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 2);
                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_bottom[0], data[typeIndex].m_bottom[1], data[typeIndex].m_bottom[2], data[typeIndex].m_bottom[3]);
                    _vertexIndex += 4;
                    break;

            }
        }
        else if(isBoundary)
        {
            switch (dir)
            {
                case DirTypeData.Forward:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 1);     //오른쪽 아래

                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Back:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0, 1, 0);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 0);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 2);
                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Right:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(1, 1, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(1, 0, 1);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래
                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 2);
                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
                case DirTypeData.Left:
                    _vertices[_vertexIndex] = vertexPos + new Vector3(0 ,1, 1);         //왼쪽 위
                    _vertices[_vertexIndex + 1] = vertexPos + new Vector3(0, 1, 0);      //오른쪽 위
                    _vertices[_vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    _vertices[_vertexIndex + 3] = vertexPos + new Vector3(0, 0, 0);     //오른쪽 아래
                    AddTriangle(_vertexIndex + 3, _vertexIndex, _vertexIndex + 2);
                    AddTriangle(_vertexIndex, _vertexIndex + 3, _vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    _vertexIndex += 4;
                    break;
            }
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
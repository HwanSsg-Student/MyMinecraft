using UnityEngine;
/// <summary>
/// vertices(vertex)    : 꼭짓점 = Width * Height, 정점은 255^2(65025)개까지의 제한이 있음
/// 삼각형 배열의 크기  : (Width - 1)(Height - 1) * 6
/// heightMultiplier    : 높이 상수(?)
/// levelOfDetail(LOD)  : 세부 수준, (width - 1)의 인수를 갖음
/// LOD에 따른 꼭짓점 수: (width - 1)/LOD + 1
/// heightCurve         : 높이에 따른 가파름 정도(?)
/// meshSize = bordered size - 2;
/// </summary>
public static class MeshGenerator
{
    public static BlockMeshData GenerateTerrainMeshAsBlock(BlockData blockData, BlockSettings blockSettings)
    {
        int width = blockSettings.width;
        int maxHeight = blockSettings.MaxHeight;

        
        BlockType[,,] blockTypes = blockData.m_blockTypes;
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
    BlockSettings m_blockSettings;

    Vector3[] vertices; //꼭짓점
    int[] triangles;    //삼각형을 구성하는 배열
    Vector2[] uvs;      //UV 배열

    int vertexIndex;
    int triangleIndex;
    int uvIndex;

    public BlockMeshData(int width, int maxHeight, BlockSettings blockSettings)
    {
        vertices = new Vector3[width * width * maxHeight * 24];
        triangles = new int[width * width * maxHeight * 36];
        uvs = new Vector2[vertices.Length];
        vertexIndex = 0;
        triangleIndex = 0;
        uvIndex = 0;
        m_blockSettings = blockSettings;
    }


    public void AddVertex(Vector3 vertexPos, DirTypeData dir, BlockType blockType, bool isBoundary = false)
    {
        var data = m_blockSettings.blockDatas;
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
                    vertices[vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 0, 1);     //오른쪽 아래

                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 2);
                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Back:
                    vertices[vertexIndex] = vertexPos + new Vector3(0, 1, 0);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 0, 0);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 2);
                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Right:
                    vertices[vertexIndex] = vertexPos + new Vector3(1, 1, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(1, 0, 1);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 2);
                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Left:
                    vertices[vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(0, 1, 0);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(0, 0, 0);     //오른쪽 아래

                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 2);
                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Up:
                    vertices[vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 1, 0);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 1, 0);     //오른쪽 아래

                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 2);
                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 1);
                    AddUV(data[typeIndex].m_top[0], data[typeIndex].m_top[1], data[typeIndex].m_top[2], data[typeIndex].m_top[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Down:
                    vertices[vertexIndex] = vertexPos + new Vector3(0, 0, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 0, 1);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 0, 0);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 2);
                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 1);
                    AddUV(data[typeIndex].m_bottom[0], data[typeIndex].m_bottom[1], data[typeIndex].m_bottom[2], data[typeIndex].m_bottom[3]);
                    vertexIndex += 4;
                    break;

            }
        }
        else if(isBoundary)
        {
            switch (dir)
            {
                case DirTypeData.Forward:
                    vertices[vertexIndex] = vertexPos + new Vector3(0, 1, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 1, 1);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 0, 1);     //오른쪽 아래

                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 2);
                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Back:
                    vertices[vertexIndex] = vertexPos + new Vector3(0, 1, 0);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 0, 0);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래

                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 2);
                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Right:
                    vertices[vertexIndex] = vertexPos + new Vector3(1, 1, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(1, 1, 0);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(1, 0, 1);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(1, 0, 0);     //오른쪽 아래
                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 2);
                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
                case DirTypeData.Left:
                    vertices[vertexIndex] = vertexPos + new Vector3(0 ,1, 1);         //왼쪽 위
                    vertices[vertexIndex + 1] = vertexPos + new Vector3(0, 1, 0);      //오른쪽 위
                    vertices[vertexIndex + 2] = vertexPos + new Vector3(0, 0, 1);    //왼쪽 아래
                    vertices[vertexIndex + 3] = vertexPos + new Vector3(0, 0, 0);     //오른쪽 아래
                    AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 2);
                    AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 1);
                    AddUV(data[typeIndex].m_side[0], data[typeIndex].m_side[1], data[typeIndex].m_side[2], data[typeIndex].m_side[3]);
                    vertexIndex += 4;
                    break;
            }
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }
    public void AddUV(Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4)
    {
        uvs[uvIndex] = pos1;
        uvs[uvIndex + 1] = pos2;
        uvs[uvIndex + 2] = pos3;
        uvs[uvIndex + 3] = pos4;
        uvIndex += 4;
    }
    
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        
        return mesh;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MeshSettings에 LOD가 없는 이유는 각기 다른 해상도로 Mesh를 그린다고 하더라도 모두 같은 Mesh라고 생각하기 때문에 모든 Mesh가 모두 같은 Mesh 객체를 갖고있어야 한다고 생각한다.
/// </summary>
[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{
    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatshadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };
    

    public float meshScale;
    public bool useFlatShading;


    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex;

    [Range(0, numSupportedFlatshadedChunkSizes - 1)]
    public int flatshadedChunkSizeIndex;

    //num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals;
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }

    public float meshWorldSize      //청크 하나 크기
    {
        get
        {
            return 16 * meshScale;
        }
    }
}

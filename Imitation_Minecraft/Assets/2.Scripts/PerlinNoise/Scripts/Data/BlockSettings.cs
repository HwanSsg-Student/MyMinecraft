using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class BlockSettings : UpdatableData
{
    public const int maxHeight = 30;

    public int width = 18;

    public static bool isFlat;     //ÆòÁö

    public int MaxHeight
    {
        get { return maxHeight; }
    }
    
    public int ChunkSize
    {
        get
        {
            return (width - 2);
        }
    }


    public BlockLayer[] blockDatas;

    [System.Serializable]
    public class BlockLayer
    {
        public BlockType m_blockType;
        public Vector2[] m_top;
        public Vector2[] m_side;
        public Vector2[] m_bottom;
        public Rect m_rect;
        [Range(-1, 21)]
        public int m_startHeight;
    }

}

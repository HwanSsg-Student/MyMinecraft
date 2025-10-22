using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class BlockSettings : UpdatableData
{
    public const int maxHeight = 30;

    public float scale;

    public int width;

    public static bool isFlat;     //ÆòÁö

    public int MaxHeight
    {
        get { return maxHeight; }
    }
    
    public float WorldSize
    {
        get
        {
            return (width - 2) * scale;
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
        [Range(0, 21)]
        public float m_startHeight;
    }

}

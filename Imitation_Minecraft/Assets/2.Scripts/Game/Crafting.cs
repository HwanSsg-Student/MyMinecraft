using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField]
    List<ItemSlot> m_itemSlotList = new List<ItemSlot>();
    [SerializeField]
    ItemSlot m_resultSlot;
    [SerializeField]
    GameObject m_gameItem;


    Dictionary<string, BlockType> m_recipeDic = new Dictionary<string, BlockType>();

    BlockType Craft(List<ItemSlot> slots)
    {
        string str = "";
        foreach (var item in slots)
        {
            if(item.Type == BlockType.None)
            {
                str += "-1";
            }
            else
            {
                str += ((int)item.Type).ToString();
            }
        }

        if(m_recipeDic.ContainsKey(str))
        {
            var obj = Instantiate(m_gameItem);
            obj.transform.parent = m_resultSlot.transform;
            obj.transform.position = Vector3.zero;
            var item = obj.GetComponent<GameItem>();
            m_resultSlot.m_gameItem = item;
            return m_recipeDic[str];
        }
        return BlockType.None;
    }

    void Awake()
    {
        m_recipeDic.Add("4-1-1-1", BlockType.Plank);
        m_recipeDic.Add("-14-1-1", BlockType.Plank);
        m_recipeDic.Add("-1-14-1", BlockType.Plank);
        m_recipeDic.Add("-1-1-14", BlockType.Plank);
    }

    
    void Update()
    {
        Craft(m_itemSlotList);
    }
}
public class Recipe
{
    List<int> work = new List<int>();

    int[] Plank =
    {
        4, -1, -1, -1
    };

    BlockType Craft(List<ItemSlot> slots)
    {
        foreach (var item in slots)
        {
            if (item == null) work.Add((int)BlockType.None);
            else work.Add((int)item.Type);
        }

        if(work.ToArray() == Plank)
        {
            return BlockType.Plank;
        }
        return BlockType.None;
    }

}

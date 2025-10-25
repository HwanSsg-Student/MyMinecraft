using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CraftingData
{
    ItemSlot m_itemSlot1;
    ItemSlot m_itemSlot2;
    ItemSlot m_itemSlot3;
    ItemSlot m_itemSlot4;

    public CraftingData(ItemSlot slot1, ItemSlot slot2, ItemSlot slot3, ItemSlot slot4)
    {
        this.m_itemSlot1 = slot1;
        this.m_itemSlot2 = slot2;
        this.m_itemSlot3 = slot3;
        this.m_itemSlot4 = slot4;
    }

    
}

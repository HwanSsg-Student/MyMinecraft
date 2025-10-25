using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System;

public class GameItem : MonoBehaviour
{
    const int MaxCount = 64;
    int m_count;
    int m_myIndex;

    [SerializeField]
    BlockSettings m_blockSettings;
    [SerializeField]
    RawImage m_image;
    [SerializeField]
    Text m_countText;
    [SerializeField]
    ItemSlot m_itemSlot;


    StringBuilder m_sb;
    BlockType m_blockType;
    

    public RectTransform m_rectTransform;
    bool m_isDragging;
    bool m_isSelect;
    Vector3 m_offset;


    #region [Property]
    public BlockType Type
    {
        get { return m_blockType; }
        set { m_blockType = value; }
    }
    public int Count
    {
        get { return m_count; }
        set { m_count = value; }
    }
    public int MyIndex
    {
        get { return m_myIndex; }
        private set { }
    }

    public bool IsDragging
    {
        get { return m_isDragging; }
        set { m_isDragging = value; }
    }
    public ItemSlot CurSlot
    {
        get { return m_itemSlot; }
    }
    #endregion

    public void OnDrag()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform, mousePos, null))
            {
                if (Inventory.Instance.IsHold)
                {
                    Inventory.Instance.IsHold = false;
                    return;
                }
                m_isDragging = true;
                m_offset = m_rectTransform.position - mousePos;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_isDragging = false;
        }

        if(m_isDragging)
        {
            Vector3 curMousePos = Input.mousePosition;
            m_rectTransform.position = curMousePos + m_offset;
        }
    }
    public void OnHoldHalf()
    {
        if(Inventory.Instance.IsHold)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Inventory.Instance.IsHold = false;
                return;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            
            if (RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform, mousePos))
            {
                if (Inventory.Instance.IsHold)
                {
                    Inventory.Instance.IsHold = false;
                    return;
                }
                Inventory.Instance.BringItemHalf(this, m_itemSlot);

                if(m_count != 1)
                {
                    m_count = Mathf.CeilToInt(((float)m_count) / 2);
                }
                else if(m_count == 1)
                {
                    Inventory.Instance.HoldingItem = m_itemSlot.m_gameItem;
                    m_itemSlot.InitSlot();
                    m_itemSlot = null;
                    Inventory.Instance.IsHold = true;

                    //Inventory.Instance.ReturnToPool(this);
                    return;
                }

                m_sb.AppendFormat("{0}", m_count);
                m_countText.text = m_sb.ToString();
                m_sb.Clear();
            }
        }
    }

    public void InitBlockItem(ItemData item, ItemSlot slot)
    {
        m_blockType = item.blockType;
        m_itemSlot = slot;
        m_myIndex = item.index;
        var rawImage = GetComponent<RawImage>();

        if((int)m_blockType >= 9)
            rawImage.texture = Resources.Load<Texture>("Image/ItemIcons");
        else 
            rawImage.texture = Resources.Load<Texture>("Image/BlockIcons");

        var datas = m_blockSettings.blockDatas;

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].m_blockType == m_blockType)
            {       
                m_image.uvRect = datas[i].m_rect;
            }
        }
        m_count = item.count;
        m_sb.AppendFormat("{0}", item.count);
        m_countText.text = m_sb.ToString();
        m_sb.Clear();

    }
    
    public bool AddItem(int count = 1)
    {
        if(m_count < MaxCount)
        {
            m_count += count;
            m_sb.AppendFormat("{0}", m_count);
            m_countText.text = m_sb.ToString();
            m_sb.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void UseItem()
    {
        m_count -= 1;
        if(m_count == 0)
        {
            m_count = 1;
            m_blockType = BlockType.None;
            m_itemSlot.InitSlot();
            m_itemSlot = null;
            Inventory.Instance.ReturnToPool(this);
            return;
        }
        m_sb.AppendFormat("{0}", m_count);
        m_countText.text = m_sb.ToString();
        m_sb.Clear();
    }
    public void ChangeSlot(ItemSlot slot)
    {
        m_itemSlot = slot;
        transform.parent = m_itemSlot.transform;
        m_rectTransform.anchoredPosition = new Vector2(24f, -24f);
    }
    void Awake()
    {
        m_sb = new StringBuilder();
        m_blockType = BlockType.None;
        m_rectTransform = GetComponent<RectTransform>();
        m_count = 1;
    }
    void Update()
    {
        if (Inventory.Instance.GetActive() || GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
        {
            OnDrag();
            OnHoldHalf();
        }
    }
}

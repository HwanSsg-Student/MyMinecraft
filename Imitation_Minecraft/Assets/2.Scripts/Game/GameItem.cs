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
    BlockSettings _blockSettings;
    [SerializeField]
    RawImage _image;
    [SerializeField]
    Text _countText;
    [SerializeField]
    ItemSlot _itemSlot;


    StringBuilder _sb;
    BlockType _blockType;
    

    public RectTransform _rectTransform;
    bool _isDragging;
    Vector3 _offset;


    #region [Property]
    public BlockType Type
    {
        get { return _blockType; }
        set { _blockType = value; }
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
        get { return _isDragging; }
        set { _isDragging = value; }
    }
    public ItemSlot CurSlot
    {
        get { return _itemSlot; }
    }
    #endregion

    public void OnDrag()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, mousePos, null))
            {
                if (Inventory.Instance.IsHold)
                {
                    Inventory.Instance.IsHold = false;
                    return;
                }
                _isDragging = true;
                _offset = _rectTransform.position - mousePos;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        if(_isDragging)
        {
            Vector3 curMousePos = Input.mousePosition;
            _rectTransform.position = curMousePos + _offset;
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
            
            if (RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, mousePos))
            {
                if (Inventory.Instance.IsHold)
                {
                    Inventory.Instance.IsHold = false;
                    return;
                }
                Inventory.Instance.BringItemHalf(this, _itemSlot);

                if(m_count != 1)
                {
                    m_count = Mathf.CeilToInt(((float)m_count) / 2);
                }
                else if(m_count == 1)
                {
                    Inventory.Instance.HoldingItem = _itemSlot.m_gameItem;
                    _itemSlot.InitSlot();
                    _itemSlot = null;
                    Inventory.Instance.IsHold = true;

                    //Inventory.Instance.ReturnToPool(this);
                    return;
                }

                _sb.AppendFormat("{0}", m_count);
                _countText.text = _sb.ToString();
                _sb.Clear();
            }
        }
    }

    public void InitBlockItem(ItemData item, ItemSlot slot)
    {
        _blockType = item.blockType;
        _itemSlot = slot;
        m_myIndex = item.index;
        var rawImage = GetComponent<RawImage>();

        if((int)_blockType >= 9)
            rawImage.texture = Resources.Load<Texture>("Image/ItemIcons");
        else 
            rawImage.texture = Resources.Load<Texture>("Image/BlockIcons");

        var datas = _blockSettings.blockDatas;

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].m_blockType == _blockType)
            {       
                _image.uvRect = datas[i].m_rect;
            }
        }
        m_count = item.count;
        _sb.AppendFormat("{0}", item.count);
        _countText.text = _sb.ToString();
        _sb.Clear();

    }
    
    public bool AddItem(int count = 1)
    {
        if(m_count < MaxCount)
        {
            m_count += count;
            _sb.AppendFormat("{0}", m_count);
            _countText.text = _sb.ToString();
            _sb.Clear();
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
            _blockType = BlockType.None;
            _itemSlot.InitSlot();
            _itemSlot = null;
            Inventory.Instance.ReturnToPool(this);
            return;
        }
        _sb.AppendFormat("{0}", m_count);
        _countText.text = _sb.ToString();
        _sb.Clear();
    }
    public void ChangeSlot(ItemSlot slot)
    {
        _itemSlot = slot;
        transform.parent = _itemSlot.transform;
        _rectTransform.anchoredPosition = new Vector2(24f, -24f);
    }
    void Awake()
    {
        _sb = new StringBuilder();
        _blockType = BlockType.None;
        _rectTransform = GetComponent<RectTransform>();
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

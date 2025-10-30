using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System;

public class GameItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    BlockSettings _blockSettings;

    [SerializeField]
    RawImage _image;

    [SerializeField]
    Text _countText;

    [SerializeField]
    ItemSlot _itemSlot;

    ItemStack _itemStack;

    StringBuilder _sb;

    public RectTransform _rectTransform;
    public bool _isHolding;
    Vector3 _offset;

    #region [Property]
    public ItemSlot CurSlot
    {
        get { return _itemSlot; }
    }
    public ItemStack ItemStack => _itemStack;
    #endregion

    void Awake()
    {
        _sb = new StringBuilder();
        _rectTransform = GetComponent<RectTransform>();
    }
    public void InitBlockItem(ItemStack itemStack, ItemSlot slot = null)
    {
        _itemStack = itemStack;

        if ((int)_itemStack.BlockType >= 9) _image.texture = Resources.Load<Texture>("Image/ItemIcons");
        else _image.texture = Resources.Load<Texture>("Image/BlockIcons");

        if (slot != null) _itemSlot = slot;

        var datas = _blockSettings.blockDatas;

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].m_blockType == _itemStack.BlockType)
            {
                _image.uvRect = datas[i].m_rect;
            }
        }

        SetText();
    }
    public bool AddItem(int count = 1)
    {
        if (_itemStack.RemainingCapacity == 0) return false;

        _itemStack.TryAdd(count);
        SetText();

        return true;
    }
    public void UseItem()
    {
        _itemStack.TryRemove(1);
        if (_itemStack.Count <= 0)
        {
            _itemStack = null;
            _itemSlot.InitSlot();
            _itemSlot = null;
            Inventory.Instance.ReturnToPool(this);
            return;
        }
        SetText();
    }
    public void ChangeSlot(ItemSlot slot)
    {
        if(_itemSlot != null) _itemSlot.InitSlot();
        _itemSlot = slot;
        _rectTransform.SetParent(slot.transform);
        _rectTransform.anchoredPosition = new Vector2(24f, -24f);
    }

    #region [Event]
    public void OnPointerDown(PointerEventData e)
    {
        if (Inventory.Instance.GetActive() || GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
        {
            if (Inventory.Instance.IsHold) return;


            if (e.button == PointerEventData.InputButton.Left)
            {
                _isHolding = true;
                _itemSlot.InitSlot();
                _itemSlot = null;
            }
            else if (e.button == PointerEventData.InputButton.Right)
            {
                
                Inventory.Instance.BringItemHalf(this);

                if (_itemStack.Count <= 0)
                {
                    _itemSlot.InitSlot();
                    _itemSlot = null;
                    Inventory.Instance.ReturnToPool(this);
                    return;
                }

                SetText();
            }
        }
    }
    public void OnPointerUp(PointerEventData e)
    {
        if (Inventory.Instance.GetActive() || GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
        {
            if (_isHolding && e.button == PointerEventData.InputButton.Left)
            {
                Inventory.Instance.IsHold = true;
                Inventory.Instance.HoldingItem = this;
            }
        }
            
    }

    #endregion
    public void SetText()
    {
        _sb.AppendFormat("{0}", _itemStack.Count);
        _countText.text = _sb.ToString();
        _sb.Clear();
    }
    void Update()
    {
        if (Inventory.Instance.GetActive() || GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
        {
            if (_isHolding)
            {
                Vector3 curMousePos = Input.mousePosition;
                _rectTransform.position = curMousePos;
            }
        }
    }

}

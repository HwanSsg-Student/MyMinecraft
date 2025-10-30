using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public GameItem _gameItem;
    public bool _isEmpty;

    [SerializeField] GameItem _newGameItem;
    [SerializeField] RectTransform _rect;
    [SerializeField] bool _wasInside;
    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _wasInside = false;
    }
    public void SetItem(GameItem item)
    {
        _gameItem = item;
        _gameItem._isHolding = false;
        _isEmpty = false;
        
    }
    public void InitSlot()
    {
        _isEmpty = true;
        _gameItem = null;
    }



    //public void OnDrop(PointerEventData e)
    //{
    //    if (!Inventory.Instance.GetActive() && !GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf) return;

    //    GameItem item = e.pointerDrag?.GetComponent<GameItem>();

    //    if(item != null)
    //    {
    //        if(_isEmpty && _newGameItem == item)
    //        {
    //            item.ChangeSlot(this);
    //            SetItem(item);
    //        }
    //        else if (!_isEmpty && _newGameItem == item && _gameItem.ItemStack.CanMerge(item.ItemStack))
    //        {
    //            _gameItem.ItemStack.MergeFrom(item.ItemStack);

    //            if(item.ItemStack.Count == 0)
    //            {
    //                Inventory.Instance.ReturnToPool(item);
    //                _newGameItem = null;
    //            }
    //        }
    //    }
    //}

    void Update()
    {
        if (!Inventory.Instance.GetActive() && !GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf) return;

        Vector3 mousePosition = Input.mousePosition;
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(_rect, mousePosition, Inventory.Instance._canvas.worldCamera);
        if (!_wasInside && !isInside) return;
        else if (!_wasInside && isInside) //마우스 커서가 들어옴
        {
            _wasInside = true;
            GameItem item = Inventory.Instance.HoldingItem;
            if (item != null)
            {
                _newGameItem = item;
            }
        }
        else if (_wasInside && !isInside) //마우스 커서가 나감
        {
            _wasInside = false;
            GameItem item = Inventory.Instance.HoldingItem;

            if (item != null)
            {
                if (_newGameItem != null && _newGameItem.gameObject == item.gameObject) _newGameItem = null;

                if (this == Inventory.Instance._slotList[45]) // result slot
                {
                    if (_gameItem == null || item.gameObject != _gameItem.gameObject) return;

                    if (Inventory.Instance.GetActive())
                    {

                        for (int i = 36; i < 40; i++)
                        {
                            if (Inventory.Instance._slotList[i]._gameItem.ItemStack.BlockType != BlockType.None)
                            {
                                Inventory.Instance._slotList[i]._gameItem.UseItem();
                            }
                        }
                    }
                    else if (GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
                    {
                        for (int i = 36; i < 45; i++)
                        {
                            if (Inventory.Instance._slotList[i]._gameItem.ItemStack.BlockType != BlockType.None)
                            {
                                Inventory.Instance._slotList[i]._gameItem.UseItem();
                            }
                        }
                    }
                }
            }
        }
        else if (_wasInside && isInside)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameItem item = Inventory.Instance.HoldingItem;
                if (item != null)
                {
                    if (_isEmpty && _newGameItem == item)
                    {
                        item.ChangeSlot(this);
                        SetItem(item);
                        Inventory.Instance._isHold = false;
                    }
                    else if (!_isEmpty && _newGameItem == item && _gameItem.ItemStack.CanMerge(item.ItemStack))
                    {
                        _gameItem.ItemStack.MergeFrom(item.ItemStack);
                        _gameItem.SetText();
                        if (item.ItemStack.Count == 0)
                        {
                            Inventory.Instance.ReturnToPool(item);
                            _newGameItem = null;
                            Inventory.Instance.IsHold = false;
                        }
                    }
                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                GameItem item = Inventory.Instance.HoldingItem;
                if(item != null)
                {
                    if (_isEmpty && _newGameItem == item)
                    {
                        var newitemStack = item.ItemStack.SplitItem(1);
                        item.SetText();
                        var newGameItem = Inventory.Instance.GetGameItemInPool();
                        newGameItem.InitBlockItem(newitemStack);
                        newGameItem.ChangeSlot(this);
                        newGameItem.gameObject.SetActive(true);
                        SetItem(newGameItem);
                    }
                    else if (!_isEmpty && _newGameItem == item && _gameItem.ItemStack.CanMerge(item.ItemStack))
                    {
                        _gameItem.ItemStack.MergeFrom(item.ItemStack.SplitItem(1));
                        _gameItem.SetText();
                        item.SetText();
                        if (item.ItemStack.Count == 0)
                        {
                            Inventory.Instance.ReturnToPool(item);
                            _newGameItem = null;
                            Inventory.Instance.IsHold = false;
                        }
                    }
                }
            }
        }


    }


}

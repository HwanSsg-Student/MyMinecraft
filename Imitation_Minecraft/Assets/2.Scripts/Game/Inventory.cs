using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Inventory : SingletonMonoBehaviour<Inventory>
{
    public Canvas _canvas;
    [SerializeField]
    GameObject _panelInventory;
    [SerializeField]
    GameObject _panelCraftingTable;
    [SerializeField]
    GameObject _gameItem;
    [SerializeField]
    GameObject _itemSlot;
    [SerializeField]
    GameObject _gridView;
    [SerializeField]
    GameObject _gridViewInTab;
    [SerializeField]
    GameObject _gridViewInTabHand;
    [SerializeField]
    GameObject _gridViewInCraftingArea;
    [SerializeField]
    GameObject _gridViewInResultArea;
    [SerializeField]
    GameObject _gridViewInTable;
    [SerializeField]
    GameObject _gridViewInTableHand;
    [SerializeField]
    GameObject _gridViewInCraftingTable;
    [SerializeField]
    GameObject _gridViewInResultAreaInTable;
    [SerializeField]
    GameObject _slotPoolObj;
    [SerializeField]
    GameObject _cursor;
    [SerializeField]
    GameItem _holdingItem;

    public List<ItemSlot> _slotList = new List<ItemSlot>();
    public List<GameItem> _gameItemList = new List<GameItem>();

    Dictionary<string, BlockType> _recipeInInv = new Dictionary<string, BlockType>();
    Dictionary<string, BlockType> _recipeInTable = new Dictionary<string, BlockType>();
    ObjectPool<GameItem> _gameItemPool;
    ObjectPool<ItemSlot> _itemSlotPool;

    int _curIndex;

    bool _isCompleted;
    
    public bool _isHold;

    #region [Property]
    public int CurIndex
    {
        get { return _curIndex; }
        set { _curIndex = value; }
    }
    public bool IsHold
    {
        get { return _isHold; }
        set { _isHold = value; }
    }
    public GameItem HoldingItem
    {
        get { return _holdingItem; }
        set { _holdingItem = value; }
    }
    #endregion

    void InitRecipe()
    {
        _recipeInInv.Add("-1-1-1-1", BlockType.None);
        _recipeInTable.Add("-1-1-1-1", BlockType.None);

        _recipeInInv.Add("4-1-1-1", BlockType.Plank);
        _recipeInInv.Add("-14-1-1", BlockType.Plank);
        _recipeInInv.Add("-1-14-1", BlockType.Plank);
        _recipeInInv.Add("-1-1-14", BlockType.Plank);

        _recipeInTable.Add("4-1-1-1-1-1-1-1-1", BlockType.Plank);
        _recipeInTable.Add("-14-1-1-1-1-1-1-1", BlockType.Plank);
        _recipeInTable.Add("-1-14-1-1-1-1-1-1", BlockType.Plank);
        _recipeInTable.Add("-1-1-14-1-1-1-1-1", BlockType.Plank);
        _recipeInTable.Add("-1-1-1-14-1-1-1-1", BlockType.Plank);
        _recipeInTable.Add("-1-1-1-1-14-1-1-1", BlockType.Plank);
        _recipeInTable.Add("-1-1-1-1-1-14-1-1", BlockType.Plank);
        _recipeInTable.Add("-1-1-1-1-1-1-14-1", BlockType.Plank);
        _recipeInTable.Add("-1-1-1-1-1-1-1-14", BlockType.Plank);

        _recipeInInv.Add("7-17-1", BlockType.Stick);
        _recipeInInv.Add("-17-17", BlockType.Stick);

        _recipeInTable.Add("7-1-17-1-1-1-1-1", BlockType.Stick);
        _recipeInTable.Add("-17-1-17-1-1-1-1", BlockType.Stick);
        _recipeInTable.Add("-1-17-1-17-1-1-1", BlockType.Stick);
        _recipeInTable.Add("-1-1-17-1-17-1-1", BlockType.Stick);
        _recipeInTable.Add("-1-1-1-17-1-17-1", BlockType.Stick);
        _recipeInTable.Add("-1-1-1-1-17-1-17", BlockType.Stick);

        _recipeInInv.Add("7777", BlockType.CraftingTable);

        _recipeInTable.Add("77-177-1-1-1-1", BlockType.CraftingTable);
        _recipeInTable.Add("-177-177-1-1-1", BlockType.CraftingTable);
        _recipeInTable.Add("-1-1-177-177-1", BlockType.CraftingTable);
        _recipeInTable.Add("-1-1-1-177-177", BlockType.CraftingTable);

        _recipeInTable.Add("777-110-1-110-1", BlockType.Pickax);
    }
    void CreateGameItem()
    {
        _gameItemPool = new ObjectPool<GameItem>(1, () =>
        {
            var obj = Instantiate(_gameItem);
            var item = obj.GetComponent<GameItem>();
            item.gameObject.SetActive(false);
            _gameItemList.Add(item);
            return item;
        });
    }
    void CreateSlot()
    {
        _itemSlotPool = new ObjectPool<ItemSlot>(46, () =>
        {
            var obj = Instantiate(_itemSlot);
            obj.GetComponent<RectTransform>().SetParent(_slotPoolObj.transform);
            var itemSlot = obj.GetComponent<ItemSlot>();
            itemSlot.InitSlot();
            _slotList.Add(itemSlot);
            
            return itemSlot;
        });
        _panelInventory.SetActive(false);
    }
    public void SetSlot(PanelType panelType, bool isVisible)
    {
        if (panelType == PanelType.None && isVisible)
        {
            for (int i = 0; i < 9; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_gridView.transform);
            }
            for (int i = 9; i < 46; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_slotPoolObj.transform);
            }
        }
        if (panelType == PanelType.Inventory && isVisible)
        {
            for (int i = 0; i < 9; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_gridViewInTabHand.transform);
            }
            for (int i = 9; i < 36; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_gridViewInTab.transform);
            }
            for (int i = 36; i < 40; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_gridViewInCraftingArea.transform);
            }
            _slotList[45].GetComponent<RectTransform>().SetParent(_gridViewInResultArea.transform);
        }
        if (panelType == PanelType.CraftingTable && isVisible)
        {
            for (int i = 0; i < 9; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_gridViewInTableHand.transform);
            }
            for (int i = 9; i < 36; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_gridViewInTable.transform);
            }
            for (int i = 36; i < 45; i++)
            {
                var slot = _slotList[i];
                slot.GetComponent<RectTransform>().SetParent(_gridViewInCraftingTable.transform);
            }
            _slotList[45].GetComponent<RectTransform>().SetParent(_gridViewInResultAreaInTable.transform);
        }


    }
    public void Craft()
    {
        string str = "";

        for (int i = 36; i < 40; i++)
        {
            if (_slotList[i]._isEmpty)
            {
                str += "-1";
            }
            else
            {
                str += ((int)_slotList[i]._gameItem.ItemStack.BlockType).ToString();
            }
        }
        if (_recipeInInv.ContainsKey(str) && _recipeInInv[str] != BlockType.None && _slotList[45]._isEmpty)
        {
            var obj = _gameItemPool.Get();
            obj.gameObject.SetActive(true);
            obj.transform.parent = _slotList[45].transform;
            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(24f, -24f);

            ItemStack itemStack = null;

            if (_recipeInInv[str] == BlockType.Plank || _recipeInInv[str] == BlockType.Stick)
            {
                itemStack = new ItemStack(_recipeInInv[str], 4, 64);
            }
            else
            {
                itemStack = new ItemStack(_recipeInInv[str], 1, 64);
            }

            var item = obj.GetComponent<GameItem>();
            _gameItemList.Add(item);
            item.InitBlockItem(itemStack, _slotList[45]);
            _slotList[45].SetItem(item);

        }
        if (!_slotList[45]._isEmpty)
        {
            if (!_recipeInInv.ContainsKey(str) || _recipeInInv[str] != _slotList[45]._gameItem.ItemStack.BlockType)
            {
                ReturnToPool(_slotList[45]._gameItem);
                _slotList[45].InitSlot();
            }
        }

    }
    public void CraftInTable()
    {
        string str = "";

        for (int i = 36; i < 45; i++)
        {
            str += ((int)_slotList[i]._gameItem.ItemStack.BlockType).ToString();
        }

        if (_recipeInTable.ContainsKey(str) && _recipeInTable[str] != BlockType.None && _slotList[45]._isEmpty)
        {
            var obj = _gameItemPool.Get();
            obj.gameObject.SetActive(true);
            obj.transform.parent = _slotList[45].transform;
            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(24f, -24f);

            ItemStack itemStack = null;

            if (_recipeInInv[str] == BlockType.Plank || _recipeInInv[str] == BlockType.Stick)
            {
                itemStack = new ItemStack(_recipeInInv[str], 4, 64);
            }
            else
            {
                itemStack = new ItemStack(_recipeInInv[str], 1, 64);
            }

            var item = obj.GetComponent<GameItem>();
            _gameItemList.Add(item);
            item.InitBlockItem(itemStack, _slotList[45]);
            _slotList[45].SetItem(item);

        }
        if (!_slotList[45]._isEmpty)
        {
            if (!_recipeInTable.ContainsKey(str) || _recipeInTable[str] != _slotList[45]._gameItem.ItemStack.BlockType)
            {
                ReturnToPool(_slotList[45]._gameItem);

                _slotList[45].InitSlot();
            }
        }

    }
    public void BringItemHalf(GameItem gameitem)
    {
        ItemStack itemStack = gameitem.ItemStack.SplitItem(Mathf.CeilToInt(gameitem.ItemStack.Count / 2f));

        var item = _gameItemPool.Get();
        item.gameObject.GetComponent<RectTransform>().SetParent(this.transform);
        item.gameObject.SetActive(true);
        item.InitBlockItem(itemStack);
        item._isHolding = true;

        _isHold = true;
        _holdingItem = item;

        _gameItemList.Add(item);
    }
    public void SelectCursor(int index)
    {
        _cursor.transform.position = _slotList[index].transform.position;
        _curIndex = index;
    }
    public void SetActive()
    {
        _panelInventory.gameObject.SetActive(!GetActive());

        if (_panelInventory.gameObject.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SetSlot(PanelType.Inventory, true);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SetSlot(PanelType.None, true);
        }
    }
    public bool GetActive()
    {
        return _panelInventory.gameObject.activeSelf;
    }
    public void SetItem(BlockType blockType)
    {
        ItemStack itemStack = new ItemStack(blockType, 1, 64);
        for (int i = 0; i < _slotList.Count; i++)
        {
            if (_slotList[i]._isEmpty) //슬롯이 비어있을 때
            {
                var obj = _gameItemPool.Get();
                obj.gameObject.SetActive(true);
                obj._rectTransform.SetParent(_slotList[i].transform);
                //obj.transform.parent = _slotList[i].transform;
                var rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(24f, -24f);
                var item = obj.GetComponent<GameItem>();
                _gameItemList.Add(item);
                item.InitBlockItem(itemStack, _slotList[i]);
                _slotList[i].SetItem(item);

                break;
            }
            else if (!_slotList[i]._isEmpty) //슬롯이 비어있지 않을 때
            {
                if (_slotList[i]._gameItem.ItemStack.BlockType == blockType)
                {
                    var isAdd = _slotList[i]._gameItem.AddItem(1);
                    if (isAdd)
                    {
                        Debug.Log("isAdd");
                        break;
                    }
                    else if (!isAdd)
                    {
                        Debug.Log("not isAdd");
                        continue;
                    }
                }

            }
        }
    }
    public BlockType UseItem()
    {
        BlockType type = _slotList[_curIndex]._gameItem.ItemStack.BlockType;
        if ((int)type < 9)
        {
            _slotList[_curIndex]._gameItem.UseItem();
        }
        return type;
    }
    public void ReturnToPool(GameItem item)
    {
        item.gameObject.SetActive(false);
        _gameItemPool.Set(item);
    }
    public void InitInventory()
    {
        _panelInventory = GameManager.Instance._panels[(int)PanelType.Inventory].gameObject;
        _panelCraftingTable = GameManager.Instance._panels[(int)PanelType.CraftingTable].gameObject;
        _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        InitRecipe();
        CreateSlot();
        CreateGameItem();
        SetSlot(PanelType.None, true);
        _curIndex = 0;
        _isHold = false;
        _cursor.transform.position = new Vector3(400f, 30f, 0f);
        _isCompleted = true;
    }

    public GameItem GetGameItemInPool()
    {
        return _gameItemPool.Get();
    }
    void Update()
    {
        if(_isCompleted)
        {
            if (GetActive())
            {
                Craft();
            }

            if (GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
            {
                CraftInTable();

            }

            if (!GetActive() && !GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
            {
                _isHold = false;
            }
            if (!_isHold)
            {
                _holdingItem = null;
            }
        }
       
    }
}



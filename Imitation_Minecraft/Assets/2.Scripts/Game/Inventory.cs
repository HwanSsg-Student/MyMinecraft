using System.Collections.Generic;
using UnityEngine;
public class Inventory : SingletonMonoBehaviour<Inventory>
{
    [SerializeField]
    GameObject m_panelInventory;
    [SerializeField]
    GameObject m_panelCraftingTable;
    [SerializeField]
    GameObject m_gameItem;
    [SerializeField]
    GameObject m_itemSlot;
    [SerializeField]
    GameObject m_gridView;
    [SerializeField]
    GameObject m_gridViewInTab;
    [SerializeField]
    GameObject m_gridViewInTabHand;
    [SerializeField]
    GameObject m_gridViewInCraftingArea;
    [SerializeField]
    GameObject m_gridViewInResultArea;
    [SerializeField]
    GameObject m_gridViewInTable;
    [SerializeField]
    GameObject m_gridViewInTableHand;
    [SerializeField]
    GameObject m_gridViewInCraftingTable;
    [SerializeField]
    GameObject m_gridViewInResultAreaInTable;
    [SerializeField]
    GameObject m_slotPoolObj;
    [SerializeField]
    GameObject m_cursor;
    [SerializeField]
    GameItem m_holdingItem;

    public List<ItemSlot> m_slotList = new List<ItemSlot>();
    public List<GameItem> m_gameItemList = new List<GameItem>();

    Dictionary<string, BlockType> m_recipeInInv = new Dictionary<string, BlockType>();
    Dictionary<string, BlockType> m_recipeInTable = new Dictionary<string, BlockType>();
    ObjectPool<GameItem> m_gameItemPool;
    ObjectPool<ItemSlot> m_itemSlotPool;

    int m_curIndex;

    bool m_isCompleted;
    [SerializeField]
    bool m_isHold;

    #region [Property]
    public int CurIndex
    {
        get { return m_curIndex; }
        set { m_curIndex = value; }
    }
    public bool IsHold
    {
        get { return m_isHold; }
        set { m_isHold = value; }
    }
    public GameItem HoldingItem
    {
        get { return m_holdingItem; }
        set { m_holdingItem = value; }
    }

    #endregion

    void InitRecipe()
    {
        m_recipeInInv.Add("-1-1-1-1", BlockType.None);
        m_recipeInTable.Add("-1-1-1-1", BlockType.None);

        m_recipeInInv.Add("4-1-1-1", BlockType.Plank);
        m_recipeInInv.Add("-14-1-1", BlockType.Plank);
        m_recipeInInv.Add("-1-14-1", BlockType.Plank);
        m_recipeInInv.Add("-1-1-14", BlockType.Plank);

        m_recipeInTable.Add("4-1-1-1-1-1-1-1-1", BlockType.Plank);
        m_recipeInTable.Add("-14-1-1-1-1-1-1-1", BlockType.Plank);
        m_recipeInTable.Add("-1-14-1-1-1-1-1-1", BlockType.Plank);
        m_recipeInTable.Add("-1-1-14-1-1-1-1-1", BlockType.Plank);
        m_recipeInTable.Add("-1-1-1-14-1-1-1-1", BlockType.Plank);
        m_recipeInTable.Add("-1-1-1-1-14-1-1-1", BlockType.Plank);
        m_recipeInTable.Add("-1-1-1-1-1-14-1-1", BlockType.Plank);
        m_recipeInTable.Add("-1-1-1-1-1-1-14-1", BlockType.Plank);
        m_recipeInTable.Add("-1-1-1-1-1-1-1-14", BlockType.Plank);

        m_recipeInInv.Add("7-17-1", BlockType.Stick);
        m_recipeInInv.Add("-17-17", BlockType.Stick);

        m_recipeInTable.Add("7-1-17-1-1-1-1-1", BlockType.Stick);
        m_recipeInTable.Add("-17-1-17-1-1-1-1", BlockType.Stick);
        m_recipeInTable.Add("-1-17-1-17-1-1-1", BlockType.Stick);
        m_recipeInTable.Add("-1-1-17-1-17-1-1", BlockType.Stick);
        m_recipeInTable.Add("-1-1-1-17-1-17-1", BlockType.Stick);
        m_recipeInTable.Add("-1-1-1-1-17-1-17", BlockType.Stick);

        m_recipeInInv.Add("7777", BlockType.CraftingTable);

        m_recipeInTable.Add("77-177-1-1-1-1", BlockType.CraftingTable);
        m_recipeInTable.Add("-177-177-1-1-1", BlockType.CraftingTable);
        m_recipeInTable.Add("-1-1-177-177-1", BlockType.CraftingTable);
        m_recipeInTable.Add("-1-1-1-177-177", BlockType.CraftingTable);

        m_recipeInTable.Add("777-110-1-110-1", BlockType.Pickax);
    }
    void CreateGameItem()
    {
        m_gameItemPool = new ObjectPool<GameItem>(1, () =>
        {
            var obj = Instantiate(m_gameItem);
            var item = obj.GetComponent<GameItem>();
            item.gameObject.SetActive(false);
            m_gameItemList.Add(item);
            return item;
        });
    }
    void CreateSlot()
    {
        m_itemSlotPool = new ObjectPool<ItemSlot>(46, () =>
        {
            var obj = Instantiate(m_itemSlot);
            obj.transform.parent = m_slotPoolObj.transform;
            var itemSlot = obj.GetComponent<ItemSlot>();
            itemSlot.InitSlot();
            m_slotList.Add(itemSlot);
            //itemSlot.gameObject.SetActive(false);
            return itemSlot;
        });
        m_panelInventory.SetActive(false);
    }
    public void SetSlot(PanelType panelType, bool isVisible)
    {
        if (panelType == PanelType.None && isVisible)
        {
            for (int i = 0; i < 9; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_gridView.transform;
            }
            for (int i = 9; i < 46; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_slotPoolObj.transform;
            }
        }
        if (panelType == PanelType.Inventory && isVisible)
        {
            for (int i = 0; i < 9; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_gridViewInTabHand.transform;
            }
            for (int i = 9; i < 36; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_gridViewInTab.transform;
            }
            for (int i = 36; i < 40; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_gridViewInCraftingArea.transform;
            }
            m_slotList[45].transform.parent = m_gridViewInResultArea.transform;
        }
        if (panelType == PanelType.CraftingTable && isVisible)
        {
            for (int i = 0; i < 9; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_gridViewInTableHand.transform;
            }
            for (int i = 9; i < 36; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_gridViewInTable.transform;
            }
            for (int i = 36; i < 45; i++)
            {
                var slot = m_slotList[i];
                slot.transform.parent = m_gridViewInCraftingTable.transform;
            }
            m_slotList[45].transform.parent = m_gridViewInResultAreaInTable.transform;
        }


    }
    public void Craft()
    {
        string str = "";

        for (int i = 36; i < 40; i++)
        {
            str += ((int)m_slotList[i].Type).ToString();
        }
        if (m_recipeInInv.ContainsKey(str) && m_recipeInInv[str] != BlockType.None && m_slotList[45].IsEmpty)
        {
            var obj = m_gameItemPool.Get();
            obj.gameObject.SetActive(true);
            obj.transform.parent = m_slotList[45].transform;
            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(24f, -24f);

            ItemData itemData = new ItemData() { blockType = m_recipeInInv[str], count = 1, index = m_gameItemList.Count };

            if (m_recipeInInv[str] == BlockType.Plank || m_recipeInInv[str] == BlockType.Stick)
            {
                itemData.count = 4;
            }

            var item = obj.GetComponent<GameItem>();
            m_gameItemList.Add(item);
            item.InitBlockItem(itemData, m_slotList[45]);
            m_slotList[45].SetItem(item);

        }
        if (!m_slotList[45].IsEmpty)
        {
            if (!m_recipeInInv.ContainsKey(str) || m_recipeInInv[str] != m_slotList[45].m_gameItem.Type)
            {
                ReturnToPool(m_slotList[45].m_gameItem);
                m_slotList[45].InitSlot();
            }
        }

    }
    public void CraftInTable()
    {
        string str = "";

        for (int i = 36; i < 45; i++)
        {
            str += ((int)m_slotList[i].Type).ToString();
        }

        if (m_recipeInTable.ContainsKey(str) && m_recipeInTable[str] != BlockType.None && m_slotList[45].IsEmpty)
        {
            var obj = m_gameItemPool.Get();
            obj.gameObject.SetActive(true);
            obj.transform.parent = m_slotList[45].transform;
            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(24f, -24f);

            ItemData itemData = new ItemData() { blockType = m_recipeInTable[str], count = 1, index = m_gameItemList.Count };

            if (m_recipeInTable[str] == BlockType.Plank || m_recipeInTable[str] == BlockType.Stick)
            {
                itemData.count = 4;
            }

            var item = obj.GetComponent<GameItem>();
            m_gameItemList.Add(item);
            item.InitBlockItem(itemData, m_slotList[45]);
            m_slotList[45].SetItem(item);

        }
        if (!m_slotList[45].IsEmpty)
        {
            if (!m_recipeInTable.ContainsKey(str) || m_recipeInTable[str] != m_slotList[45].m_gameItem.Type)
            {
                ReturnToPool(m_slotList[45].m_gameItem);

                m_slotList[45].InitSlot();
            }
        }

    }
    public void BringItemHalf(GameItem gameitem, ItemSlot slot)
    {
        ItemData data = new ItemData() { count = gameitem.Count / 2, blockType = gameitem.Type, index = m_gameItemList.Count };
        if (data.count == 0) return;

        m_holdingItem = m_gameItemPool.Get();
        m_holdingItem.gameObject.transform.parent = this.transform;
        m_holdingItem._rectTransform.transform.position = Input.mousePosition;
        m_holdingItem.gameObject.SetActive(true);


        var item = m_holdingItem.GetComponent<GameItem>();
        m_gameItemList.Add(item);
        item.InitBlockItem(data, slot);
        m_isHold = true;
    }
    public void SelectCursor(int index)
    {
        m_cursor.transform.position = m_slotList[index].transform.position;
        m_curIndex = index;
    }
    public void SetActive()
    {
        m_panelInventory.gameObject.SetActive(!GetActive());

        if (m_panelInventory.gameObject.activeSelf)
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
        return m_panelInventory.gameObject.activeSelf;
    }
    public void SetItem(BlockType blockType)
    {
        ItemData itemData = new ItemData() { blockType = blockType, count = 1, index = m_gameItemList.Count };
        for (int i = 0; i < m_slotList.Count; i++)
        {
            if (m_slotList[i].IsEmpty) //슬롯이 비어있을 때
            {
                var obj = m_gameItemPool.Get();
                obj.gameObject.SetActive(true);
                obj.transform.parent = m_slotList[i].transform;
                var rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(24f, -24f);
                var item = obj.GetComponent<GameItem>();
                m_gameItemList.Add(item);
                item.InitBlockItem(itemData, m_slotList[i]);
                m_slotList[i].SetItem(item);

                break;
            }
            else if (!m_slotList[i].IsEmpty) //슬롯이 비어있지 않을 때
            {
                if (m_slotList[i].Type == blockType)
                {
                    var isAdd = m_slotList[i].m_gameItem.AddItem();
                    if (isAdd) break;
                    else if (!isAdd) continue;
                }

            }
        }
    }
    public BlockType UseItem()
    {
        BlockType type = m_slotList[m_curIndex].m_gameItem.Type;
        if ((int)type < 9)
        {
            m_slotList[m_curIndex].m_gameItem.UseItem();
        }
        return type;
    }
    public void ReturnToPool(GameItem item)
    {
        item.gameObject.SetActive(false);
        m_gameItemPool.Set(item);
    }

    public void InitInventory()
    {
        
        m_panelInventory = GameManager.Instance.m_panels[(int)PanelType.Inventory].gameObject;
        m_panelCraftingTable = GameManager.Instance.m_panels[(int)PanelType.CraftingTable].gameObject;
        InitRecipe();
        CreateSlot();
        CreateGameItem();
        SetSlot(PanelType.None, true);
        m_curIndex = 0;
        m_isHold = false;
        m_cursor.transform.position = new Vector3(400f, 30f, 0f);
        m_isCompleted = true;
    }

    void Start()
    {
        //m_isCompleted = false;

    }

    void Update()
    {
        if(m_isCompleted)
        {
            if (GetActive())
            {
                Craft();
                if (m_isHold)
                {
                    m_holdingItem._rectTransform.position = Input.mousePosition;
                }
            }
            if (GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
            {
                CraftInTable();
                if (m_isHold)
                {
                    m_holdingItem._rectTransform.position = Input.mousePosition;
                }
            }

            if (!GetActive() && !GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
            {
                m_isHold = false;
            }
            if (!m_isHold)
            {
                m_holdingItem = null;
            }
        }
       
    }
}



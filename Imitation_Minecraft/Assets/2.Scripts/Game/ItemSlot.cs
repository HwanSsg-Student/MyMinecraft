using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public GameItem m_gameItem;
    [SerializeField]
    bool m_isEmpty;

    BlockType m_blockType;

    GameItem m_newGameItem;
    
#region [Property]
    public bool IsEmpty 
    {
        get { return m_isEmpty; }
        set { m_isEmpty = value; }
    }
    public BlockType Type
    {
        get { return m_blockType; }
        set { m_blockType = value; }
    }
    #endregion

    // 아이템이 옮겨져 왔을 때

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("GameItem"))
        {
            if (!m_isEmpty)
            {
                m_newGameItem = other.GetComponent<GameItem>();
            }
        }
    }
    private void OnTriggerStay(Collider other) 
    {
        if (!Inventory.Instance.GetActive() && !GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf) return;
        if(other.CompareTag("GameItem"))
        {
            var item = other.GetComponent<GameItem>();
            
            if((!item.IsDragging && !Inventory.Instance.IsHold))
            {
                if (m_isEmpty)
                {
                    item.ChangeSlot(this);
                    SetItem(item);
                    return;
                }
                else
                {
                    if (m_newGameItem == item && item.gameObject != m_gameItem.gameObject)
                    {
                        if (m_blockType == item.Type)
                        {
                            m_gameItem.AddItem(item.Count);
                            Inventory.Instance.ReturnToPool(item);
                            m_newGameItem = null;
                        }
                        else
                        {
                            item.ChangeSlot(item.CurSlot);
                        }
                    }
                }
                
            }

        }

    }

    // 아이템을 옮길 때
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("GameItem"))
        {
            if(m_newGameItem != null)
            {
                m_newGameItem = null;
            }

            var item = other.GetComponent<GameItem>();
            if(this == Inventory.Instance.m_slotList[45])
            {
                if(m_gameItem == null || item.gameObject != m_gameItem.gameObject )
                {
                    return;
                }

                if(Inventory.Instance.GetActive())
                {
                    
                    for (int i = 36; i < 40; i++)
                    {
                        if (Inventory.Instance.m_slotList[i].Type != BlockType.None)
                        {
                            Inventory.Instance.m_slotList[i].m_gameItem.UseItem();
                        }
                    }
                }
                if(GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
                {
                    for (int i = 36; i < 45; i++)
                    {
                        if (Inventory.Instance.m_slotList[i].Type != BlockType.None)
                        {
                            Inventory.Instance.m_slotList[i].m_gameItem.UseItem();
                        }
                    }
                }

            }
            InitSlot();
        }
    }


    #region [Public Method]
    public void SetItem(GameItem item)
    {
        m_gameItem = item;
        m_blockType = item.Type;
        m_isEmpty = false;
    }
    public void InitSlot()
    {
        m_blockType = BlockType.None;
        m_isEmpty = true;
        m_gameItem = null;
    }

    #endregion
    private void Update()
    {
        if(m_isEmpty)
        {
            InitSlot();
        }
    }
}

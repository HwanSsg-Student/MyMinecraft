using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
[System.Serializable]
public class PlayerController : MonoBehaviourPunCallbacks
{
    #region [변수]
    Camera m_camera;        //카메라
    [SerializeField]
    GameObject m_blockPos;  //블럭이 손에 생기는 위치
    [SerializeField]
    GameObject m_currentBlock;//현재 블럭
    [SerializeField]
    CharacterController m_charController; //플레이어 컨트롤러 컴포넌트
    [SerializeField]
    float m_moveSpeed = 3f; //이동 속도

    [SerializeField]
    float sensitivity;      //감도
    float clampAngle = 90f; //회전할 수 있는 최대 각도
    float rotX;             //X축 회전
    float rotY;             //Y축 회전



    [SerializeField]
    GameObject m_aim;       //조준선
    [SerializeField]
    Animator m_animator;    //플레이어 애니메이션 컴포넌트

    [SerializeField]
    GameObject[] m_blocks;


    Ray m_ray;              //Ray
    RaycastHit m_hit;       //RayInfo

    [SerializeField]
    bool m_initCompleted;

    #endregion

    #region [Property]
    public bool InitCompleted
    {
        get { return m_initCompleted; }
        set { m_initCompleted = value; }
    }

    #endregion
    #region [Method]
    public void InitPlayerCtrl()
    {
        try
        {
            m_camera = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Camera>();
            m_aim = GameObject.FindGameObjectWithTag("Aim");
            m_blocks = Resources.LoadAll<GameObject>("Prefabs/Block/");

            if (photonView.IsMine)
            {
                Camera.main.gameObject.SetActive(false);
                m_camera.gameObject.SetActive(true);
            }

            GetComponent<Renderer>().material.color = photonView.IsMine ? Color.green : Color.red;
            m_initCompleted = true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
        
    }
    private void SetInventory()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Inventory.Instance.SetActive();
        }
    }
    private void SetGameMenu()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager.Instance.VisibleGameMenu();
        }
#else 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.VisibleGameMenu();
        }
#endif
    }
    private void SetCraftingTable()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            Inventory.Instance.SetSlot(PanelType.None, true);
            GameManager.Instance.m_panels[(int)PanelType.CraftingTable].SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
#else 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Inventory.Instance.SetSlot(PanelType.None, true);
            GameManager.Instance.m_panels[(int)PanelType.CraftingTable].SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
#endif
    }
    private void SetBlock()     //블럭 선택
    {
        BlockType blockType = BlockType.None;
        if (Input.GetKeyDown(KeyCode.Alpha1) || (Inventory.Instance.CurIndex == 0))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[0].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[0].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(0);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || (Inventory.Instance.CurIndex == 1))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[1].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[1].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(1);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || (Inventory.Instance.CurIndex == 2))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[2].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[2].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(2);

        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || (Inventory.Instance.CurIndex == 3))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[3].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[3].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(3);

        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || (Inventory.Instance.CurIndex == 4))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[4].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[4].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(4);

        }
        if (Input.GetKeyDown(KeyCode.Alpha6) || (Inventory.Instance.CurIndex == 5))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[5].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[5].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) || (Inventory.Instance.CurIndex == 6))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[6].IsEmpty)
            {
                m_currentBlock = null;
            }
            else
            {
                blockType = Inventory.Instance.m_slotList[6].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) || (Inventory.Instance.CurIndex == 7))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[7].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[7].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) || (Inventory.Instance.CurIndex == 8))
        {
            if (m_currentBlock != null)
            {
                Destroy(m_currentBlock);
            }
            if (Inventory.Instance.m_slotList[8].IsEmpty) m_currentBlock = null;
            else
            {
                blockType = Inventory.Instance.m_slotList[8].Type;
                m_currentBlock = Instantiate(m_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(8);
        }
        if (m_currentBlock != null)
        {
            if (Inventory.Instance.m_slotList[Inventory.Instance.CurIndex].IsEmpty)
            {
                Destroy(m_currentBlock);
            }
            else
            {
                m_currentBlock.transform.SetParent(m_blockPos.transform);
                if((int)blockType < 9)
                {
                    m_currentBlock.transform.localRotation = m_blockPos.transform.localRotation;
                    m_currentBlock.transform.localScale = Vector3.one * 0.3f;
                }
                else if(blockType == BlockType.Stick)
                {
                    m_currentBlock.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 30f));
                }
                else if(blockType == BlockType.Pickax)
                {
                    m_currentBlock.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 60f));
                }
                m_currentBlock.transform.position = m_blockPos.transform.position;

            }
        }
    }
    private void PlayerMove()   //플레이어 이동
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);    // Z축
        Vector3 right = transform.TransformDirection(Vector3.right);        // X축
        Vector3 moveVector = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        m_charController.Move(moveVector.normalized * m_moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * m_moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += Vector3.down * m_moveSpeed * Time.deltaTime;
        }
    }
    private void PlayerRotate() //1인칭 플레이어 화면
    {
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    private void InstallAndBreakBlock()  //블럭 설치
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if(leftClick || rightClick)
        {
            Debug.Log("Click");
        }

        if (leftClick || rightClick)
        {
            if (Physics.Raycast(m_ray.origin, transform.forward, out m_hit, 4f, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (m_hit.transform.gameObject != null)
                {
                    m_animator.Play("BreaknCreate", 0);
                    Vector3 targetBlockPos;
                    Vector3 targetOfUseBlockPos;
                    if (rightClick)
                    {
                        targetBlockPos = m_hit.point - transform.forward * 0.01f;
                        targetOfUseBlockPos = m_hit.point + transform.forward * 0.01f;
                    }
                    else
                    {
                        targetBlockPos = m_hit.point + transform.forward * 0.01f;
                        targetOfUseBlockPos = targetBlockPos;
                    }
                        
                    int chunkPosX = Mathf.FloorToInt(targetBlockPos.x / 16f);
                    int chunkPosZ = Mathf.FloorToInt(targetBlockPos.z / 16f);

                    Vector2 coord = new Vector2(chunkPosX, chunkPosZ);

                    int chunkPosXForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.x / 16f);
                    int chunkPosZForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.z / 16f);

                    int bix = Mathf.FloorToInt(targetBlockPos.x) - (chunkPosX * 16);
                    int biy = Mathf.FloorToInt(targetBlockPos.y);
                    int biz = Mathf.FloorToInt(targetBlockPos.z) - (chunkPosZ * 16);

                    if (TerrainGenerator.m_terrainChunkDictionary.ContainsKey(coord))
                    {
                        TerrainChunk tc = TerrainGenerator.m_terrainChunkDictionary[coord];

                        if (rightClick) //Install
                        {
                            int bixForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.x) - (chunkPosXForTheUse * 16);
                            int biyForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.y);
                            int bizForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.z) - (chunkPosZForTheUse * 16);

                            if (tc.GetBlockType(bixForTheUse, biyForTheUse, bizForTheUse) == BlockType.CraftingTable)
                            {
                                GameManager.Instance.m_panels[(int)PanelType.CraftingTable].SetActive(true);
                                Inventory.Instance.SetSlot(PanelType.CraftingTable, true);
                                Cursor.visible = true;
                                Cursor.lockState = CursorLockMode.None;
                                return;
                            }

                            if (m_currentBlock == null)
                            {
                                return;
                            }
                            else
                            {
                                BlockType type = Inventory.Instance.UseItem();
                                if (type == BlockType.None || (int)type >= 9) return;
                                else tc.UpdateTerrainChunkByPlayer(bix, biy, biz, type);
                            }
                        }
                        else if (leftClick)  //break
                        {
                            Inventory.Instance.SetItem(tc.GetBlockType(bix, biy, biz));
                            tc.UpdateTerrainChunkByPlayer(bix, biy, biz, BlockType.Air);
                        }

                        if(!TerrainGenerator.m_changedTerrainChunkDic.ContainsKey(coord))
                        {
                            TerrainGenerator.m_changedTerrainChunkDic.Add(coord, tc);
                            GameManager.Instance.SaveCoord(coord);
                            Debug.Log("Add");
                        }
                    }
                }
            }
        }
    }

    #endregion

    void Awake()
    {
        m_initCompleted = false;
        rotX = transform.rotation.eulerAngles.x;
        rotY = transform.rotation.eulerAngles.y;
        
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(photonView.IsMine && m_initCompleted)
        {
            if (GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetCraftingTable();
            }
            if (!Inventory.Instance.GetActive() && !GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetGameMenu();
            }
            if (!GameManager.Instance.IsGameMenuVisible && !GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetInventory();
            }
            if (!GameManager.Instance.IsGameMenuVisible && !Inventory.Instance.GetActive() && !GameManager.Instance.m_panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetBlock();
                PlayerMove();
                PlayerRotate();

                m_ray = m_camera.ScreenPointToRay(m_aim.transform.position);
                InstallAndBreakBlock();
            }
        }

    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 5f);

    }

#endif
}

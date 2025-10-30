using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
[System.Serializable]
public class PlayerController : MonoBehaviourPunCallbacks
{
    #region [Variable]
    Camera _camera;        
    [SerializeField]
    GameObject _blockPos;    //블럭이 손에 생기는 위치
    [SerializeField]
    GameObject _currentBlock;//현재 블럭
    [SerializeField]
    CharacterController _charController; //플레이어 컨트롤러 컴포넌트
    [SerializeField]
    float _moveSpeed = 9f;   //이동 속도

    [SerializeField]
    float _sensitivity;      //감도
    float _clampAngle = 90f; //회전할 수 있는 최대 각도
    float _rotX;             //X축 회전
    float _rotY;             //Y축 회전

    [SerializeField]
    GameObject _aim;         //조준선
    [SerializeField]
    Animator _animator;      //플레이어 애니메이션 컴포넌트

    [SerializeField]
    GameObject[] _blocks;

    Ray _ray;                //Ray
    RaycastHit _hit;         //RayInfo

    [SerializeField]
    bool _initCompleted;

    #endregion

    #region [Property]
    public bool InitCompleted
    {
        get { return _initCompleted; }
        set { _initCompleted = value; }
    }

    #endregion
    #region [Method]
    public void InitPlayerCtrl()
    {
        try
        {
            _camera = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Camera>();
            _aim = GameObject.FindGameObjectWithTag("Aim");
            _blocks = Resources.LoadAll<GameObject>("Prefabs/Block/");

            if (photonView.IsMine)
            {
                Camera.main.gameObject.SetActive(false);
                _camera.gameObject.SetActive(true);
            }

            GetComponent<Renderer>().material.color = photonView.IsMine ? Color.green : Color.red;
            _initCompleted = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
        
    }
    void SetInventory()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Inventory.Instance.SetActive();
        }
    }
    void SetGameMenu()
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
    void SetCraftingTable()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            Inventory.Instance.SetSlot(PanelType.None, true);
            GameManager.Instance._panels[(int)PanelType.CraftingTable].SetActive(false);
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
    void SetBlock()     //블럭 선택
    {
        BlockType blockType = BlockType.None;
        if (Input.GetKeyDown(KeyCode.Alpha1) || (Inventory.Instance.CurIndex == 0))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[0]._isEmpty) _currentBlock = null;
            else
            {
                try
                {
                    blockType = Inventory.Instance._slotList[0]._gameItem.ItemStack.BlockType;
                    _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
                }
                catch(Exception)
                {
                    Debug.Log(blockType);
                }
            }
            Inventory.Instance.SelectCursor(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || (Inventory.Instance.CurIndex == 1))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[1]._isEmpty) _currentBlock = null;
            else
            {
                blockType = Inventory.Instance._slotList[1]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(1);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || (Inventory.Instance.CurIndex == 2))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[2]._isEmpty) _currentBlock = null;
            else
            {
                blockType = Inventory.Instance._slotList[2]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(2);

        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || (Inventory.Instance.CurIndex == 3))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[3]._isEmpty) _currentBlock = null;
            else
            {
                blockType = Inventory.Instance._slotList[3]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(3);

        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || (Inventory.Instance.CurIndex == 4))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[4]._isEmpty) _currentBlock = null;
            else
            {
                blockType = Inventory.Instance._slotList[4]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(4);

        }
        if (Input.GetKeyDown(KeyCode.Alpha6) || (Inventory.Instance.CurIndex == 5))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[5]._isEmpty) _currentBlock = null;
            else
            {
                blockType = Inventory.Instance._slotList[5]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) || (Inventory.Instance.CurIndex == 6))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[6]._isEmpty)
            {
                _currentBlock = null;
            }
            else
            {
                blockType = Inventory.Instance._slotList[6]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) || (Inventory.Instance.CurIndex == 7))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[7]._isEmpty) _currentBlock = null;
            else
            {
                blockType = Inventory.Instance._slotList[7]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) || (Inventory.Instance.CurIndex == 8))
        {
            if (_currentBlock != null)
            {
                Destroy(_currentBlock);
            }
            if (Inventory.Instance._slotList[8]._isEmpty) _currentBlock = null;
            else
            {
                blockType = Inventory.Instance._slotList[8]._gameItem.ItemStack.BlockType;
                _currentBlock = Instantiate(_blocks[(int)blockType - 1]);
            }
            Inventory.Instance.SelectCursor(8);
        }
        if (_currentBlock != null)
        {
            if (Inventory.Instance._slotList[Inventory.Instance.CurIndex]._isEmpty)
            {
                Destroy(_currentBlock);
            }
            else
            {
                _currentBlock.transform.SetParent(_blockPos.transform);
                if((int)blockType < 9)
                {
                    _currentBlock.transform.localRotation = _blockPos.transform.localRotation;
                    _currentBlock.transform.localScale = Vector3.one * 0.3f;
                }
                else if(blockType == BlockType.Stick)
                {
                    _currentBlock.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 30f));
                }
                else if(blockType == BlockType.Pickax)
                {
                    _currentBlock.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 60f));
                }
                _currentBlock.transform.position = _blockPos.transform.position;
            }
        }
    }
    void PlayerMove()   //플레이어 이동
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);    // Z축
        Vector3 right = transform.TransformDirection(Vector3.right);        // X축
        Vector3 moveVector = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        _charController.Move(moveVector.normalized * _moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * _moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += Vector3.down * _moveSpeed * Time.deltaTime;
        }
    }
    void PlayerRotate() //1인칭 플레이어 화면
    {
        _rotX += -(Input.GetAxis("Mouse Y")) * _sensitivity * Time.deltaTime;
        _rotY += Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;

        _rotX = Mathf.Clamp(_rotX, -_clampAngle, _clampAngle);
        Quaternion rot = Quaternion.Euler(_rotX, _rotY, 0);
        transform.rotation = rot;
    }
    void InstallAndBreakBlock()  //블럭 설치
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (leftClick || rightClick)
        {
            if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, 4f, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (_hit.transform.gameObject != null)
                {
                    _animator.Play("BreaknCreate", 0);
                    Vector3 targetBlockPos;
                    Vector3 targetOfUseBlockPos;

                    if (rightClick)
                    {
                        targetBlockPos = _hit.point - transform.forward * 0.01f;
                        targetOfUseBlockPos = _hit.point + transform.forward * 0.01f;
                    }
                    else
                    {
                        targetBlockPos = _hit.point + transform.forward * 0.01f;
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

                        if (rightClick) 
                        {
                            int bixForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.x) - (chunkPosXForTheUse * 16);
                            int biyForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.y);
                            int bizForTheUse = Mathf.FloorToInt(targetOfUseBlockPos.z) - (chunkPosZForTheUse * 16);

                            if (tc.GetBlockType(bixForTheUse, biyForTheUse, bizForTheUse) == BlockType.CraftingTable)
                            {
                                GameManager.Instance._panels[(int)PanelType.CraftingTable].SetActive(true);
                                Inventory.Instance.SetSlot(PanelType.CraftingTable, true);
                                Cursor.visible = true;
                                Cursor.lockState = CursorLockMode.None;
                                return;
                            }
                            if (_currentBlock == null)
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
                        else if (leftClick) 
                        {
                            if(tc.GetBlockType(bix, biy, biz) != BlockType.Air) Inventory.Instance.SetItem(tc.GetBlockType(bix, biy, biz));
                            tc.UpdateTerrainChunkByPlayer(bix, biy, biz, BlockType.Air);
                        }

                        if(!TerrainGenerator.m_changedTerrainChunkDic.ContainsKey(coord))
                        {
                            TerrainGenerator.m_changedTerrainChunkDic.Add(coord, tc);
                            GameManager.Instance.SaveCoord(coord);
                        }
                    }
                }
            }
        }
    }

    #endregion

    void Awake()
    {
        _initCompleted = false;
        _rotX = transform.rotation.eulerAngles.x;
        _rotY = transform.rotation.eulerAngles.y;
        
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(photonView.IsMine && _initCompleted)
        {
            if (GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetCraftingTable();
            }
            if (!Inventory.Instance.GetActive() && !GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetGameMenu();
            }
            if (!GameManager.Instance.IsGameMenuVisible && !GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetInventory();
            }
            if (!GameManager.Instance.IsGameMenuVisible && !Inventory.Instance.GetActive() && !GameManager.Instance._panels[(int)PanelType.CraftingTable].activeSelf)
            {
                SetBlock();
                PlayerMove();
                PlayerRotate();

                _ray = _camera.ScreenPointToRay(_aim.transform.position);
                InstallAndBreakBlock();
            }
        }

    }

#if UNITY_EDITOR

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, 5f);
    //}

#endif
}

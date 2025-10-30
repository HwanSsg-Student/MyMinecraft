using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;

public class GameManager : SingletonDontDestroy<GameManager>
{
    StringBuilder _sb;
    int _clickCount;
    bool _isSuccess;

    [SerializeField]
    string _title;

    [SerializeField]
    public PlayerController _player;

    List<Vector2> _coordList = new List<Vector2>();

    #region [Property]
    public PlayerController Player
    {
        get { return _player; }
        set { _player = value; }
    }
    public string CurTitle
    {
        get { return _title; }
        set { _title = value; }
    }
    public bool IsGameMenuVisible
    {
        get { return _panels[(int)PanelType.GameMenu] == null ? false : _panels[(int)PanelType.GameMenu].activeSelf; }
        private set { }
    }
    #endregion

    #region [Chunk_Variable]
    [SerializeField]
    HeightMapSettings _heightMapSettings;
    [SerializeField]
    BlockSettings _blockSettings;
    [SerializeField]
    PlayerSettings _playerSettings;
    #endregion 

    #region [UI_Variable]
    PanelType _prevPanel;
    
    public Button[] _buttons;
    public GameObject[] _panels;
    public ScrollViewController[] _scrollViewCtrl;

    [SerializeField]
    Text[] _texts;


    #endregion

    public void InitObject(SceneType sceneType)
    {
        if (sceneType == SceneType.Game)
        {
            _panels[(int)PanelType.Inventory] = GameObject.FindGameObjectWithTag("Panel_Inventory");
            _panels[(int)PanelType.CraftingTable] = GameObject.FindGameObjectWithTag("Panel_CraftingTable");
            _panels[(int)PanelType.GameMenu] = GameObject.FindGameObjectWithTag("Panel_GameMenu");
            
            _buttons[(int)ButtonType.BtnBackToGame] = GameObject.FindGameObjectWithTag("Btn_BackToGame").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnSaveAndQuitToTitle] = GameObject.FindGameObjectWithTag("Btn_SaveAndQuitToTitle").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnBackToGame].onClick.AddListener(OnBackToGame);
            _buttons[(int)ButtonType.BtnSaveAndQuitToTitle].onClick.AddListener(OnSaveAndQuitToTitle);
            _panels[(int)PanelType.CraftingTable].gameObject.SetActive(false);

            var players = GameObject.FindGameObjectsWithTag("Player");
            NetworkManager.Instance.InitPlayers(players);

            var terrainGen = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            terrainGen.InitTerrainGenerator();
            Inventory.Instance.InitInventory();
        }
        else if (sceneType == SceneType.Title)
        {
            _panels[(int)PanelType.MainTitle] = GameObject.FindGameObjectWithTag("Panel_MainTitle");
            _panels[(int)PanelType.SinglePlay] = GameObject.FindGameObjectWithTag("Panel_SinglePlay");
            _panels[(int)PanelType.MultiPlay] = GameObject.FindGameObjectWithTag("Panel_MultiPlay");
            _panels[(int)PanelType.CreateWorld] = GameObject.FindGameObjectWithTag("Panel_CreateWorld");

            _scrollViewCtrl[0] = GameObject.FindGameObjectWithTag("ScrView_Single").GetComponent<ScrollViewController>();
            _scrollViewCtrl[1] = GameObject.FindGameObjectWithTag("ScrView_Multi").GetComponent<ScrollViewController>();

            _buttons[(int)ButtonType.BtnSinglePlay] = GameObject.FindGameObjectWithTag("Btn_SinglePlay").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnMultiPlay] = GameObject.FindGameObjectWithTag("Btn_MultiPlay").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnPlay] = GameObject.FindGameObjectWithTag("Btn_Play").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnCreate] = GameObject.FindGameObjectWithTag("Btn_Create").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnCancelAtSinglePlay] = GameObject.FindGameObjectWithTag("Btn_CancelAtSinglePlay").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnWorldType] = GameObject.FindGameObjectWithTag("Btn_WorldType").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnCreateAtCreateWorld] = GameObject.FindGameObjectWithTag("Btn_CreateAtCreateWorld").GetComponent<Button>();
            _buttons[(int)ButtonType.BtnCancelAtCreateWorld] = GameObject.FindGameObjectWithTag("Btn_CancelAtCreateWorld").GetComponent<Button>();

            _buttons[(int)ButtonType.BtnSinglePlay].onClick.AddListener(OnSinglePlayer);
            _buttons[(int)ButtonType.BtnMultiPlay].onClick.AddListener(OnMultiPlayer);
            _buttons[(int)ButtonType.BtnCreate].onClick.AddListener(OnCreateWorld);
            _buttons[(int)ButtonType.BtnCancelAtSinglePlay].onClick.AddListener(OnCancle);
            _buttons[(int)ButtonType.BtnWorldType].onClick.AddListener(OnWorldType);
            _buttons[(int)ButtonType.BtnCreateAtCreateWorld].onClick.AddListener(OnCreateAtCreateWorld);
            _buttons[(int)ButtonType.BtnCancelAtCreateWorld].onClick.AddListener(OnCancle);

            _texts[(int)TextType.InputTitle] = GameObject.FindGameObjectWithTag("Text_InputTitle").GetComponent<Text>();
            _texts[(int)TextType.WorldType] = GameObject.FindGameObjectWithTag("Text_WorldType").GetComponent<Text>();

            _panels[(int)PanelType.MainTitle].SetActive(true);
            _panels[(int)PanelType.SinglePlay].SetActive(false);
            _panels[(int)PanelType.MultiPlay].SetActive(false);
            _panels[(int)PanelType.CreateWorld].SetActive(false);
            _prevPanel = PanelType.None;

            _isSuccess = false;
        }
    }
    public void SaveCoord(Vector2 coord)
    {
        _coordList.Add(coord);
    }
    #region [UI_Method]
    public void VisibleGameMenu()
    {
        if (_panels[(int)PanelType.GameMenu].activeSelf)
        {
            _panels[(int)PanelType.GameMenu].SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            _panels[(int)PanelType.GameMenu].SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void OnBackToGame()
    {
        VisibleGameMenu();
    }
    public void OnSaveAndQuitToTitle()
    {
        OnSaveInfo();
    }

    public void OnSaveInfo()
    {
        DBManager.Instance.Reference.Child("Minecraft").Child(_title).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("IsFault");
                return;
            }
            if (task.IsCompleted)
            {
                _playerSettings.m_pos = _player.transform.position;

                var json_playerSettings = JsonUtility.ToJson(_playerSettings);
                var snapShot = task.Result;
                snapShot.Child("m_playerSettings").Reference.SetRawJsonValueAsync(json_playerSettings);

                foreach (Vector2 coord in _coordList)
                {
                    TerrainChunk tc = TerrainGenerator.m_changedTerrainChunkDic[coord];
                    string str = string.Format($"{coord.x}_{coord.y}");
                    var blockData = tc.BlockData;
                    var blockDataJson = JsonConvert.SerializeObject(blockData);
                    snapShot.Child("Chunks").Child(str).Reference.SetRawJsonValueAsync(blockDataJson);
                }
                _player.InitCompleted = false;

                LoadSceneManager.Instance.LoadScene(SceneType.Title);
                
                Debug.Log("OnSaveInfo");
            }
        });

    }
    public void OnCreateAtCreateWorld() //생성
    {
        Debug.Log("생성");
        _heightMapSettings.noiseSettings.seed = UnityEngine.Random.Range(0, 100000);

        var json_heightMapSettings = JsonUtility.ToJson(_heightMapSettings);

        _title = _texts[(int)TextType.InputTitle].text;
        
        DBManager.Instance.Reference.Child("Minecraft").Child(_title).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) return;
            if (task.IsCompleted)
            {
                var snapShot = task.Result;
                snapShot.Child("heightSettings").Reference.SetRawJsonValueAsync(json_heightMapSettings);

                snapShot.Child("date").Reference.SetValueAsync(DateTime.Now.ToString());
            }
        });
    }
    public void OnSinglePlayer()
    {
        _panels[(int)PanelType.SinglePlay].gameObject.SetActive(true);
        _panels[(int)PanelType.MainTitle].gameObject.SetActive(false);
        _prevPanel = PanelType.MainTitle;
        if (!_isSuccess)
        {
            DBManager.Instance.Reference.Child("Minecraft").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Fault");
                }
                if (task.IsCompleted)
                {
                    
                    var snapShot = task.Result;
                    if (snapShot.HasChildren && snapShot.ChildrenCount > 0)
                    {
                        if (_scrollViewCtrl[0].gameObject.transform.GetChild(0).GetChild(0).childCount != snapShot.ChildrenCount)
                        {

                            for (int i = 0; i < snapShot.ChildrenCount; i++)
                            {
                                _scrollViewCtrl[0].AddNewUiObject(i);
                            }
                        }
                    }
                    _isSuccess = true;
                }
            });
        }
    }
    public void OnMultiPlayer()
    {
        _panels[(int)PanelType.MultiPlay].gameObject.SetActive(true);
        _panels[(int)PanelType.MainTitle].gameObject.SetActive(false);
        _prevPanel = PanelType.MainTitle;
        if (!_isSuccess)
        {
            NetworkManager.Instance.CheckRoom();

            _isSuccess = true;
        }
    }

    public void OnCreateWorld()
    {
        _panels[(int)PanelType.CreateWorld].gameObject.SetActive(true);
        _panels[(int)PanelType.SinglePlay].gameObject.SetActive(false);
        _prevPanel = PanelType.SinglePlay;
    }
    public void OnCancle()
    {
        foreach (var panel in _panels)
        {
            if (panel == null) continue;
            if (panel.activeSelf)
            {
                panel.SetActive(false);
                break;
            }
        }
        _panels[(int)_prevPanel].gameObject.SetActive(true);
        _prevPanel = PanelType.MainTitle;
        _isSuccess = false;

    }
    public void OnWorldType()
    {
        _clickCount++;
        if (_clickCount % 2 == 0)
        {
            _sb.AppendFormat("세계 유형 : 기본");
            _texts[(int)TextType.WorldType].text = _sb.ToString();
            BlockSettings.isFlat = false;
        }
        else if (_clickCount % 2 != 0)
        {
            _sb.AppendFormat("세계 유형 : 평지");
            _texts[(int)TextType.WorldType].text = _sb.ToString();
            BlockSettings.isFlat = true;
        }
        _sb.Clear();
    }
    #endregion


    protected override void OnAwake()
    {
        _clickCount = 0;
        _sb = new StringBuilder();
        _sb.Clear();
        InitObject(SceneType.Title);
    }


}

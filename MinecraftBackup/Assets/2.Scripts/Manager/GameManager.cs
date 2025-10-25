using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;

public class GameManager : SingletonDontDestory<GameManager>
{
    StringBuilder m_sb;
    int m_clickCount;
    bool m_isSuccess;

    [SerializeField]
    string m_title;
    [SerializeField]
    public PlayerController m_player;



    List<Vector2> m_vectorList = new List<Vector2>();

    #region [Property]
    public PlayerController Player
    {
        get { return m_player; }
        set { m_player = value; }
    }
    public string CurTitle
    {
        get { return m_title; }
        set { m_title = value; }
    }
    public bool IsGameMenuVisible
    {
        get { return m_panels[(int)PanelType.GameMenu] == null ? false : m_panels[(int)PanelType.GameMenu].activeSelf; }
        private set { }
    }
    #endregion

    #region [Chunk_Variable]
    [SerializeField]
    HeightMapSettings m_heightMapSettings;
    [SerializeField]
    BlockSettings m_blockSettings;
    [SerializeField]
    PlayerSettings m_playerSettings;
    #endregion 

    #region [UI_Variable]
    PanelType m_prevPanel;
    
    public Button[] m_buttons;
    public GameObject[] m_panels;
    public ScrollViewController[] m_scrollViewCtrl;


    [SerializeField]
    Text[] m_texts;


    #endregion
    public void UpdateChunk(GameObject chunkObject, TerrainChunk TerrainChunk, int index)
    {
        DBManager.Instance.Reference.Child("Minecraft").Child(m_title).Child("TerrainChuck").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) return;
            if (task.IsCompleted)
            {
                var chunk = JsonUtility.ToJson(TerrainChunk);
                var chunkObjectPos = JsonUtility.ToJson(chunkObject.transform.position);
                var snapShot = task.Result;
                snapShot.Child(index.ToString()).Reference.SetRawJsonValueAsync(chunk);
                snapShot.Child(index.ToString()).Reference.SetRawJsonValueAsync(chunkObjectPos);
                snapShot.Child(index.ToString()).Child("Active").Reference.SetValueAsync(chunkObject.activeSelf);
            }
        });


    }
    public void InitObject(SceneType sceneType)
    {
        if (sceneType == SceneType.Game)
        {
            m_panels[(int)PanelType.Inventory] = GameObject.FindGameObjectWithTag("Panel_Inventory");
            m_panels[(int)PanelType.CraftingTable] = GameObject.FindGameObjectWithTag("Panel_CraftingTable");
            m_panels[(int)PanelType.GameMenu] = GameObject.FindGameObjectWithTag("Panel_GameMenu");
            
            m_buttons[(int)ButtonType.BtnBackToGame] = GameObject.FindGameObjectWithTag("Btn_BackToGame").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnSaveAndQuitToTitle] = GameObject.FindGameObjectWithTag("Btn_SaveAndQuitToTitle").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnBackToGame].onClick.AddListener(OnBackToGame);
            m_buttons[(int)ButtonType.BtnSaveAndQuitToTitle].onClick.AddListener(OnSaveAndQuitToTitle);
            m_panels[(int)PanelType.CraftingTable].gameObject.SetActive(false);

            DBManager.Instance.Load(m_title);
            var players = GameObject.FindGameObjectsWithTag("Player");
            NetworkManager.Instance.InitPlayers(players);

            //m_player.InitPlayerCtrl();
            var terrainGen = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<TerrainGenerator>();
            terrainGen.InitTerrainGenerator();
            Inventory.Instance.InitInventory();
        }
        else if (sceneType == SceneType.Title)
        {
            m_panels[(int)PanelType.MainTitle] = GameObject.FindGameObjectWithTag("Panel_MainTitle");
            m_panels[(int)PanelType.SinglePlay] = GameObject.FindGameObjectWithTag("Panel_SinglePlay");
            m_panels[(int)PanelType.MultiPlay] = GameObject.FindGameObjectWithTag("Panel_MultiPlay");
            m_panels[(int)PanelType.CreateWorld] = GameObject.FindGameObjectWithTag("Panel_CreateWorld");

            m_scrollViewCtrl[0] = GameObject.FindGameObjectWithTag("ScrView_Single").GetComponent<ScrollViewController>();
            m_scrollViewCtrl[1] = GameObject.FindGameObjectWithTag("ScrView_Multi").GetComponent<ScrollViewController>();

            m_buttons[(int)ButtonType.BtnSinglePlay] = GameObject.FindGameObjectWithTag("Btn_SinglePlay").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnMultiPlay] = GameObject.FindGameObjectWithTag("Btn_MultiPlay").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnPlay] = GameObject.FindGameObjectWithTag("Btn_Play").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnCreate] = GameObject.FindGameObjectWithTag("Btn_Create").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnCancelAtSinglePlay] = GameObject.FindGameObjectWithTag("Btn_CancelAtSinglePlay").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnWorldType] = GameObject.FindGameObjectWithTag("Btn_WorldType").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnCreateAtCreateWorld] = GameObject.FindGameObjectWithTag("Btn_CreateAtCreateWorld").GetComponent<Button>();
            m_buttons[(int)ButtonType.BtnCancelAtCreateWorld] = GameObject.FindGameObjectWithTag("Btn_CancelAtCreateWorld").GetComponent<Button>();

            m_buttons[(int)ButtonType.BtnSinglePlay].onClick.AddListener(OnSinglePlayer);
            m_buttons[(int)ButtonType.BtnMultiPlay].onClick.AddListener(OnMultiPlayer);
            m_buttons[(int)ButtonType.BtnCreate].onClick.AddListener(OnCreateWorld);
            m_buttons[(int)ButtonType.BtnCancelAtSinglePlay].onClick.AddListener(OnCancle);
            m_buttons[(int)ButtonType.BtnWorldType].onClick.AddListener(OnWorldType);
            m_buttons[(int)ButtonType.BtnCreateAtCreateWorld].onClick.AddListener(OnCreateAtCreateWorld);
            m_buttons[(int)ButtonType.BtnCancelAtCreateWorld].onClick.AddListener(OnCancle);

            m_texts[(int)TextType.InputTitle] = GameObject.FindGameObjectWithTag("Text_InputTitle").GetComponent<Text>();
            m_texts[(int)TextType.WorldType] = GameObject.FindGameObjectWithTag("Text_WorldType").GetComponent<Text>();

            m_panels[(int)PanelType.MainTitle].SetActive(true);
            m_panels[(int)PanelType.SinglePlay].SetActive(false);
            m_panels[(int)PanelType.MultiPlay].SetActive(false);
            m_panels[(int)PanelType.CreateWorld].SetActive(false);
            m_prevPanel = PanelType.None;

            m_isSuccess = false;
        }
    }
    public void SaveCoord(Vector2 coord)
    {
        m_vectorList.Add(coord);
    }
    #region [UI_Method]
    public void VisibleGameMenu()
    {
        if (m_panels[(int)PanelType.GameMenu].activeSelf)
        {
            m_panels[(int)PanelType.GameMenu].SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            m_panels[(int)PanelType.GameMenu].SetActive(true);
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
        DBManager.Instance.Reference.Child("Minecraft").Child(m_title).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("IsFault");
                return;
            }
            if (task.IsCompleted)
            {
                
                m_playerSettings.m_pos = m_player.transform.position;

                var json_playerSettings = JsonUtility.ToJson(m_playerSettings);
                var snapShot = task.Result;
                snapShot.Child("m_playerSettings").Reference.SetRawJsonValueAsync(json_playerSettings);

                foreach (Vector2 coord in m_vectorList)
                {
                    TerrainChunk tc = TerrainGenerator.m_changedTerrainChunkDic[coord];   
                    string str = coord.x.ToString() + coord.y.ToString();
                    var blockData = tc.m_blockData;
                    var blockDataJson = JsonConvert.SerializeObject(blockData);
                    snapShot.Child("Chunks").Child(str).Reference.SetRawJsonValueAsync(blockDataJson);
                }
                m_player.InitCompleted = false;

                LoadSceneManager.Instance.LoadScene(SceneType.Title);
                
                Debug.Log("OnSaveInfo");
            }
        });

    }
    public void OnCreateAtCreateWorld() //생성
    {
        Debug.Log("생성");
        m_heightMapSettings.noiseSettings.seed = UnityEngine.Random.Range(0, 100000);
        var json_heightMapSettings = JsonUtility.ToJson(m_heightMapSettings);
        var json_blockSettings = JsonUtility.ToJson(m_blockSettings);
        m_title = m_texts[(int)TextType.InputTitle].text;
        DBManager.Instance.Reference.Child("Minecraft").Child(m_title).SetValueAsync("");
        DBManager.Instance.Reference.Child("Minecraft").Child(m_title).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) return;
            if (task.IsCompleted)
            {
                var snapShot = task.Result;
                snapShot.Child("heightSettings").Reference.SetRawJsonValueAsync(json_heightMapSettings);
                snapShot.Child("blockSettings").Reference.SetRawJsonValueAsync(json_blockSettings);
                snapShot.Child("date").Reference.SetValueAsync(DateTime.Now.ToString());
            }
        });
    }
    public void OnSinglePlayer()
    {
        m_panels[(int)PanelType.SinglePlay].gameObject.SetActive(true);
        m_panels[(int)PanelType.MainTitle].gameObject.SetActive(false);
        m_prevPanel = PanelType.MainTitle;
        if (!m_isSuccess)
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
                        if (m_scrollViewCtrl[0].gameObject.transform.GetChild(0).GetChild(0).childCount != snapShot.ChildrenCount)
                        {

                            for (int i = 0; i < snapShot.ChildrenCount; i++)
                            {
                                m_scrollViewCtrl[0].AddNewUiObject(i);
                            }
                        }
                    }
                        m_isSuccess = true;
                }
            });
        }
    }
    public void OnMultiPlayer()
    {
        m_panels[(int)PanelType.MultiPlay].gameObject.SetActive(true);
        m_panels[(int)PanelType.MainTitle].gameObject.SetActive(false);
        m_prevPanel = PanelType.MainTitle;
        if (!m_isSuccess)
        {
            NetworkManager.Instance.CheckRoom();

            m_isSuccess = true;
        }
    }

    public void OnCreateWorld()
    {
        m_panels[(int)PanelType.CreateWorld].gameObject.SetActive(true);
        m_panels[(int)PanelType.SinglePlay].gameObject.SetActive(false);
        m_prevPanel = PanelType.SinglePlay;
    }
    public void OnCancle()
    {
        foreach (var panel in m_panels)
        {
            if (panel == null) continue;
            if (panel.activeSelf)
            {
                panel.SetActive(false);
                break;
            }
        }
        m_panels[(int)m_prevPanel].gameObject.SetActive(true);
        m_prevPanel = PanelType.MainTitle;
        m_isSuccess = false;

    }
    public void OnWorldType()
    {
        m_clickCount++;
        if (m_clickCount % 2 == 0)
        {
            m_sb.AppendFormat("세계 유형 : 기본");
            m_texts[(int)TextType.WorldType].text = m_sb.ToString();
            BlockSettings.isFlat = false;
        }
        else if (m_clickCount % 2 != 0)
        {
            m_sb.AppendFormat("세계 유형 : 평지");
            m_texts[(int)TextType.WorldType].text = m_sb.ToString();
            BlockSettings.isFlat = true;
        }
        m_sb.Clear();
    }
    #endregion


    protected override void OnAwake()
    {
        InitObject(SceneType.Title);
        m_isSuccess = false;

        m_clickCount = 0;
        m_sb = new StringBuilder();
        m_sb.Clear();

        
    }


}

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static NetworkManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PhotonView.DontDestroyOnLoad(gameObject);
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        else
        {
            PhotonView.Destroy(gameObject);
        }
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CheckRoom()
    {
        if (GameManager.Instance._scrollViewCtrl[1].gameObject.transform.GetChild(0).GetChild(0).childCount != 1)
        {
            GameManager.Instance._scrollViewCtrl[1].AddNewUiObject(0);
        }
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        PhotonNetwork.JoinOrCreateRoom(GameManager.Instance.CurTitle, roomOptions, null);
       
    }
    public override void OnJoinedRoom()
    {
        var player = PhotonNetwork.Instantiate("Player", new Vector3(0f, 15f, 0f), Quaternion.identity);
        
        GameManager.Instance.Player = player.GetComponent<PlayerController>();
        LoadSceneManager.Instance.LoadScene(SceneType.Game);
    }
    
    [PunRPC]
    public void InitPlayers(GameObject[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerController>().InitPlayerCtrl();
        }
    }
}

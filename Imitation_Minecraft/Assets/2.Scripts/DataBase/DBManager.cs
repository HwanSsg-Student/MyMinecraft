using System;
using System.Collections;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Firebase;
using Photon.Pun;
public class DBManager : SingletonDontDestroy<DBManager>
{
    DatabaseReference _reference;
    FirebaseApp _app;
    [SerializeField]
    HeightMapSettings _heightMapSettings;
    [SerializeField]
    PlayerSettings _playerSettings;
    
    #region [Property]
    public DatabaseReference Reference
    {
        get { return _reference; }
        private set { }
    }
    #endregion

    protected override void OnAwake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {

            if (task.IsFaulted || task.IsCanceled || task.Result != DependencyStatus.Available)
            {
                Debug.LogError($"Firebase 준비 실패: {(task.IsFaulted ? task.Exception?.ToString() : task.Result.ToString())}");
                return;
            }

            _app = FirebaseApp.DefaultInstance;
            _reference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("시작");
        });
    }
    
    #region [public Method]

    public void Load(string title, object sender = null)
    {
        var reference = _reference.Child("Minecraft").Child(title);

        reference.Child("heightSettings").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) return;
            if (task.IsCompleted)
            {
                
                var snapShot = task.Result;
                string json_heightSettings = snapShot.GetRawJsonValue();
                if(!string.IsNullOrEmpty(json_heightSettings))
                {
                    try
                    {
                        JsonUtility.FromJsonOverwrite(json_heightSettings, _heightMapSettings);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        throw;
                    }
                }
            }
        });
        reference.Child("m_playerSettings").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) return;
            if (task.IsCompleted)
            {
                if (!PhotonNetwork.IsMasterClient) return;
                var snapShot = task.Result;
                string json_playerInfo = snapShot.GetRawJsonValue();
                if (!string.IsNullOrEmpty(json_playerInfo))
                {
                    try
                    {
                        JsonUtility.FromJsonOverwrite(json_playerInfo, _playerSettings);
                        GameManager.Instance._player.transform.position = _playerSettings.m_pos;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        throw;
                    }
                }
            }
        });       
    }


    #endregion

    //#region [EventHandler]
    //void HandleChildAddedEvent(object sender, ChildChangedEventArgs args)
    //{
    //    if (args.DatabaseError != null)
    //    {
    //        Debug.LogError(args.DatabaseError.Message);
    //        return;
    //    }
    //}
    //#endregion


}

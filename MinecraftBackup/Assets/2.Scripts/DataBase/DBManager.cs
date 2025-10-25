using System;
using System.Collections;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Firebase;
using Photon.Pun;
public class DBManager : SingletonDontDestory<DBManager>
{
    DatabaseReference m_reference;
    FirebaseApp m_app;
    [SerializeField]
    HeightMapSettings m_heightMapSettings;
    [SerializeField]
    PlayerSettings m_playerSettings;
    
    #region [Property]
    public DatabaseReference Reference
    {
        get { return m_reference; }
        private set { }
    }
    #endregion
    #region [Coroutine]
    IEnumerator Coroutine_Action()
    {

        yield return null;
    }
    #endregion

    protected override void OnAwake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            m_app = FirebaseApp.DefaultInstance;
            m_reference = FirebaseDatabase.DefaultInstance.RootReference;
        });
        

        Debug.Log("½ÃÀÛ");
        
    }
    
    #region [public Method]



    public void Load(string title, object sender = null)
    {
        var reference = m_reference.Child("Minecraft").Child(title);

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
                        JsonUtility.FromJsonOverwrite(json_heightSettings, m_heightMapSettings);
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
                        JsonUtility.FromJsonOverwrite(json_playerInfo, m_playerSettings);
                        GameManager.Instance.m_player.transform.position = m_playerSettings.m_pos;
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

    #region [EventHandler]
    void EventHandler_ChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

    }
    #endregion


}

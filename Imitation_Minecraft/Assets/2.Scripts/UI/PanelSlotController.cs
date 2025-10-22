using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class PanelSlotController : MonoBehaviour
{
    [SerializeField]
    Text m_title;
    [SerializeField]
    Text m_log;

    [SerializeField]
    int m_index;
    public int Index
    {
        get { return m_index; }
        set { m_index = value; }
    }
    public string Title
    {
        get { return m_title.text; }
        private set { }
    }
    void Start()
    {
        m_title = transform.GetChild(1).GetComponent<Text>();
        m_log = transform.GetChild(2).GetComponent<Text>();
        SetTitle(false);
    }

    void SetTitle(bool isMulti)
    {
        if(!isMulti)
        {
            DBManager.Instance.Reference.Child("Minecraft").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted) return;
                if (task.IsCompleted)
                {
                    var snapShot = task.Result;
                    var dic = (Dictionary<string, object>)snapShot.Value;
                    m_title.text = dic.ElementAt(m_index).Key;
                    SetLog();
                }
            });
        }
    }
    void SetLog()
    {
        DBManager.Instance.Reference.Child("Minecraft").Child(m_title.text).Child("date").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) return;
            if (task.IsCompleted)
            {
                m_log.text = task.Result.Value.ToString();
            }
        });

    }
    
}

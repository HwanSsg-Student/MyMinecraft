using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class PanelSlotController : MonoBehaviour
{
    [SerializeField]
    Text _title;
    [SerializeField]
    Text _log;
    [SerializeField]
    int _index;
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }
    public string Title
    {
        get { return _title.text; }
        private set { }
    }
    void Start()
    {
        _title = transform.GetChild(1).GetComponent<Text>();
        _log = transform.GetChild(2).GetComponent<Text>();
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
                    _title.text = dic.ElementAt(_index).Key;
                    SetLog();
                }
            });
        }
    }
    void SetLog()
    {
        DBManager.Instance.Reference.Child("Minecraft").Child(_title.text).Child("date").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) return;
            if (task.IsCompleted)
            {
                _log.text = task.Result.Value.ToString();
            }
        });

    }
    
}

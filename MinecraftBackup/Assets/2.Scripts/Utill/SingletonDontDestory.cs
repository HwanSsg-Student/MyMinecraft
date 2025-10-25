using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDontDestory<T> : MonoBehaviour where T : SingletonDontDestory<T>
{
    public static T Instance { get; private set; }
    protected virtual void OnStart() { }
    protected virtual void OnAwake() { }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            OnAwake();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        if (Instance == (T)this)
        {
            OnStart();
        }
    }

    
}

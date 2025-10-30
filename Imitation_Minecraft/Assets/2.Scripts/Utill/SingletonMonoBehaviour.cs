using UnityEngine;

public class SingletonMonoBehaviour<T> :MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    public static T Instance { get; set; }
    protected virtual void OnStart() { }
    protected virtual void OnAwake() { }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            OnAwake();
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

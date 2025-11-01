using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections;
using UnityEngine;
using System;

public enum SceneType
{
    None = -1,
    Title,
    Game,
    Max
}

public class LoadSceneManager : SingletonDontDestroy<LoadSceneManager>
{
    public SceneType _sceneType = SceneType.None;
    public event Action<float> OnProgress;

    bool _finished;
    void Start()
    {
        _finished = false;
        FindObjectOfType<Panel_Loading>().OnCompleted += HandlerOnCompleted;

    }

    public void LoadScene(SceneType sceneNum)
    {
        if(sceneNum == SceneType.Game) StartCoroutine(Coroutine_Load(sceneNum));
        else
        {
            var op = SceneManager.LoadSceneAsync((int)sceneNum);
            op.completed += (temp) =>
            {
                GameManager.Instance.InitObject(sceneNum);
                _sceneType = sceneNum;
            };
        }
    }
    public void OnPlay()
    {
        var btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<PanelSlotController>();
        if (btn != null)
        {
            GameManager.Instance.CurTitle = btn.Title.ToString();
            DBManager.Instance.Load(btn.Title);
        }
        GameManager.Instance.OnLoading();
        NetworkManager.Instance.Connect();   
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;

#else
        UnityEngine.Application.Quit();
#endif
    }

    IEnumerator Coroutine_Load(SceneType sceneNum)
    {
        var op = SceneManager.LoadSceneAsync((int)sceneNum);
        op.allowSceneActivation = false;

        op.completed += (temp) =>
        {
            GameManager.Instance.InitObject(sceneNum);
            _sceneType = sceneNum;
        };

        while (op.progress < 0.9f)
        {
            
            OnProgress?.Invoke(Mathf.Clamp01(op.progress));
            yield return null;
        }
        
        while (!_finished) yield return null;

        yield return new WaitForSeconds(1f); // 마무리 UI 연출

        op.allowSceneActivation = _finished;
        _finished = false;
    }

    void HandlerOnCompleted(bool finish)
    {
        _finished = finish;
    }

}

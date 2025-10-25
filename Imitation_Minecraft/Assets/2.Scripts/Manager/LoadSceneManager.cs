using UnityEngine.SceneManagement;
using UnityEditor;
using System.Threading.Tasks;
public enum SceneType
{
    None = -1,
    Title,
    Game,
    Max
}

public class LoadSceneManager : SingletonDontDestroy<LoadSceneManager>
{
    public SceneType m_sceneType = SceneType.None;
    public void LoadScene(SceneType sceneNum)
    {
        var asyncOperation = SceneManager.LoadSceneAsync((int)sceneNum);
        
        asyncOperation.completed += (temp) => 
        { 
            GameManager.Instance.InitObject(sceneNum);
            m_sceneType = sceneNum;
        };
                
    }
    public void OnPlay()
    {
        var btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<PanelSlotController>();
        if (btn != null)
        {
            GameManager.Instance.CurTitle = btn.Title.ToString();
            DBManager.Instance.Load(btn.Title);
        }
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



}

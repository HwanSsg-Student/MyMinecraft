using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
public class ScrollViewController : MonoBehaviour
{ 
    [SerializeField]
    ScrollRect m_scrollRect;
    [SerializeField]
    float m_space = 50f;
    [SerializeField]
    GameObject m_UIPrefab;
    
    List<RectTransform> m_UIList = new List<RectTransform>();

    StringBuilder sb;

    void Start()
    {
        sb = new StringBuilder();
        sb.Clear();
        
    }

    public void AddNewUiObject(int index)
    {
        var uiPrefab = Instantiate(m_UIPrefab, m_scrollRect.content);
        var newUI = uiPrefab.GetComponent<RectTransform>();
        var panelSlotController = uiPrefab.GetComponent<PanelSlotController>();
        panelSlotController.Index = index;

        m_UIList.Add(newUI);

        float y = 0f;
        for (int i = 0; i < m_UIList.Count; i++)
        {
            m_UIList[i].anchoredPosition = new Vector2(0f, -y);
            y += m_UIList[i].sizeDelta.y + m_space;
        }
        m_scrollRect.content.sizeDelta = new Vector2(m_scrollRect.content.sizeDelta.x, y);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
public class ScrollViewController : MonoBehaviour
{ 
    [SerializeField]
    ScrollRect _scrollRect;
    [SerializeField]
    float _space;
    [SerializeField]
    GameObject _UIPrefab;
    
    List<RectTransform> _UIList = new List<RectTransform>();

    StringBuilder _sb;

    void Start()
    {
        _sb = new StringBuilder();
        _sb.Clear();
    }

    public void AddNewUiObject(int index)
    {
        var uiPrefab = Instantiate(_UIPrefab, _scrollRect.content);
        var newUI = uiPrefab.GetComponent<RectTransform>();
        var panelSlotController = uiPrefab.GetComponent<PanelSlotController>();
        panelSlotController.Index = index;

        _UIList.Add(newUI);

        float y = 0f;
        for (int i = 0; i < _UIList.Count; i++)
        {
            _UIList[i].anchoredPosition = new Vector2(0f, -y);
            y += _UIList[i].sizeDelta.y + _space;
            
        }
        _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, y);

    }

}

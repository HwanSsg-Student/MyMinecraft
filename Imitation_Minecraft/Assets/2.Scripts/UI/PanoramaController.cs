using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PanoramaController : MonoBehaviour
{
    [SerializeField]
    float m_speed;

    RectTransform m_rectTransform;

    
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        m_rectTransform.position += Vector3.left * m_speed * Time.deltaTime;
        
        if (m_rectTransform.position.x <= -1280f)
        {
            m_rectTransform.position = new Vector3(3840, 360);
        }
    }
    
    

}

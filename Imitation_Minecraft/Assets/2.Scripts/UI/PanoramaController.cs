using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PanoramaController : MonoBehaviour
{
    [SerializeField]
    float _speed;

    Image _image;
    Material _material;
    
    void Start()
    {
        _image = GetComponent<Image>();
        _material = _image.material;
    }

    void Update()
    {
        _material.mainTextureOffset += Vector2.right * _speed * Time.deltaTime;
    }
    
    

}

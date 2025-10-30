using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    [SerializeField]
    bool isOn;
    void Start()
    {
        gameObject.SetActive(!isOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

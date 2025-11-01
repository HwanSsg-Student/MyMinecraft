using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Loading : MonoBehaviour
{
    public Slider _slider;
    public Image _image;

    public event Action<bool> OnCompleted;
    float _progress;
    float _target;

    Coroutine _coroutine;
    void Awake()
    {
        _slider = transform.GetChild(0).GetChild(1).GetComponent<Slider>();
        _image = transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Image>();
        _progress = 0f;
        _target = 0f;
    }
    void OnDisable()
    {
        LoadSceneManager.Instance.OnProgress -= HandlerOnProgress;
    }
    void Start()
    {
        try
        {
            LoadSceneManager.Instance.OnProgress += HandlerOnProgress;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        } 
    }

    public void HandlerOnProgress(float target01)
    {
        _target = Mathf.Clamp01(target01);
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(SmoothProgress(_target));
    }
    IEnumerator SmoothProgress(float target01)
    {
        _progress = target01;
        while (!Mathf.Approximately(_progress, 1f))
        {
            _progress = Mathf.MoveTowards(_progress, 1f, 0.01f);
            ApplyUI();
            yield return null;
        }
        ApplyUI();
        OnCompleted?.Invoke(true);
    }
    void ApplyUI()
    {
        if (_slider) _slider.value = _progress;
        if (_image) _image.fillAmount = 1f - _progress;
    }
}

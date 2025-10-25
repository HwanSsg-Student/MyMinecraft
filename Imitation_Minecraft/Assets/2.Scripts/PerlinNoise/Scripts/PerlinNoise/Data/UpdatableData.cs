using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


[DataContract]
public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    [JsonIgnore] public bool autoUpdate;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
    }
    public void NotifyOfUpdatedValues()
    {
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UpdateableData : ScriptableObject
{
    public event System.Action onValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate() {
        if (autoUpdate) {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
    }

    public void NotifyOfUpdatedValues() {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (onValuesUpdated != null) {
            onValuesUpdated();
        }
    }


}

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
            NotifyOfUpdatedValues();
        }
    }

    public void NotifyOfUpdatedValues() {
        if (onValuesUpdated != null) {
            onValuesUpdated();
        }
    }


}

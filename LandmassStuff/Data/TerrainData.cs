using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdateableData
{
    public float uniformScale = 1f;
    public bool useFlatShading;
    public bool useFallOff;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
}

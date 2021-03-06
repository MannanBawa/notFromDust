﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdateableData {
    
    public Noise.NormalizeMode normalizeMode;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistence;
    public float lacunarity;

    public int seed;
    public Vector2 offset;


    protected override void OnValidate() {
        if (lacunarity < 1f) {
           lacunarity = 1f;
        }
        
        if (octaves < 0) {
           octaves = 0;
        }

        base.OnValidate();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size) {
        float[,] map = new float[size,size];

        for (int i = 0; i < size; i++) {
            for(int j = 0; j < size; j++) {
                // (value from 0~1) * 2 - 1 = value from -1~1
                float x = i/(float)size * 2 - 1;
                float y = j/(float)size * 2 - 1;

                //find which value is closest to the edge of the square
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i,j] = Evaluate(value);
            }
        } 
        return map;
    }

    static float Evaluate(float value) {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(value, a)/(Mathf.Pow(value, a) + (b - b*value));
    }
}

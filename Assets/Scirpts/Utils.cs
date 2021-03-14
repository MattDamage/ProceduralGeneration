using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils  {


    public static float fBM(float x, float y, int oct, float persistance)
    {
       
             float total = 0;
        float freqency = 1;
        float amplituide = 1;
        float maxValue = 0;
         
        for (int i = 0; i < oct; i++)
        {

            total += Mathf.PerlinNoise(x  * freqency, y  * freqency * amplituide);
            maxValue += amplituide;
            amplituide *= persistance;

            freqency *= 2;
        }
        return total / maxValue;
    }

}

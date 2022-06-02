using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ExtensionMethods
{

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float Remap(int value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static double IntRound(this int Value, int Digit)
    {
        double Temp = Math.Pow(10.0, Digit);
        return Math.Round(Value * Temp) / Temp;
    }

    public static int GetMaxValue(List<int> integers)
    {
        int max = 0;

        for(int i = 0; i < integers.Count; i++)
        {
            max = i == 0 ? integers[i] : max;    
            max = max < integers[i] ? integers[i] : max;
        }
        return max;
    }
}

using System.Collections;

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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

}

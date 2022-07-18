using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TimeUtils
{
    public static string GetCurrentDate()
    {
        return DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss tt"));
    }
}

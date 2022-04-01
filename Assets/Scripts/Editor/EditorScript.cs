using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorScript : EditorWindow
{
    [MenuItem("Custom/TestSymbol")]
    private static void Build()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "TEST");
    }

    [MenuItem("Custom/RealSymbol")]
    private static void Test()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "REAL");
    }
}

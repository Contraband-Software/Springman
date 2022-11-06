using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InternalLogger
{
    private static ILogger logger = Debug.unityLogger;
    private static string kTag = "CONTRABAND";

    public static void Log(string message)
    {
        logger.Log(kTag, message);
    }
}

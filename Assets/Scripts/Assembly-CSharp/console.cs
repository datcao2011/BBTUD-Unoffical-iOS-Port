using UnityEngine;
using System;

public class console : MonoBehaviour
{
    [HideInCallstack]
    public static void log(object log)
    {
        Debug.Log(log);
    }

    [HideInCallstack]
    public static void error(object log)
    {
        Debug.LogError(log);
    }

    [HideInCallstack]
    public static void warn(object log)
    {
        Debug.LogWarning(log);
    }

    [HideInCallstack]
    public static void info(object log)
    {
        Debug.Log(log);
    }

    [HideInCallstack]
    public static void logException(Exception log)
    {
        Debug.LogException(log);
    }
}
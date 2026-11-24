using System;
using UnityEngine;

public class VersionInput : MonoBehaviour
{
    public static Version currentVersion
    {
        get
        {
            return new Version(Application.version);
        }
    }

    public static string unityVersion
    {
        get
        {
            return Application.unityVersion;
        }
    }
}

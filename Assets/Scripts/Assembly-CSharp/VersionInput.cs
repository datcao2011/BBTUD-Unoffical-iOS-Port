using System;
using UnityEngine;

public class VersionInput : MonoBehaviour
{
    public static Version currentVersion
    {
        get
        {
            return new Version(3, 0, 605);
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

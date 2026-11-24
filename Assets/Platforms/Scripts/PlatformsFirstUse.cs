#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;
using System;

namespace Platforms
{
    [InitializeOnLoad]
    public class PlatformsFirstUse
    {
        static PlatformsFirstUse()
        {
            EditorApplication.update += Update;
        }

        static bool isSessionStarted;

        static void Update()
        {
            if (!isSessionStarted)
            {
                isSessionStarted = true;
                PlatformsWindowScript.SetupScreenFinished = EditorPrefs.GetBool("PFFU-SS2");
                if (EditorApplication.isUpdating)
                {
                    return;
                }
                if (EditorApplication.isCompiling)
                {
                    return;
                }
                PlatformsUpdater.CheckForUpdates();
            }
            EditorApplication.update -= Update;
        }
    }
}
#endif
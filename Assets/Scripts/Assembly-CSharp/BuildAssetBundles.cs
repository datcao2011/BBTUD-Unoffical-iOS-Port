using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System;

public class BuildAssetBundles : MonoBehaviour
{
    const string Version = "3.0.5";
    const string FileName = "Baldi's Basics Ultra Decompile OS";

    // [MenuItem("Build/Test")]
    // static void GameTest()
    // {
    //     string Path = Application.dataPath + @"\Build\Game";
    //     ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $"/k tar acvf \"Baldi's Basics TUD Windows V{Version} x64.zip\" \"BBTUD x64\"")
    //     {
    //         WorkingDirectory = Path
    //     };
    //     Process.Start(info); //Start cmd process
    // }

    [MenuItem("Build/Game/Windows")]
    static void GameWin()
    {
        UnityEngine.Debug.Log("Building on Windows");
        string Path = Application.dataPath.Replace("Assets", "Build") + @"\Game";
        List<string> Scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            Scenes.Add(scene.path);
        }
        // 64
        UnityEngine.Debug.Log("Building Windows x64 " + DateTime.Now.ToString());
        BuildPlayerOptions win64 = new BuildPlayerOptions()
        {
            locationPathName = Path + "/BBTUD x64/" + FileName + ".exe",
            scenes = Scenes.ToArray(),
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone
        };
        BuildReport report;
        report = BuildPipeline.BuildPlayer(win64);
        if (report.summary.totalErrors <= 0)
        {
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $"/c tar acvf \"Baldi's Basics TUD Windows V{Version} x64.zip\" \"BBTUD x64\"")
            {
                WorkingDirectory = Path
            };
            Process.Start(info); //Start cmd process
        }
    }

    [MenuItem("Build/Game/All")]
    static void GameAll()
    {
        string Path = Application.dataPath.Replace("Assets", "Build") + @"\Game";
        bool Confirmation = EditorUtility.DisplayDialog("Build all?", "Confirm to build on folder version " + Version + "?", "Yes", "No");
        if (!Confirmation)
        {
            UnityEngine.Debug.LogError("Build cancel");
            return;
        }
        List<string> Scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            Scenes.Add(scene.path);
        }
        // 64
        UnityEngine.Debug.Log("Building Windows x64 " + DateTime.Now.ToString());
        BuildPlayerOptions win64 = new BuildPlayerOptions()
        {
            locationPathName = Path + "/BBTUD x64/" + FileName + ".exe",
            scenes = Scenes.ToArray(),
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone
        };
        BuildReport report;
        report = BuildPipeline.BuildPlayer(win64);
        if (report.summary.totalErrors <= 0)
        {
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $"/c tar acvf \"Baldi's Basics TUD Windows V{Version} x64.zip\" \"BBTUD x64\"")
            {
                WorkingDirectory = Path
            };
            Process.Start(info); //Start cmd process
        }
        // 32
        UnityEngine.Debug.Log("Building Windows x32 " + DateTime.Now.ToString());
        BuildPlayerOptions win32 = new BuildPlayerOptions()
        {
            locationPathName = Path + "/BBTUD x32/" + FileName + ".exe",
            scenes = Scenes.ToArray(),
            target = BuildTarget.StandaloneWindows,
            targetGroup = BuildTargetGroup.Standalone
        };
        report = BuildPipeline.BuildPlayer(win32);
        if (report.summary.totalErrors <= 0)
        {
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $"/c tar acvf \"Baldi's Basics TUD Windows V{Version} x32.zip\" \"BBTUD x32\"")
            {
                WorkingDirectory = Path
            };
            Process.Start(info); //Start cmd process
        }
        // Mac
        UnityEngine.Debug.Log("Building Mac OSX " + DateTime.Now.ToString());
        BuildPlayerOptions mac = new BuildPlayerOptions()
        {
            locationPathName = Path + $"/Baldi's Basics TUD MacOS X V{Version}.app",
            scenes = Scenes.ToArray(),
            target = BuildTarget.StandaloneOSX,
            targetGroup = BuildTargetGroup.Standalone
        };
        report = BuildPipeline.BuildPlayer(mac);
        if (report.summary.totalErrors <= 0)
        {
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $"/c tar acvf \"Baldi's Basics TUD Mac OS X V{Version}.zip\" \"Baldi's Basics TUD MacOS X V{Version}.app\"")
            {
                WorkingDirectory = Path
            };
            Process.Start(info); //Start cmd process
        }
        // Linux
        UnityEngine.Debug.Log("Building Mac OSX " + DateTime.Now.ToString());
        BuildPlayerOptions linux = new BuildPlayerOptions()
        {
            locationPathName = Path + $"/Baldi's Basics TUD Linux V{Version}/Baldis Basics TUD.x86_64",
            scenes = Scenes.ToArray(),
            target = BuildTarget.StandaloneLinux64,
            targetGroup = BuildTargetGroup.Standalone
        };
        report = BuildPipeline.BuildPlayer(linux);
        if (report.summary.totalErrors <= 0)
        {
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $"/c tar acvf \"Baldi's Basics TUD Linux V{Version}.zip\" \"Baldi's Basics TUD Linux V{Version}")
            {
                WorkingDirectory = Path
            };
            Process.Start(info); //Start cmd process
        }
        // Android
        UnityEngine.Debug.Log("Building Android " + DateTime.Now.ToString());
        BuildPlayerOptions and = new BuildPlayerOptions()
        {
            locationPathName = Path + $"/Baldi's Basics TUD Android V{Version}.apk",
            scenes = Scenes.ToArray(),
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android
        };
        BuildPipeline.BuildPlayer(and);
        // WebGL
        UnityEngine.Debug.Log("Switching Web Browser " + DateTime.Now.ToString());
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL); // It will fail since things don't get enabled at start.
        UnityEngine.Debug.Log("Building Web Browser " + DateTime.Now.ToString());
        BuildPlayerOptions browser = new BuildPlayerOptions()
        {
            locationPathName = Path + "/HTML/",
            scenes = Scenes.ToArray(),
            target = BuildTarget.WebGL,
            targetGroup = BuildTargetGroup.WebGL
        };
        BuildPipeline.BuildPlayer(browser);
    }

    // [MenuItem("Build/Bundle/Build V3")]
    // static void BuildMapAll()
    // {
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/mac", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/linux", BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/android", BuildAssetBundleOptions.None, BuildTarget.Android);
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/browser", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    // }


    // [MenuItem("Build/Bundle/Build V3 - Windows")]
    // static void BuildMapABs()
    // {
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    // }

    // [MenuItem("Build/Bundle/Build V3 - Android")]
    // static void BuildMapAndroid()
    // {
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/android", BuildAssetBundleOptions.None, BuildTarget.Android);
    // }

    // [MenuItem("Build/Bundle/Build V3 - Other")]
    // static void BuildMapAndroidABs()
    // {
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/mac", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/linux", BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/android", BuildAssetBundleOptions.None, BuildTarget.Android);
    //     BuildPipeline.BuildAssetBundles("Build/Bundles_3/browser", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    // }
}
#endif
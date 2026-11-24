#if UNITY_EDITOR
using UnityEditor;
using Platforms.Console.Logger;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Diagnostics;

namespace Platforms
{
    public class PlatformsUpdater
    {
        public static readonly string version = "1.1";
        public static readonly string installationPath = Application.persistentDataPath + "/Platforms/Installer.unitypackage";

        [MenuItem("Platforms/Check and Install Updates")]
        public static void CheckAndInstallUpdates()
        {
            // Check latest version
            System.Version v = new System.Version(version);
            UnityWebRequest request1 = UnityWebRequest.Get("https://seenwalex.wixsite.com/unityplatforms/_functions/latestVersion");
            request1.timeout = 15;
            bool canceled = EditorUtility.DisplayCancelableProgressBar("Platforms Installer", "Checking for updates", 0f);
            ConsoleLogger.Log("(UPDATER) Checking updates");

            request1.SendWebRequest();
        rb1:
            if (!request1.isDone)
            {
                if (canceled)
                {
                    EditorUtility.ClearProgressBar();
                    request1.Abort();
                    ConsoleLogger.Log("(UPDATER) Canceled");
                    return;
                }
                canceled = EditorUtility.DisplayCancelableProgressBar("Platforms Installer", "Checking for updates", request1.downloadProgress);
                goto rb1;
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayProgressBar("Platforms Installer", "Checking for updates", 1f);

            if (!string.IsNullOrEmpty(request1.error))
            {
                ConsoleLogger.LogError("(UPDATER) Failed: " + request1.error);
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Platforms Installer", request1.error, "OK");
                return;
            }
            System.Version latestVersion;
            try
            {
                latestVersion = new System.Version(request1.downloadHandler.text);
                if (v == latestVersion)
                {
                    ConsoleLogger.Log("(UPDATER) Up to date. No further action is neccessary.");
                    EditorUtility.DisplayDialog("Platforms Installer", "You are up to date!", "OK");

                    EditorUtility.ClearProgressBar();
                    return;
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Platforms Installer", e.Message, "OK");
                return;
            }
            if (v > latestVersion)
            {
                EditorUtility.DisplayDialog("Platforms Installer", "No new standard releases available. If updating a beta build, please update it from the website manually.", "OK");
                ConsoleLogger.Log("(UPDATER) No new standard releases available. No further action is neccessary.");
                EditorUtility.ClearProgressBar();
                return;
            }

            // INSTALL UPDATE
            if (File.Exists(installationPath)) File.Delete(installationPath); // DELETE FILE IF EXISTS TO CREATE A FRESH NEW ONE
            ConsoleLogger.Log("(UPDATER) Installing V" + latestVersion.ToString());
            UnityWebRequest request2 = UnityWebRequest.Get("https://seenwalex.wixsite.com/unityplatforms/_functions/installation");
            EditorUtility.DisplayProgressBar("Platforms Installer", "Getting Installation URL", 0f);
            request2.SendWebRequest();
        rb2:
            if (!request2.isDone)
            {
                EditorUtility.DisplayProgressBar("Platforms Installer", "Getting Installation URL", request2.downloadProgress);
                goto rb2;
            }

            if (!string.IsNullOrEmpty(request2.error))
            {
                ConsoleLogger.LogError("(UPDATER) Failed: " + request2.error);
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Platforms Installer", request2.error, "OK");
                return;
            }

            UnityWebRequest request3 = UnityWebRequest.Get(request2.downloadHandler.text);
            DownloadHandlerFile dhf = new DownloadHandlerFile(installationPath);
            dhf.removeFileOnAbort = true;
            request3.downloadHandler = dhf;
            EditorUtility.DisplayProgressBar("Platforms Installer", "Waiting For Server", 0f);
            request3.SendWebRequest();

            ConsoleLogger.Log("(UPDATER) Downloading Installer");
            canceled = false;
        rb3:
            if (canceled)
            {
                ConsoleLogger.Log("(UPDATER) Canceling download");
                request3.Abort();
                EditorUtility.ClearProgressBar();
                return;
            }
            if (!request3.isDone)
            {
                canceled = EditorUtility.DisplayCancelableProgressBar("Platforms Installer", "Downloading (" + Mathf.RoundToInt(request3.downloadProgress) * 100f + "%) [" + SizeSuffix(((long)request3.downloadedBytes)) + "]", request3.downloadProgress);
                goto rb3;
            }
            EditorUtility.DisplayProgressBar("Platforms Installer", "Downloaded (" + Mathf.RoundToInt(request3.downloadProgress) * 100f + "%) [" + SizeSuffix(((long)request3.downloadedBytes)) + "]", 1f);
            if (!string.IsNullOrEmpty(request3.error))
            {
                ConsoleLogger.LogError("(UPDATER) Failed: " + request3.error);
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Platforms Installer", request3.error, "OK");
                return;
            }

            ConsoleLogger.Log("(UPDATER) Opening Installer");
            // File.WriteAllBytes(installationPath, request3.downloadHandler.data);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = Application.persistentDataPath + "/Platforms/";
            processStartInfo.FileName = "Installer.unitypackage";
            Process.Start(processStartInfo);
            EditorUtility.ClearProgressBar();
        }

        public static void CheckForUpdates()
        {
            // Check latest version
            System.Version v = new System.Version(version);
            UnityWebRequest request1 = UnityWebRequest.Get("https://seenwalex.wixsite.com/unityplatforms/_functions/latestVersion");
            bool canceled = EditorUtility.DisplayCancelableProgressBar("Platforms", "Checking for updates", 0f);

            request1.SendWebRequest();
        rb1:
            if (!request1.isDone)
            {
                if (canceled)
                {
                    EditorUtility.ClearProgressBar();
                    request1.Abort();
                    ConsoleLogger.Log("(UPDATER) Canceled");
                    return;
                }
                canceled = EditorUtility.DisplayCancelableProgressBar("Platforms", "Checking for updates", request1.downloadProgress);
                goto rb1;
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayProgressBar("Platforms Installer", "Checking for updates", 1f);

            if (!string.IsNullOrEmpty(request1.error))
            {
                ConsoleLogger.LogError("(UPDATER) Failed: " + request1.error);
                EditorUtility.ClearProgressBar();
                return;
            }
            System.Version latestVersion;
            try
            {
                latestVersion = new System.Version(request1.downloadHandler.text);
                if (v == latestVersion)
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("UNEXPECTED PLATFORMS ERROR", e.Message, "OK");
                return;
            }
            if (v > latestVersion)
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            // INSTALL UPDATE
            ConsoleLogger.Log("(UPDATER) A NEW UPDATE IS AVAILABLE! Go to Platforms > Check and Install Updates");
            EditorUtility.ClearProgressBar();
        }

        // CREDIT: https://stackoverflow.com/a/14488941
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        private static string SizeSuffix(Int64 value, int decimalPlaces = 0)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} B", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
#endif
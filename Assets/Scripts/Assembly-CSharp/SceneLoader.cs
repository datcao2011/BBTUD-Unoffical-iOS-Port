using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Platforms;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Slider slider;

    public TMP_Text textProgress;

    public bool LoadResult;

    public bool isLoadingStarterScenes;

    public bool dontLoadWhenNotReady;

    public GameObject loadButton;

    public static AssetBundle LoadedBundle;

    public TMP_Text status;

    public GameObject loadText;

    public void ReadyToLoad()
    {
        LoadResult = true;
    }

    public void LoadScene(string scene)
    {
        LoadScene(false, scene);
    }

    public void AddScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }

    public void LoadSceneAsync(string scene)
    {
        LoadScene(true, scene);
    }

    private void LoadScene(bool async, string scene)
    {
        if (async)
        {
            if (isLoadingStarterScenes)
            {
                if (!dontLoadWhenNotReady)
                {
                    StartCoroutine(AsyncLoadOperation(null, false, scene));
                }
                else
                {
                    StartCoroutine(AsyncLoadOperation(null, true, scene));
                }
                return;
            }
            if (!dontLoadWhenNotReady)
            {
                StartCoroutine(AsyncLoadOperation(SceneManager.LoadSceneAsync(scene), false, scene));
            }
            else
            {
                StartCoroutine(AsyncLoadOperation(SceneManager.LoadSceneAsync(scene), true, scene));
            }
            return;
        }
        SceneManager.LoadScene(scene);
    }

    private float progress;

    private IEnumerator AsyncLoadOperation(AsyncOperation sceneLoaderStatus, bool dontLoadUntilReady, string scene)
    {
        if (isLoadingStarterScenes)
        {
            // I removed the auto-updating here.
// #if !UNITY_EDITOR
//             if (sceneLoaderStatus == null && scene == "School_2")
//             {
//                 yield return StartCoroutine(CheckForNewSchoolhouse());
//             }
//             if (scene == "School_2" && LoadedBundle == null)
//             {
//                 InputManager.DisplayEvent("The Grand Opening Schoolhouse may be outdated, as we are unable to update it at this time.", 5f);
//             }
// #endif
            if (sceneLoaderStatus == null)
            {
                status.text = "Waiting for management";
                if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(0.35f);
            }
            status.text = "Loading level assets";
            if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(0.1f);
            sceneLoaderStatus = SceneManager.LoadSceneAsync(scene);
            sceneLoaderStatus.allowSceneActivation = Settings.GetBool("SkipUpdatesAndGaps"); // Instant load

            while (progress < 1f)
            {
                progress = Mathf.Clamp01(sceneLoaderStatus.progress / .9f);
                slider.value = progress;
                float progressText = progress * 100f;
                textProgress.text = Mathf.CeilToInt(progressText) + "%";
                yield return null;
            }

            if (dontLoadUntilReady)
            {
                slider.value = slider.maxValue;
                status.text = "Waiting for confirmation...";
                loadButton.SetActive(true);
                while (!LoadResult)
                {
                    yield return null;
                }
                status.fontStyle = FontStyles.Normal;
                status.text = GetRandomText();
                loadButton.SetActive(false);
                if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(0.15f);
                sceneLoaderStatus.allowSceneActivation = true;
            }
            else
            {
                status.fontStyle = FontStyles.Normal;
                status.text = GetRandomText();
                if (!Settings.GetBool("SkipUpdatesAndGaps")) yield return new WaitForSecondsRealtime(0.15f);
                sceneLoaderStatus.allowSceneActivation = true;
            }
        }
        else
        {
            while (!sceneLoaderStatus.isDone && !dontLoadUntilReady || sceneLoaderStatus.progress >= 0.9f && dontLoadUntilReady)
            {
                progress = Mathf.Clamp01(sceneLoaderStatus.progress / .9f);
                slider.value = progress;
                float progressText = progress * 100f;
                textProgress.text = Mathf.FloorToInt(progressText) + "%";
                yield return null;
            }

            if (dontLoadUntilReady)
            {
                textProgress.text = "Ready!";
                slider.value = slider.maxValue;
                loadText.SetActive(false);
                loadButton.SetActive(true);
                while (!LoadResult)
                {
                    yield return null;
                }
                textProgress.text = "LOADING";
                loadText.SetActive(true);
                loadButton.SetActive(false);
                sceneLoaderStatus.allowSceneActivation = true;
            }
        }
        yield break;
    }

    private class ContentResponse
    {
        public string url;
        public string assets;
        public string error;
        public bool success;
        public string crc;
    }

    private static string BytesToString(long byteCount)
    {
        string[] suf = { " B", " KB", " MB", " GB", " TB", " PB", " EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = System.Math.Abs(byteCount);
        int place = System.Convert.ToInt32(System.Math.Floor(System.Math.Log(bytes, 1024)));
        double num = System.Math.Round(bytes / System.Math.Pow(1024, place), 1);
        return (System.Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    // private IEnumerator CheckForNewSchoolhouse()
    // {
    //     string Path = System.IO.Path.Combine(Application.persistentDataPath, "v30_cache");
    //     string CrcPath = System.IO.Path.Combine(Application.persistentDataPath, "crc");
    //     string AssetPath = System.IO.Path.Combine(Application.persistentDataPath, "v30_textures");
    //     if (Application.internetReachability == NetworkReachability.NotReachable)
    //     {
    //         if (!System.IO.File.Exists(CrcPath)) yield break;
    //         uint crc2 = uint.Parse(System.IO.File.ReadAllText(CrcPath));
    //         AssetBundleCreateRequest assetReq2 = AssetBundle.LoadFromFileAsync(Path, crc2);
    //         status.text = "Loading content (1/2)";
    //         // Progress check
    //         while (!assetReq2.isDone)
    //         {
    //             slider.value = Mathf.Clamp01(assetReq2.progress / .9f);
    //             textProgress.text = (Mathf.Clamp01(assetReq2.progress / .9f) * 100f).ToString("0") + "%";
    //             yield return null;
    //         }
    //         LoadedBundle = assetReq2.assetBundle;
    //         // Second content
    //         AssetBundleCreateRequest assetsReq3;
    //         assetsReq3 = AssetBundle.LoadFromFileAsync(AssetPath, 0U);
    //         status.text = "Loading content (2/2)";
    //         // Progress check
    //         while (!assetsReq3.isDone)
    //         {
    //             slider.value = Mathf.Clamp01(assetsReq3.progress / .9f);
    //             textProgress.text = (Mathf.Clamp01(assetsReq3.progress / .9f) * 100f).ToString("0") + "%";
    //             yield return null;
    //         }
    //         // Done, reset values
    //         slider.value = 0f;
    //         textProgress.text = "0%";
    //         if (LoadedBundle != null)
    //         {
    //             InputManager.DisplayEvent("The Grand Opening Schoolhouse may be outdated, as we are unable to update it at this time.", 5f);
    //         }
    //         yield break;
    //     }
    //     if (LoadedBundle) yield break;
    //     bool CanContinue = true;
    //     status.text = "Checking schoolhouse updates";
    //     UnityWebRequest ContentRetrieveReq = UnityWebRequest.Get(InputManager.siteRequestUrl + "RetrieveTemporaryContent?v=" + Application.version + "&d=" + CurrentDevice.CheckDevice(false).ToString());
    //     yield return ContentRetrieveReq.SendWebRequest();
    //     ContentResponse response;
    //     try
    //     {
    //         if (ContentRetrieveReq.result != UnityWebRequest.Result.Success)
    //         {
    //             InputManager.DisplayException(new System.Exception("Web server connection failed"), 5f);
    //             yield break;
    //         }
    //         response = JsonUtility.FromJson<ContentResponse>(ContentRetrieveReq.downloadHandler.text);
    //         if (!response.success)
    //         {
    //             InputManager.DisplayException(new System.Exception(response.error ?? "Fail"), 5f);
    //             yield break;
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         InputManager.DisplayException(ex);
    //         yield break;
    //     }
    //     AssetBundleCreateRequest assetReq;
    //     if (!System.IO.File.Exists(Path) || (System.IO.File.Exists(CrcPath) && System.IO.File.ReadAllText(CrcPath) != response.crc && CanContinue))
    //     {
    //         status.text = "Preparing to update";
    //         UnityWebRequest SizeRetrieveReq = UnityWebRequest.Head(response.url);
    //         yield return SizeRetrieveReq.SendWebRequest();
    //         long SizeLength = 0L;
    //         try
    //         {
    //             if (SizeRetrieveReq.GetResponseHeaders().ContainsKey("Content-Length"))
    //             {
    //                 SizeLength = long.Parse(SizeRetrieveReq.GetResponseHeader("Content-Length"));
    //             }
    //         }
    //         catch (System.Exception e)
    //         {
    //             InputManager.DisplayException(e, 15f);
    //             yield break;
    //         }
    //         status.text = "Downloading update";
    //         UnityWebRequest DownloadReq = UnityWebRequest.Get(response.url);
    //         DownloadReq.downloadHandler = new DownloadHandlerFile(Path);
    //         DownloadReq.SendWebRequest();
    //         while (!DownloadReq.isDone)
    //         {
    //             if (SizeLength == 0L)
    //             {
    //                 textProgress.text = "(" + BytesToString((long)DownloadReq.downloadedBytes) + ")";
    //                 if (DownloadReq.GetResponseHeaders() != null && DownloadReq.GetResponseHeaders().ContainsKey("Content-Length"))
    //                 {
    //                     SizeLength = long.Parse(DownloadReq.GetResponseHeader("Content-Length"));
    //                 }
    //             }
    //             else
    //             {
    //                 status.text = "Downloading update (" + BytesToString(SizeLength) + ")";
    //                 slider.value = DownloadReq.downloadProgress;
    //                 textProgress.text = (DownloadReq.downloadProgress * 100f).ToString("0") + "% (" + BytesToString((long)DownloadReq.downloadedBytes) + ")";
    //             }
    //             yield return null;
    //         }
    //         slider.value = DownloadReq.downloadProgress;
    //         textProgress.text = (DownloadReq.downloadProgress * 100f).ToString("0") + "% (" + BytesToString((long)DownloadReq.downloadedBytes) + ")";
    //         yield return new WaitForSecondsRealtime(0.1f);
    //         status.text = "Downloading new content";
    //         UnityWebRequest AssetDownloadReq = UnityWebRequest.Get(response.assets);
    //         AssetDownloadReq.downloadHandler = new DownloadHandlerFile(AssetPath);
    //         AssetDownloadReq.SendWebRequest();
    //         SizeLength = 0L;
    //         while (!AssetDownloadReq.isDone)
    //         {
    //             slider.value = AssetDownloadReq.downloadProgress;
    //             textProgress.text = (AssetDownloadReq.downloadProgress * 100f).ToString("0") + "%";
    //             yield return null;
    //         }
    //         slider.value = 0f;
    //         textProgress.text = "0% (1)";

    //         assetReq = AssetBundle.LoadFromFileAsync(Path, uint.Parse(response.crc));
    //         status.text = "Loading new content (1/2)";
    //         // Progress check
    //         while (!assetReq.isDone)
    //         {
    //             slider.value = Mathf.Clamp01(assetReq.progress / .9f) / 2;
    //             textProgress.text = (Mathf.Clamp01(assetReq.progress / .9f) / 2 * 100f).ToString("0") + "% (1)";
    //             yield return null;
    //         }
    //         LoadedBundle = assetReq.assetBundle;
    //         // Second content
    //         AssetBundleCreateRequest _assetsReq;
    //         _assetsReq = AssetBundle.LoadFromFileAsync(AssetPath, 0U);
    //         status.text = "Loading new content (2/2)";
    //         // Progress check
    //         while (!_assetsReq.isDone)
    //         {
    //             slider.value = 0.5f + (Mathf.Clamp01(_assetsReq.progress / .9f) / 2);
    //             textProgress.text = ((0.5f + (Mathf.Clamp01(_assetsReq.progress / .9f) / 2)) * 100f).ToString("0") + "% (2)";
    //             yield return null;
    //         }
    //         try
    //         {
    //             System.IO.File.WriteAllText(CrcPath, response.crc.ToString());
    //         }
    //         catch (System.Exception)
    //         {
    //         }
    //         // Done, reset values
    //         slider.value = 0f;
    //         textProgress.text = "0%";
    //         yield break;
    //     }
    //     uint crc = uint.Parse(System.IO.File.Exists(CrcPath) ? System.IO.File.ReadAllText(CrcPath) : "1");
    //     assetReq = AssetBundle.LoadFromFileAsync(Path, crc);
    //     status.text = "Loading new content (1/2)";
    //     // Progress check
    //     while (!assetReq.isDone)
    //     {
    //         slider.value = Mathf.Clamp01(assetReq.progress / .9f);
    //         textProgress.text = (Mathf.Clamp01(assetReq.progress / .9f) * 100f).ToString("0") + "%";
    //         yield return null;
    //     }
    //     LoadedBundle = assetReq.assetBundle;
    //     // Second content
    //     AssetBundleCreateRequest assetsReq;
    //     assetsReq = AssetBundle.LoadFromFileAsync(AssetPath, 0U);
    //     status.text = "Loading new content (2/2)";
    //     // Progress check
    //     while (!assetsReq.isDone)
    //     {
    //         slider.value = Mathf.Clamp01(assetsReq.progress / .9f);
    //         textProgress.text = (Mathf.Clamp01(assetsReq.progress / .9f) * 100f).ToString("0") + "%";
    //         yield return null;
    //     }
    //     // Done, reset values
    //     slider.value = 0f;
    //     textProgress.text = "0%";
    //     yield break;
    // }

    public static string GetRandomText()
    {
        string[] LoadedTextPhrases = new string[] {
            "<color=blue>HERE WE <b><color=#FF00CD>GO!</color></b></color>",
            "<color=purple>LET'S <b><color=#FF00CD>GO!</color></b></color>",
            "<color=#FF00CD>GET <b><color=red>READY</color></b></color>",
            "<color=green><b>GO!</b></color>",
            "<color=yellow><b>HELLO</b></color>",
            "<color=green>IT'S</color> <color=blue><b>DONE!</b></color>",
            "<color=red>READY FOR <b><color=#FF00CD>FUN?</color></b></color>",
            "<color=green>GOOD <color=yellow>LUCK!</color></color>"
        };
        return LoadedTextPhrases[Mathf.RoundToInt(Random.Range(0f, LoadedTextPhrases.Length - 1))];
    }
}
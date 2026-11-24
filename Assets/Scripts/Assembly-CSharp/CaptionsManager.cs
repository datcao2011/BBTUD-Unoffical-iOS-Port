using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CaptionsManager : MonoBehaviour
{
    public GameObject Template2D;
    public GameObject Template3D;
    public CaptionItem CaptionItems;

    public Transform Captions2DManager;

    private List<string> AudioSourceAndClips = new List<string>();

    private List<string> AudioOneShots = new List<string>();

    public bool ActivateIntensiveAudioSourceChecking = true;
    private AudioSource[] AudioDevices;

    private Dictionary<string, string> DoorCaptions = new Dictionary<string, string>();
    private bool Initialized;

    private string GetPositionName(Vector3 Pos)
    {
        return Pos.x.ToString("0") + Pos.y.ToString("0") + Pos.z.ToString("0");
    }

    private void Start()
    {
        Initialized = true;
#if UNITY_STANDALONE && !UNITY_EDITOR
        string path = Application.dataPath;
#else
        string path = Application.persistentDataPath;
#endif
#if !UNITY_WEBGL || UNITY_EDITOR
        try
        {
            if (File.Exists(Path.Combine(path, "Captions.ini")))
            {
                IniStorage file = new IniStorage(Path.Combine(path, "Captions.ini"));
                if (file.ContainsGroup("Settings"))
                {
                    IniStorage.Group group = file.GetGroup("Settings");
                    if (group.Get("File").Get() != "1")
                    {
                        AddDoorCaptions();
                        return;
                    }
                    if (group.Get("Version") == null || group.Get("Version").Get() != "2.1")
                    {
                        GenerateCaptions(Path.Combine(path, "Captions.ini"));
                        AddDoorCaptions();
                        return;
                    }
                }
                if (!file.ContainsGroup("Captions"))
                {
                    throw new ArgumentException("Unable to find the 'Captions' group");
                }
                IniStorage.Pair[] pairs = file.GetGroup("Captions").ToArray();
                CaptionItems.Clear(); // Clears captions items
                foreach (IniStorage.Pair pair in pairs)
                {
                    CaptionItems.Add(pair.Name, pair.Get()); // Add the caption item
                };
            }
            else if (path != Application.dataPath || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
            {
                GenerateCaptions(Path.Combine(path, "Captions.ini"));
            }
        }
        catch (Exception ex)
        {
            InputManager.DisplayException(ex, 15f, "Captions had trouble loading, default will be used instead!");
            Debug.LogException(ex);
        }
#endif
        AddDoorCaptions();
    }

    private void AddDoorCaptions()
    {
        if (Settings.GetBool("EnableDoorCaptions", true))
        {
            foreach (DoorScript Door in FindObjectsOfType<DoorScript>(true))
            {
                Door.name = "_DoorOut_" + GetPositionName(Door.transform.position);
            }
            foreach (SwingingDoorScript Door in FindObjectsOfType<SwingingDoorScript>(true))
            {
                Door.name = "_DoorOut_" + GetPositionName(Door.transform.position);
            }
        }
        else
        {
            foreach (KeyValuePair<string, string> pair in CaptionItems.ToList())
            {
                string C = pair.Key;
                if (C.StartsWith("door") || C == "swingdoor_open" || C.StartsWith("Doors"))
                {
                    CaptionItems.Remove(C);
                    DoorCaptions.Add(pair.Key, pair.Value);
                }
            }
        }
    }

    private void OnEnable()
    {
        Captions2DManager.gameObject.SetActive(true);
        Settings.OnSettingChanged += OnDoorCaptionSetting;
        if (Initialized)
        {
            OnDoorCaptionSetting("EnableDoorCaptions", Settings.GetBool("EnableDoorCaptions", true));
        }
    }

    private void OnDoorCaptionSetting(string key, object value)
    {
        if (!Initialized) return;
        if (key == "EnableDoorCaptions")
        {
            bool Value = (bool)value;
            if (Value)
            {
                foreach (DoorScript Door in FindObjectsOfType<DoorScript>(true))
                {
                    Door.name = "_DoorOut_" + GetPositionName(Door.transform.position);
                }
                foreach (SwingingDoorScript Door in FindObjectsOfType<SwingingDoorScript>(true))
                {
                    Door.name = "_DoorOut_" + GetPositionName(Door.transform.position);
                }
                foreach (KeyValuePair<string, string> pair in DoorCaptions)
                {
                    CaptionItems.Add(pair.Key, pair.Value);
                }
                DoorCaptions.Clear();
            }
            else
            {
                foreach (DoorScript Door in FindObjectsOfType<DoorScript>(true))
                {
                    Door.name = "_DoorOut";
                }
                foreach (SwingingDoorScript Door in FindObjectsOfType<SwingingDoorScript>(true))
                {
                    Door.name = "_DoorOut";
                }
                foreach (KeyValuePair<string, string> pair in CaptionItems.ToList())
                {
                    string C = pair.Key;
                    if (C.StartsWith("door") || C == "swingdoor_open" || C.StartsWith("Doors"))
                    {
                        CaptionItems.Remove(C);
                        DoorCaptions.Add(pair.Key, pair.Value);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        if (Captions2DManager != null) // Making sure the GameObject is not getting destroyed.
        {
            Captions2DManager.gameObject.SetActive(false);
            DestroyAllCaptions(false, true); // Destroy all the captions.
        }
        Settings.OnSettingChanged -= OnDoorCaptionSetting;
    }

#if !UNITY_WEBGL || UNITY_EDITOR
    private void GenerateCaptions(string path)
    {
        if (CaptionItems == null || CaptionItems.Count <= 0)
        {
            // We're not going to generate Captions becuase there's none to generate.
            return;
        }
        string Comments = "# !!! Generated by Game !!!\n# This file will be re-generated if deleted.\n# Keep in mind, removing a caption here will remove the caption from the game too.\n\n";
        string Settings = "[Settings]\nFile=1\nVersion=2.1\n# 0 = Disabled | 1 = Enabled\n\n[Captions]\n";
        string Result = Comments + Settings;
        for (int i = 0; i < CaptionItems.Count; i++)
        {
            string C = CaptionItems.Keys.ElementAt(i);
            Result += C + "=" + CaptionItems[C] + "\n";
        }
        Result += "# End of caption generation";
        File.WriteAllText(path, Result);
    }
#endif

    private void FixedUpdate() // So it doesn't lag EVERY frame
    {
        if (ActivateIntensiveAudioSourceChecking)
        {
            AudioDevices = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioDevice in AudioDevices)
            {
                if (audioDevice.clip != null)
                {
                    if (AudioSourceAndClips.Contains(audioDevice.name + ":" + audioDevice.clip.name))
                    {
                        if (!audioDevice.isPlaying)
                        {
                            DeleteItem(audioDevice.name + ":" + audioDevice.clip.name, audioDevice.spatialBlend < 0.5f);
                            AudioSourceAndClips.Remove(audioDevice.name + ":" + audioDevice.clip.name);
                        }
                        else
                        {
                            UpdatePosition(audioDevice.name + ":" + audioDevice.clip.name, audioDevice);
                        }
                    }
                    else if (audioDevice.isPlaying)
                    {
                        string Translation;
                        if (CaptionItems.TryGetValue(audioDevice.clip.name, out Translation))
                        {
                            CreateCaptionItem(audioDevice, audioDevice.clip, false);
                        }
                    }
                }
            }

            foreach (string AudioOneShot in AudioOneShots.ToList())
            {
                try
                {
                    GameObject GB = GameObject.Find(AudioOneShot.Split(':')[0]);
                    if (GB == null) UpdatePosition(AudioOneShot, null, true);
                    else UpdatePosition(AudioOneShot, GB.GetComponent<AudioSource>(), true);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }

    public void DeleteItem(string Name)
    {
        foreach (Transform Child in transform)
        {
            if (Child != null && Child.name == Name)
            {
                Destroy(Child.gameObject, 0f);
            }
        }
    }

    public void DeleteItem(string Name, bool Is2D)
    {
        if (!Is2D) DeleteItem(Name);
        foreach (Transform Child in Captions2DManager)
        {
            if (Child != null && Child.name == Name)
            {
                Destroy(Child.gameObject, 0f);
            }
        }
    }

    public void DeleteClipCaption(GameObject gb, string Name)
    {
        if (AudioSourceAndClips.Contains(gb.name + ":" + Name))
        {
            DeleteItem(gb.name + ":" + Name);
            AudioSourceAndClips.Remove(gb.name + ":" + Name);
        }
    }

    public void DeleteOneShotCaption(GameObject gb, string Name)
    {
        if (AudioOneShots.Contains(gb.name + ":" + Name))
        {
            DeleteItem(gb.name + ":" + Name);
            AudioOneShots.Remove(gb.name + ":" + Name);
            if (DestroyList.ContainsKey(gb.name + ":" + Name))
            {
                DestroyList.Remove(gb.name + ":" + Name);
            }
        }
    }

    public void RemoveFromList(string Name)
    {
        AudioSourceAndClips.Remove(Name);
    }

    public void UpdatePosition(string Name, AudioSource Source, bool IsOneShot = false)
    {
        if (Source == null && IsOneShot)
        {
            AudioOneShots.Remove(Name);
            return;
        }
        Transform[] Children;
        if (Source.spatialBlend < 0.5f)
        {
            Children = new Transform[Captions2DManager.childCount];
            for (int i = 0; i < Captions2DManager.childCount; i++)
            {
                Children[i] = Captions2DManager.GetChild(i);
            }
        }
        else
        {
            Children = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                Children[i] = transform.GetChild(i);
            }
        }

        bool FoundChild = false;
        foreach (Transform Child in Children)
        {
            if (Child != null && Child.name == Name)
            {
                FoundChild = true;
                TransformPosition(Child.GetComponent<RectTransform>(), Source);
            }
        }
        if (!FoundChild) // It means it was automatically deleted, let's just remove it from the list
        {
            if (IsOneShot)
                AudioOneShots.Remove(Name);
            else
                AudioSourceAndClips.Remove(Name);
        }
    }

    public void TransformPosition(RectTransform DeviceBG, AudioSource AudioDevice)
    {
        if (AudioDevice.spatialBlend < 0.5f) return;
        Transform CamTransform = Camera.main.transform;
        Transform AudTransform = AudioDevice.transform;
        float Distance = Mathf.Clamp(Vector3.Distance(AudTransform.position, CamTransform.position), AudioDevice.minDistance, AudioDevice.maxDistance);
        float CalculatedOne = (Distance - AudioDevice.minDistance) / (AudioDevice.maxDistance - AudioDevice.minDistance);
        DeviceBG.GetComponent<CanvasGroup>().alpha = (1 - CalculatedOne) / 0.9f;
        float radius = 700f / 4.5f;
        float num = Mathf.Atan2(CamTransform.position.z - AudTransform.position.z, CamTransform.position.x - AudTransform.position.x) * 57.29578f + CamTransform.eulerAngles.y + 180f;
        DeviceBG.anchoredPosition = new Vector3(Mathf.Cos(num * 0.017453292f) * radius, Mathf.Sin(num * 0.017453292f) * radius, 0f);
        float num2 = Mathf.Max(1f - CalculatedOne, 0f);
        DeviceBG.localScale = new Vector3(num2, num2, 1f);
    }

    public void CreateCustomCaption(AudioSource PlayedSource, string Key, float Duration = 5f, float Scheduled = 0f)
    {
        StartCoroutine(ScheduleCaption(PlayedSource, Key, Duration, Scheduled));
    }

    private IEnumerator ScheduleCaption(AudioSource PlayedSource, string Key, float Duration, float Schedule)
    {
        yield return new WaitForSecondsRealtime(Schedule / PlayedSource.pitch);
        string Translation;
        if (!CaptionItems.TryGetValue(Key, out Translation))
        {
            // Canceling the schedule since there's no caption!
            yield break;
        }
        if (PlayedSource.spatialBlend >= 0.5f)
        {
            GameObject GB = Instantiate(Template3D, transform);
            GB.SetActive(true);
            if (CaptionItems.TryGetValue(Key, out Translation))
            {
                GB.GetComponentInChildren<TMP_Text>().text = System.Text.RegularExpressions.Regex.Unescape(Translation);
            }
            GB.name = PlayedSource.name + ":" + Key;
            TransformPosition(GB.GetComponent<RectTransform>(), PlayedSource);
            DestroyList[GB.name] = StartCoroutine(DestroyGB(GB, Duration / PlayedSource.pitch));
            AudioOneShots.Add(GB.name);
        }
        else
        {
            GameObject GB = Instantiate(Template2D, Captions2DManager);
            GB.SetActive(true);
            if (CaptionItems.TryGetValue(Key, out Translation))
            {
                GB.GetComponentInChildren<TMP_Text>().text = System.Text.RegularExpressions.Regex.Unescape(Translation);
            }
            GB.name = PlayedSource.name + ":" + Key;
            DestroyList[GB.name] = StartCoroutine(DestroyGB(GB, Duration / PlayedSource.pitch));
        }
        yield break;
    }

    public void CreateCaptionItem(AudioSource PlayedSource, AudioClip PlayedClip, bool IsOneShot)
    {
        if (PlayedSource.spatialBlend >= 0.5f)
        {
            GameObject GB = Instantiate(Template3D, transform);
            GB.SetActive(true);
            string Translation;
            if (CaptionItems.TryGetValue(PlayedClip.name, out Translation))
            {
                GB.GetComponentInChildren<TMP_Text>().text = System.Text.RegularExpressions.Regex.Unescape(Translation);
            }
            GB.name = PlayedSource.name + ":" + PlayedClip.name;
            TransformPosition(GB.GetComponent<RectTransform>(), PlayedSource);
            if (IsOneShot)
            {
                DestroyList.Add(GB.name, StartCoroutine(DestroyGB(GB, PlayedClip.length / Math.Max(0.1f, PlayedSource.pitch))));
                AudioOneShots.Add(GB.name);
            }
            else
            {
                AudioSourceAndClips.Add(GB.name);
            }
        }
        else
        {
            Animate2DCaption(PlayedSource, PlayedClip, IsOneShot);
        }
    }

    public Dictionary<string, Coroutine> DestroyList = new Dictionary<string, Coroutine>();

    public IEnumerator DestroyGB(GameObject GB, float Length)
    {
        yield return new WaitForSecondsRealtime(Length);
        string Name = "";
        if (GB != null) Name = GB.name;
        if (GB != null) Destroy(GB, 0f);
        if (Name != "" && DestroyList.Keys.Contains(Name)) DestroyList.Remove(Name);
        yield break;
    }

    public void DestroyScheduledCaptions()
    {
        StopAllCoroutines();
        for (int i = 0; i < DestroyList.Count; i++)
        {
            GameObject GB = GameObject.Find(DestroyList.Keys.ElementAt(i));
            if (GB != null)
            {
                Destroy(GB, 0f);
            }
        }
        DestroyList.Clear();
    }

    public void DestroyAllCaptions(bool DisableNewCaptions) => DestroyAllCaptions(DisableNewCaptions, false);


    private void DestroyAllCaptions(bool DisableNewCaptions, bool IgnoreActiveCheck)
    {
        if (!IgnoreActiveCheck && !gameObject.activeSelf) return;
        if (ActivateIntensiveAudioSourceChecking) ActivateIntensiveAudioSourceChecking = !DisableNewCaptions;
        AudioSourceAndClips.Clear();
        AudioOneShots.Clear();
        foreach (Transform child in transform)
        {
            if (child.gameObject.name != Template3D.name)
            {
                Destroy(child.gameObject, 0f);
            }
        }
        foreach (Transform child in Captions2DManager)
        {
            if (child.gameObject.name != Template2D.name)
            {
                Destroy(child.gameObject, 0f);
            }
        }
        if (IgnoreActiveCheck)
        {
            StopAllCoroutines();
            DestroyList.Clear();
        }
        else
        {
            DestroyScheduledCaptions();
        }
    }

    public void DestroyAllCaptions2D()
    {
        if (!gameObject.activeSelf) return;
        foreach (Transform child in Captions2DManager)
        {
            if (AudioSourceAndClips.Contains(child.gameObject.name))
            {
                AudioSourceAndClips.Remove(child.gameObject.name);
            }
            if (AudioOneShots.Contains(child.gameObject.name))
            {
                AudioOneShots.Remove(child.gameObject.name);
            }
            if (child.gameObject.name != Template2D.name)
            {
                if (DestroyList.ContainsKey(child.gameObject.name))
                {
                    DestroyList.Remove(child.gameObject.name);
                }
                Destroy(child.gameObject, 0f);
            }
        }
    }

    public void Animate2DCaption(AudioSource PlayedSource, AudioClip PlayedClip, bool IsOneShot)
    {
        GameObject GB = Instantiate(Template2D, Captions2DManager);
        GB.SetActive(true);
        string Translation;
        if (CaptionItems.TryGetValue(PlayedClip.name, out Translation))
        {
            GB.GetComponentInChildren<TMP_Text>().text = System.Text.RegularExpressions.Regex.Unescape(Translation);
        }
        GB.name = PlayedSource.name + ":" + PlayedClip.name;
        if (IsOneShot)
        {
            DestroyList.Add(GB.name, StartCoroutine(DestroyGB(GB, PlayedClip.length)));
            AudioOneShots.Add(GB.name);
        }
        else
        {
            AudioSourceAndClips.Add(GB.name);
        }
    }
}

public static class CaptionExtensions
{
    private static CaptionsManager CM = null;
    public static void PlayOneShotWC(this AudioSource source, AudioClip clip)
    {
        if (!CM) CM = GameObject.FindObjectOfType<CaptionsManager>();
        if (CM && CM.isActiveAndEnabled && CM.ActivateIntensiveAudioSourceChecking && source != null && clip != null)
        {
            string Translation;
            if (CM.CaptionItems.TryGetValue(clip.name, out Translation))
            {
                if (CM.DestroyList.Keys.Contains(source.name + ":" + clip.name))
                {
                    CM.StopCoroutine(CM.DestroyList[source.name + ":" + clip.name]);
                    CM.DestroyList.Remove(source.name + ":" + clip.name);
                    if (GameObject.Find(source.name + ":" + clip.name))
                    {
                        CM.DestroyList[source.name + ":" + clip.name] = CM.StartCoroutine(CM.DestroyGB(GameObject.Find(source.name + ":" + clip.name), clip.length / Math.Max(0.1f, source.pitch)));
                    }
                }
                else { CM.CreateCaptionItem(source, clip, true); }
            }
        }
        if (source != null) source.PlayOneShot(clip);
    }

    public static void PlayOneShotWC(this AudioSource source, AudioClip clip, float Volume)
    {
        if (!CM) CM = GameObject.FindObjectOfType<CaptionsManager>();
        if (CM && CM.isActiveAndEnabled && CM.ActivateIntensiveAudioSourceChecking && source != null && clip != null)
        {
            string Translation;
            if (CM.CaptionItems.TryGetValue(clip.name, out Translation))
            {
                if (CM.DestroyList.Keys.Contains(source.name + ":" + clip.name))
                {
                    CM.StopCoroutine(CM.DestroyList[source.name + ":" + clip.name]);
                    CM.DestroyList.Remove(source.name + ":" + clip.name);
                    if (GameObject.Find(source.name + ":" + clip.name))
                    {
                        CM.DestroyList[source.name + ":" + clip.name] = CM.StartCoroutine(CM.DestroyGB(GameObject.Find(source.name + ":" + clip.name), clip.length / Math.Max(0.1f, source.pitch)));
                    }
                }
                else { CM.CreateCaptionItem(source, clip, true); }
            }
        }
        if (source != null) source.PlayOneShot(clip, Volume);
    }
}

[System.Serializable]
public class CaptionItem : SerializableDictionary<string, string> { }
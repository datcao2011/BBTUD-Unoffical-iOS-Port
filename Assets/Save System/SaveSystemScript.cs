using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using SchoolSaves;
using System.Collections.Generic;

public class SaveSystemScript : MonoBehaviour
{
    private bool IsSchoolScene;

    private void Start()
    {
        SaveSystemStopper.isOn = true;
        if (Platforms.CurrentDevice.currentPlatform == Platforms.PFPlatform.WebGL)
        {
            GameObject.Find("Save List").SetActive(false);
            GameObject.Find("RefreshButton").SetActive(false);
            DownloadName.SetActive(true);
            GameObject.Find("OpenFolderLocation").SetActive(false);
            return;
        }
        templateDescription = saveDetails.text;
        RefreshSaves();
        if (SceneManager.GetActiveScene().name == InputManager.IsSchoolScene())
        {
            // loadText.text = "Enter the name of the file to load.\nWarning: Any changes on a unsaved game will be lost on loading.";
            //loadText.text = "Load save file?\nOnce the save file is loaded, the school scene will reload.\n<#FF0000>WARNING: Any unsaved progress will be lost if you choose to load the save file.";
            loadText.text = "Resume this save's progress?\n<#FF0000>Remember that any progress made right now will be lost!";
            saveAsButton.gameObject.SetActive(true);
            saveButton.gameObject.SetActive(true);
            IsSchoolScene = true;
        }
#if UNITY_EDITOR
        else if (SceneManager.GetActiveScene().name == "SaveSystem")
        {
            GameObject camera = new GameObject("Main Camera");
            camera.AddComponent<Camera>();
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            eventSystem.AddComponent<BaseInput>();
        }
#endif
#if !UNITY_STANDALONE_WIN && !UNITY_STANDALONE_OSX
        GameObject.Find("OpenFolderLocation").SetActive(false);
#endif
    }

    private void OnDestroy()
    {
        SaveSystemStopper.isOn = false;
    }

    public void OpenFolder()
    {
        OpenInFileBrowser(Application.persistentDataPath + "/Saves");
    }

    public static void OpenInFileBrowser(string path)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(path);
#elif UNITY_STANDALONE_WIN
        bool openInsidesOfFolder = false;

        // try windows
        string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

        if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }
        try
        {
            System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            // tried to open win explorer in mac
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
#elif UNITY_STANDALONE_OSX
        bool openInsidesOfFolder = false;

        // try mac
        string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

        if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
        {
            openInsidesOfFolder = true;
        }

        //Debug.Log("macPath: " + macPath);
        //Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

        if (!macPath.StartsWith("\""))
        {
            macPath = "\"" + macPath;
        }
        if (!macPath.EndsWith("\""))
        {
            macPath = macPath + "\"";
        }
        string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
        //Debug.Log("arguments: " + arguments);
        try
        {
            System.Diagnostics.Process.Start("open", arguments);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            // tried to open mac finder in windows
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
#endif
    }


    public GameObject saveObject;
    public GameObject DownloadName;
    public TMP_Text saveDetails;
    public GameObject saveDetailsObject;

    private string saveTitle;
    private string saveFileName;

    private string templateDescription;
    public ScrollRect scrollRect;

    public void RefreshSaves()
    {
        string path = SaveSystem.directoryPath;
        ClearSaves();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        BinaryFormatter formatter = new BinaryFormatter();
        foreach (string file in Directory.GetFiles(path))
        {
            try
            {
                FileStream stream = new FileStream(file, FileMode.Open);

                if (file.EndsWith(".dat") || file.EndsWith(".sdat"))
                {
                    PlayerData data = formatter.Deserialize(stream) as PlayerData;

                    AddSave(data.name, file, File.GetLastWriteTime(file));
                }
                else
                {
                    SchoolSaves.PlayerData data = formatter.Deserialize(stream) as SchoolSaves.PlayerData;

                    AddSave(data.Name, file, File.GetLastWriteTime(file));
                }

                stream.Close();
            }
            catch (Exception)
            {
                string a = file.Replace(path + "/", string.Empty);
                AddSave("Unknown Name", a, new DateTime(2021, 5, 7, 19, 26, 23)); // The date is literally the time I'm doing this lol
            }
        }
    }

    public TMP_Text text;

    public GameObject[] saves;

    public TMP_Text loadText;

    public Button saveAsButton;
    public Button saveButton;

    private string Param_CurrentMode;
    private string Param_ModeType;
    private string Param_Scene;
    private Dictionary<string, object> ExtraParameters = new Dictionary<string, object>();

    private void AddSave(string title, string fileName, DateTime createdDate)
    {
        // saves[save] = title;
        // text.text += title + "\n";
        GameObject saveItem = Instantiate(saveObject, scrollRect.content.transform);
        saveItem.GetComponent<RectTransform>().anchoredPosition = saveObject.GetComponent<RectTransform>().anchoredPosition;
        saves[save] = saveItem;
        saveItem.GetComponent<Button>().onClick.AddListener((() =>
        {
            OpenSave(fileName);
        }));
        TMP_Text[] texts = saveItem.GetComponentsInChildren<TMP_Text>();
        texts[0].text = title;
        texts[1].text = createdDate.ToLongDateString() + " | " + createdDate.ToLongTimeString();
        save++;
    }

    public void OpenSave(string path)
    {
        if (!File.Exists(path))
        {
            console.warn("[Save Manager] Save file was deleted: " + path);
            RefreshSaves();
            return;
        }
        console.log("[Save Manager] Opened: " + path);
        saveFileName = path.Replace(SaveSystem.directoryPath, string.Empty);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            DateTime createdDate = File.GetCreationTime(path);
            DateTime updatedDate = File.GetLastWriteTime(path);
            string version;
            string problemText = "<#16f51a>Save is up to date! No problems found.";

            if (path.EndsWith(".dat") || path.EndsWith(".sdat"))
            {
                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                saveTitle = data.name;
                version = data.version;
                if (!IsSchoolScene)
                {
                    Settings.SetString("CurrentMode", "story");
                    Settings.SetString("ModeType", "Normal");
                    Settings.SetString("ModeSelected", "School");
                    if (FindObjectOfType<StarterScript>())
                    {
                        FindObjectOfType<StarterScript>().sceneLoadAwake.sceneName = "School";
                    }
                }
                Param_CurrentMode = "story";
                Param_ModeType = "Normal";
                Param_Scene = "School";
                ExtraParameters.Clear();

                if (data.version == "" || data.version == null)
                {
                    version = "< V1.5";
                    problemText = "<#dff516>Save was saved before version 1.5. File was saved in the old save format. You might experience issues once we convert the save file to the new format.";
                }
                else
                {
                    Version v = new Version(data.version);
                    if (v > VersionInput.currentVersion)
                    {
                        problemText = "<#c26904>Save was saved on version " + v + ". The save file may not load properly. File was saved in the old save format. You might experience issues once we convert the save file to the new format.";
                    }
                    if (v < VersionInput.currentVersion)
                    {
                        problemText = "<#dff516>Save was saved on version " + v + ". File was saved in the old save format. You might experience issues once we convert the save file to the new format.";
                    }
                }
                if (data.stamina == 0f)
                {
                    version = "< V1.4";
                    problemText = "<#c26904>Save was saved before version 1.4. You'll start up with no stamina. File was saved in the old save format. You might experience issues once we convert the save file to the new format.";
                }
            }
            else
            {
                SchoolSaves.PlayerData data = formatter.Deserialize(stream) as SchoolSaves.PlayerData;
                saveTitle = data.Name;
                version = data.Version;
                if (!IsSchoolScene)
                {
                    Settings.SetString("CurrentMode", data.Param_ModeSelected ?? "story");
                    Settings.SetString("ModeType", data.Param_ModeType ?? "Normal");
                    Settings.SetString("ModeSelected", data.Param_Scene ?? "School");

                    if (data.Param_ModeType == "NULL" && (AchievementManager.Instance == null || !AchievementManager.Instance.HasEarned((int)Achievements.undefined)) && Settings.GetInt("NULL_FirstTime") == 0)
                    {
                        Settings.SetString("CurrentMode", "story");
                        Settings.SetString("ModeType", "Normal");
                        Settings.SetString("ModeSelected", "School");
                    }

                    if (FindObjectOfType<StarterScript>())
                    {
                        FindObjectOfType<StarterScript>().sceneLoadAwake.sceneName = data.Param_Scene ?? "School";
                    }
                }
                Param_CurrentMode = data.Param_ModeSelected ?? "story";
                Param_ModeType = data.Param_ModeType ?? "Normal";
                Param_Scene = data.Param_Scene ?? "School";
                ExtraParameters.Clear();
                if (data.Params_Keys != null && data.Params_Keys.Length > 0)
                {
                    for (int i = 0; i < data.Params_Keys.Length; i++)
                    {
                        ExtraParameters.Add(data.Params_Keys[i], data.Params_Values[i]);
                    }
                }
                if (data.Version == "" || data.Version == null)
                {
                    version = "< V1.5";
                    problemText = "<#dff516>Save was saved before version 1.5. No minor problems found!";
                }
                else
                {
                    Version v = new Version(data.Version);
                    if (v > VersionInput.currentVersion)
                    {
                        problemText = "<#c26904>Save was saved on version " + v + ". The save file may not load properly.";
                    }
                    if (v < VersionInput.currentVersion)
                    {
                        problemText = "<#dff516>Save was saved on version " + v + ". You may experience small issues.";
                    }
                }
                if (data.Stamina == 0f)
                {
                    version = "< V1.4";
                    problemText = "<#c26904>Save was saved before version 1.4. You'll start up with no stamina.";
                }
            }

            saveDetails.text = templateDescription;
            saveDetails.text = saveDetails.text.Replace("{SAVE_NAME}", saveTitle).Replace("{FILE_NAME}", saveFileName).Replace("{CREATED_DATE}", GetFullDateTime(createdDate)).Replace("{UPDATED_DATE}", GetFullDateTime(updatedDate)).Replace("{VERSION}", version).Replace("{PROBLEMS}", problemText);
            saveDetailsObject.SetActive(true);

            stream.Close();
        }
        catch (Exception e)
        {
            InputManager.DisplayException(e, 5f);
            saveTitle = "UNKNOWN TITLE";
            DateTime createdDate = File.GetCreationTime(path);
            DateTime updatedDate = File.GetLastWriteTime(path);
            string version;
            string problemText = "<#ff0000>Unable to read save details. The save may be corrupted.";
            if (!IsSchoolScene)
            {
                Settings.SetString("CurrentMode", "story");
                Settings.SetString("ModeType", "Normal");
                Settings.SetString("ModeSelected", "School");
                if (FindObjectOfType<StarterScript>())
                {
                    FindObjectOfType<StarterScript>().sceneLoadAwake.sceneName = "School";
                }
            }
            Param_CurrentMode = "story";
            Param_ModeType = "Normal";
            Param_Scene = "School";
            ExtraParameters.Clear();

            version = "UNKNOWN VERSION";

            saveDetails.text = templateDescription;
            saveDetails.text = saveDetails.text.Replace("{SAVE_NAME}", saveTitle).Replace("{FILE_NAME}", saveFileName).Replace("{CREATED_DATE}", GetFullDateTime(createdDate)).Replace("{UPDATED_DATE}", GetFullDateTime(updatedDate)).Replace("{VERSION}", version).Replace("{PROBLEMS}", problemText);
            saveDetailsObject.SetActive(true);

            console.logException(e);
        }
    }

    private string GetFullDateTime(DateTime createdDate)
    {
        return createdDate.ToLongDateString() + " | " + createdDate.ToLongTimeString();
    }

    public void ClearSaves()
    {
        for (int i = 0; i < save; i++)
        {
            Destroy(saves[i], 0f);
            saves[i] = null;
        }
        text.text = string.Empty;
        save = 0;
    }

    public void LoadFile()
    {
        // NULL Access Check
        if (Param_ModeType == "NULL" && (AchievementManager.Instance == null || !AchievementManager.Instance.HasEarned((int)Achievements.undefined)) && Settings.GetInt("NULL_FirstTime") == 0)
        {
            Settings.DeleteKey("CurrentMode");
            Settings.DeleteKey("ModeType");
            Settings.DeleteKey("ModeSelected");
            GameSystem.AbortApplication("null", "NULL_ACCESS_DENIED");
            return;
        }
        PlayerPrefs.SetString("Save", saveFileName);
        SceneLoader.LoadSceneAsync(Param_Scene);
        Settings.SetString("CurrentMode", Param_CurrentMode);
        Settings.SetString("ModeType", Param_ModeType);
        Settings.SetString("ModeSelected", Param_Scene);
        if (Param_ModeType == "NULL")
        {
            foreach (KeyValuePair<string, object> Pair in ExtraParameters)
            {
                if (Pair.Value.GetType() == typeof(int))
                {
                    Settings.SetInt(Pair.Key, (int)Pair.Value);
                }
                else if (Pair.Value.GetType() == typeof(float))
                {
                    Settings.SetFloat(Pair.Key, (float)Pair.Value);
                }
                else if (Pair.Value.GetType() == typeof(string))
                {
                    Settings.SetString(Pair.Key, (string)Pair.Value);
                }
                else if (Pair.Value.GetType() == typeof(bool))
                {
                    Settings.SetBool(Pair.Key, (bool)Pair.Value);
                }
            }
        }
    }

    public SceneLoader SceneLoader;

    public void SaveFile(TMP_InputField text)
    {
        string fileName = text.text.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace(" ", "-");
        if (fileName.Length > 10)
        {
            fileName = fileName.Substring(0, 10);
        }
        SaveSystem.SavePlayer(gameController, fileName, text.text);
        RefreshSaves();
    }

    public void OverwriteSave()
    {
        SchoolSaves.PlayerData data = SaveSystem.SavePlayer(gameController, Path.GetFileNameWithoutExtension(saveFileName.Replace("_schoolData.dat", "")), saveTitle);
        RefreshSaves();
        Param_CurrentMode = data.Param_ModeSelected ?? "story";
        Param_ModeType = data.Param_ModeType ?? "Normal";
        Param_Scene = data.Param_Scene ?? "School";
        ExtraParameters.Clear();
        if (data.Params_Keys != null && data.Params_Keys.Length > 0)
        {
            for (int i = 0; i < data.Params_Keys.Length; i++)
            {
                ExtraParameters.Add(data.Params_Keys[i], data.Params_Values[i]);
            }
        }
    }

    public void DeleteFile()
    {
        SaveSystem.DeletePlayer(saveFileName);
        RefreshSaves();
    }

    public void Close()
    {
        SceneManager.UnloadSceneAsync("SaveSystem");
    }

    public static GameControllerScript gameController;

    private int save = 1;
}

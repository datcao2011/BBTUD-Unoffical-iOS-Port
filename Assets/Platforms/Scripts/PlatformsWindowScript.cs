#if UNITY_EDITOR
using UnityEditor;
using Platforms.Console.Logger;
using UnityEngine;
using System.IO;
using System;

namespace Platforms
{
    public class PlatformsWindowScript : EditorWindow
    {
        [MenuItem("Platforms/Configure")]
        public static void ShowWindow()
        {
            GetWindow<PlatformsWindowScript>("Platforms Configuration");
        }

        int selected = 0;
        internal static bool SetupScreenFinished;
        private string keyField;
        private bool s1;
        private GameObject copiedGameObject;

        #region Settings
        // SETTINGS
        private bool onlyImportantBoxes;

        // LAST SAVED SETTINGS
        private bool lOnlyImportantBoxes;
        #endregion
        #region Dialogs
        private bool CreateDialog(string title, string info, string ok)
        {
            if (!onlyImportantBoxes)
                return EditorUtility.DisplayDialog(title, info, ok);
            return true;
        }

        private bool CreateDialog(string title, string info, string ok, string cancel)
        {
            if (!onlyImportantBoxes)
                return EditorUtility.DisplayDialog(title, info, ok, cancel);
            return true;
        }
        #endregion
        private static Version CheckUnityVersion()
        {
            string version = Application.unityVersion.Replace("f1", "").Replace("f2", "").Replace("f3", "").Replace("f4", "").Replace("f5", "").Replace("f6", "");
            return new Version(version);
        }

        private void OnGUI()
        {
            if (!SetupScreenFinished) { RenderSetupScreen(); return; }
            GUILayout.Space(1.5f);
            string[] categories = new string[3]
            {
                "About",
                "Input",
                "Settings"
            };
            selected = GUILayout.Toolbar(selected, categories);
            GUILayout.Space(1f);
            GUILayout.Label(categories[selected] + ":", EditorStyles.largeLabel);
            GUILayout.Space(1f);
            if (selected == 0)
            {
                GUILayout.Label("1.1 Changelog\n- You do not need to authorize anymore.\n- Optimized windows input system, it shouldn't lower frames anymore.", EditorStyles.boldLabel);
                GUILayout.Space(5f);

                GUILayout.Label("Unity Input Platforms\n\nSimplified asset for input with multiple platforms.\n\n© Copyright 2020-2023 SWA");
                GUILayout.Space(1f);
                GUILayout.Label("Development Info:", EditorStyles.largeLabel);
                GUILayout.Label("Version " + PlatformsUpdater.version + " Release");
                EditorGUILayout.HelpBox("Platforms is still in development. You may experience bugs, glitches, etc.", MessageType.Warning);
            }
            if (selected == 1)
            {
                if (!Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("Platforms use a Input Platform Manager as a GameObject. To manage its input settings, please select the GameObject or create one if haven't already.", MessageType.Info);
                    bool hasInputManager = FindObjectOfType<InputPlatformManager>();
                    bool hasKeyConfiguration = FindObjectOfType<KeybindManager>();
                    if (hasInputManager)
                    {
                        if (GUILayout.Button("Edit Input Manager"))
                        {
                            Selection.activeGameObject = FindObjectOfType<InputPlatformManager>().gameObject;
                            maximized = false;
                        }
                        if (GUILayout.Button("Copy Input Manager"))
                        {
                            EditorUtility.DisplayProgressBar("Platforms Loader System", "Copying input manager...", 0f);
                            if (!System.IO.Directory.Exists(Application.dataPath + "/Platforms/Input Manager Copy/"))
                            {
                                System.IO.Directory.CreateDirectory(Application.dataPath + "/Platforms/Input Manager Copy/");
                            }
                            bool success;
                            if (System.IO.File.Exists(Application.dataPath + "/Platforms/Input Manager Copy/InputManager.prefab"))
                            {
                                System.IO.File.Delete(Application.dataPath + "/Platforms/Input Manager Copy/InputManager.prefab");
                            }
                            try
                            {
                                copiedGameObject = PrefabUtility.SaveAsPrefabAsset(FindObjectOfType<InputPlatformManager>().gameObject, Application.dataPath + "/Platforms/Input Manager Copy/InputManager.prefab", out success);
                            }
                            catch (System.Exception)
                            {
                                EditorUtility.DisplayDialog("Platforms", "There was an prefab error while copying the input manager. Check the log for more details.", "OK");
                                EditorUtility.ClearProgressBar();
                                ConsoleLogger.RunException("Input manager failed to copy.");
                                throw;
                            }
                            EditorUtility.DisplayProgressBar("Platforms Loader System", "Copying input manager...", 1f);
                            if (success)
                            {
                                CreateDialog("Platforms", "Copied!", "OK");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Platforms", "Cannot copy input manager. Please check the log for more details.", "OK");
                            }
                            EditorUtility.ClearProgressBar();
                        }
                        if (copiedGameObject != null)
                        {
                            if (GUILayout.Button("Paste Input Manager"))
                            {
                                EditorUtility.DisplayProgressBar("Platforms Loader System", "Pasting input manager...", 0f);
                                if (hasInputManager)
                                {
                                    EditorUtility.DisplayProgressBar("Platforms Loader System", "Waiting for confirmation...", 0.1f);
                                    if (!CreateDialog("Platforms", "Pasting input manager will remove the old manager.", "Delete old manager", "Cancel"))
                                    {
                                        EditorUtility.DisplayProgressBar("Platforms Loader System", "Canceling...", 1f);
                                        CreateDialog("Platforms", "Input manager not pasted.", "OK");
                                        EditorUtility.ClearProgressBar();
                                    }
                                    else
                                    {
                                        EditorUtility.DisplayProgressBar("Platforms Loader System", "Deleting old input manager...", 0.15f);
                                        UnityEngine.Object.DestroyImmediate(FindObjectOfType<InputPlatformManager>().gameObject);
                                        EditorUtility.DisplayProgressBar("Platforms Loader System", "Deleting old input manager...", 0.30f);
                                        EditorUtility.DisplayProgressBar("Platforms Loader System", "Pasting input manager...", 0.56f);
                                        PrefabUtility.InstantiatePrefab(copiedGameObject).name = "Platforms Input Manager";
                                        EditorUtility.DisplayProgressBar("Platforms Loader System", "Pasting input manager...", 1f);
                                        CreateDialog("Platforms", "Pasted!", "OK");
                                        EditorUtility.ClearProgressBar();
                                    }
                                }
                            }
                        }
                        if (GUILayout.Button("Remove Input Manager"))
                        {
                            if (EditorUtility.DisplayDialog("Platforms", "Are you sure you want to remove the Input Manager?", "Yes", "No"))
                            {
                                DestroyImmediate(FindObjectOfType<InputPlatformManager>().gameObject);
                            }
                        }

                        GUILayout.Space(3f);
                        if (hasKeyConfiguration)
                        {
                            if (GUILayout.Button("Edit UI Key Configuration"))
                            {
                                Selection.activeGameObject = FindObjectOfType<KeybindManager>().gameObject;
                                maximized = false;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Create UI Key Configuration"))
                            {
                                GameObject createdGameObject = new GameObject();
                                createdGameObject.name = "Platforms UI Key Configuration";
                                createdGameObject.AddComponent<KeybindManager>().PlatformsInputManager = FindObjectOfType<PlatformAxisInput>();
                                Selection.activeGameObject = FindObjectOfType<KeybindManager>().gameObject;
                                maximized = false;
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label("You have not created Input Manager in this scene.");
                        if (GUILayout.Button("Create Input Manager"))
                        {
                            GameObject createdGameObject = new GameObject();
                            createdGameObject.name = "Platforms Input Manager";
                            createdGameObject.AddComponent<InputPlatformManager>();
                            Selection.activeGameObject = FindObjectOfType<InputPlatformManager>().gameObject;
                            maximized = false;
                        }
                        if (copiedGameObject != null)
                        {
                            if (GUILayout.Button("Paste Input Manager"))
                            {
                                EditorUtility.DisplayProgressBar("Platforms Loader System", "Pasting input manager...", 0f);
                                EditorUtility.DisplayProgressBar("Platforms Loader System", "Pasting input manager...", 0.56f);
                                PrefabUtility.InstantiatePrefab(copiedGameObject).name = "Platforms Input Manager";
                                EditorUtility.DisplayProgressBar("Platforms Loader System", "Pasting input manager...", 1f);
                                CreateDialog("Platforms", "Pasted!", "OK");
                                EditorUtility.ClearProgressBar();
                            }
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Input tools are not available during play mode.", MessageType.Error);
                    EditorGUILayout.HelpBox("To stop play mode, press the play button again.", MessageType.Info);
                }
            }
            if (selected == 2)
            {
                if (!s1)
                {
                    s1 = true;
                    path = Application.dataPath + "/Platforms/setting.ini";
                    bool.TryParse(GetValue("importantBoxes"), out lOnlyImportantBoxes);

                    onlyImportantBoxes = lOnlyImportantBoxes;
                }
                GUILayout.Label("Get Only Important Dialog Boxes");
                onlyImportantBoxes = EditorGUILayout.Toggle(onlyImportantBoxes);
                if (!onlyImportantBoxes)
                {
                    GUILayout.Label("You will get all dialog boxes.", EditorStyles.boldLabel);
                }
                else
                {
                    GUILayout.Label("You will get only important dialog boxes.", EditorStyles.boldLabel);
                }
                try
                {
                    if (lOnlyImportantBoxes != onlyImportantBoxes)
                    {
                        SaveValue("importantBoxes", onlyImportantBoxes.ToString());
                        lOnlyImportantBoxes = onlyImportantBoxes;
                    }
                }
                catch (System.Exception)
                {

                    throw;
                }

                if (GUILayout.Button("Check and Install Updates"))
                {
                    PlatformsUpdater.CheckAndInstallUpdates();
                }
            }
        }

        #region Setup Render Screen
        private int setup_page = 1;

        private void RenderSetupScreen()
        {
            if (setup_page == 1)
            {
                GUILayout.Label("Welcome to Platforms!", EditorStyles.largeLabel);
                GUILayout.Space(2.5f);
                GUILayout.Label("Thanks for installing Platforms, the Simplified Input System. Let's start with a few things first. Click Next to continue.");
            }
            else if (setup_page == 2)
            {
                GUILayout.Label("Where Can I Find My Tools?", EditorStyles.largeLabel);
                GUILayout.Space(2.5f);
                GUILayout.Label("You can find on the tools in the configure section (Platforms > Configure).");
                GUILayout.Label("These tools will try to help use Platforms.");
            }
            else if (setup_page == 3)
            {
                GUILayout.Label("I Can't Find What I Need.", EditorStyles.largeLabel);
                GUILayout.Space(2.5f);
                GUILayout.Label("If you need any assistance, please post a comment or email swagamesofficial@gmail.com");
            }
            else if (setup_page == 4)
            {
                GUILayout.Label("Ready?", EditorStyles.largeLabel);
                GUILayout.Space(2.5f);
                GUILayout.Label("Platforms is still in development. Anything may change.");
                GUILayout.Label("You can check for updates on the Settings section.");
            }

            // Buttons
            EditorGUILayout.BeginHorizontal(); // Horizontal group

            EditorGUI.BeginDisabledGroup(setup_page <= 1);
            if (GUILayout.Button("Back"))
            {
                setup_page--;
            }
            EditorGUI.EndDisabledGroup();
            if (setup_page < 4)
            {
                if (GUILayout.Button("Next"))
                {
                    setup_page++;
                }
            }
            else
            {
                if (GUILayout.Button("Finish"))
                {
                    SetupScreenFinished = true;
                    EditorPrefs.SetBool("PFFU-SS2", true);
                }
            }
            EditorGUILayout.EndHorizontal(); // End of group
        }
        #endregion

        // NOTE: The SaveValue and GetValue I Did Not Write And I got the code from online. If you are the owner of this, please contact SWA

        // GET VALUE
        private string path;
        private string GetValue(string key)
        {
            if (!File.Exists(path))
            {
                StreamWriter writer = new StreamWriter(path);
                writer.Write("");
                writer.Close();
            }
            StreamReader reader = new StreamReader(path);
            int i = 0;
            int max = File.ReadAllLines(path).Length - 1;
            string result = "";
            bool tf = true;
            while (tf)
            {
                string str1 = reader.ReadLine();
                if (str1 != null)
                    if (str1.Split('=')[0] == key)
                    {
                        result = str1.Split('=')[1];
                        tf = false;
                    }
                if (i >= max) tf = false;
                i++;
            }
            reader.Close();
            if (result == "True") result = "true";
            if (result == "False") result = "false";
            return result;
        }

        // SAVE VALUE
        private void SaveValue(string key, string value)
        {
            File.WriteAllText(path, $"{key}={value}");
        }
    }
}
#endif
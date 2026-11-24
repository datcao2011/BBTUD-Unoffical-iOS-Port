using UnityEngine;
using Platforms.Console.Logger;
using System.IO;
using UnityEngine.UI;
using System.Linq;

namespace Platforms
{
    public class KeybindManager : MonoBehaviour
    {
        public PlatformAxisInput PlatformsInputManager;

        public KeyBindsConfigurtion Configuration;

        private PlatPlayer player;

        private GameObject template;
        private bool ready;

        private bool LastControllerDetected;

        private void Update()
        {
            if (!PlatformsInputManager & !ready) PlatformsInputManager = FindObjectOfType<PlatformAxisInput>();
            else if (!ready && PlatformsInputManager.isReady)
            {
                ready = true;
                if (player == null) player = PlatTools.GetPlatformPlayer();
                if (PlatformsInputManager != null && Configuration != null)
                {
                    if (Configuration.keyBinds.Count <= 0)
                    {
                        Transform g = Configuration.transform.GetComponentInChildren<ScrollRect>().content;
                        Transform[] children = g.Cast<Transform>().ToArray();

                        for (int i = 0; i < children.Length; i++)
                        {
                            Transform gameObject = children[i];

                            PlatformAxises[] result = System.Array.FindAll(PlatformsInputManager.axises, element => element.AxisName == gameObject.GetChild(0).GetComponent<Text>().text);

                            PlatformAxises axis;
                            if (result.Length == 0)
                            {
                                axis = new PlatformAxises();
                                axis.AxisName = gameObject.GetChild(0).GetComponent<Text>().text;
                            }
                            else axis = result[0];

                            KeyBindsConfigurtion.KeyBind keyBind = new KeyBindsConfigurtion.KeyBind();
                            keyBind.MainButton = gameObject.GetChild(1).GetComponent<Button>();
                            keyBind.AlternativeButton = gameObject.GetChild(2).GetChild(1).GetComponent<Button>();

                            keyBind.KeyName = axis.AxisName;
                            keyBind.MainButton.GetComponentInChildren<Text>().text = player.GetKeyName(axis.AxisName);
                            keyBind.AlternativeButton.GetComponentInChildren<Text>().text = player.GetKeyName(axis.AxisName, true);

                            keyBind.MainButton.onClick.AddListener(() =>
                            {
                                Configuration.textInput = keyBind.MainButton.GetComponentInChildren<Text>();
                                Configuration.StartAssignment(keyBind.KeyName);
                            });
                            keyBind.AlternativeButton.onClick.AddListener(() =>
                            {
                                Configuration.textInput = keyBind.AlternativeButton.GetComponentInChildren<Text>();
                                Configuration.StartAssignmentAlt(keyBind.KeyName);
                            });

                            Configuration.keyBinds.Add(keyBind);
                        }
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (!PlatformsInputManager || !PlatformsInputManager.isReady) return;
            if (CurrentDevice.currentPlatform != PFPlatform.Windows) return;
            if (Configuration != null && Configuration.waitingForKey) return;
            // Controller update
            bool IsGamepadConnected = GamePadController.isAnyGamepadConnected && Controller.isEnabled;
            if (IsGamepadConnected != LastControllerDetected)
            {
                ready = false; // This will go and update.
                // Reset all buttons too
                foreach (KeyBindsConfigurtion.KeyBind key in Configuration.keyBinds)
                {
                    key.MainButton.onClick.RemoveAllListeners();
                    key.AlternativeButton.onClick.RemoveAllListeners();
                }
                Configuration.keyBinds.Clear();
            }
            LastControllerDetected = IsGamepadConnected;
        }

        public void GenerateUI()
        {
            if (Resources.Load("Keybind Prefab") == null)
            {
                ConsoleLogger.LogError($"The following file doesn't exist: {Application.dataPath.Replace(Path.DirectorySeparatorChar, '/')}/Platforms/Resources/Keybind Prefab.prefab");
                return;
            }
            if (PlatformsInputManager.axises.Length == 0)
            {
                ConsoleLogger.LogError("No Axises were found on the Input Manager.");
                return;
            }
            GameObject prefab = Resources.Load<GameObject>("Keybind Prefab");
            prefab = Object.Instantiate(prefab);
            prefab.name = "Platforms Configuration Manager";
            prefab.transform.SetParent(transform);
            Configuration = prefab.GetComponent<KeyBindsConfigurtion>();

            template = Configuration.transform.GetComponentInChildren<ScrollRect>().content.GetChild(0).gameObject;

            for (int i = 0; i < PlatformsInputManager.axises.Length; i++)
            {
                PlatformAxises axis = PlatformsInputManager.axises[i];
                Transform g = Instantiate(template, template.transform.position, template.transform.rotation).transform;

                g.SetParent(Configuration.transform.GetChild(1).GetComponent<ScrollRect>().content.transform);
                g.localScale = new Vector3(1f, 1f, 1f);
                g.GetChild(0).GetComponent<Text>().text = axis.AxisName;
                if (axis.positiveAxisKey == KeyCode.None && axis.mouseButton != MouseButton.None)
                {
                    g.GetChild(1).GetComponentInChildren<Text>().text = axis.mouseButton + " Mouse Button";
                }
                else
                {
                    g.GetChild(1).GetComponentInChildren<Text>().text = axis.positiveAxisKey.ToString();
                }
                if (axis.negativeAxisKey == KeyCode.None && axis.alternativeMouseButton != MouseButton.None)
                {
                    g.GetChild(2).GetChild(1).GetComponentInChildren<Text>().text = axis.alternativeMouseButton + " Mouse Button";
                }
                else
                {
                    g.GetChild(2).GetChild(1).GetComponentInChildren<Text>().text = axis.negativeAxisKey.ToString();
                }

                g.GetChild(3).GetComponent<Text>().text = axis.AxisName;
                g.GetChild(0).gameObject.SetActive(false);
                g.GetChild(3).gameObject.SetActive(true);
            }

            DestroyImmediate(template);
        }
    }
}
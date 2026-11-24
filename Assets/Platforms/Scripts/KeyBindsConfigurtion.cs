using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Platforms.XInputDotNetPure;

namespace Platforms
{
    public class KeyBindsConfigurtion : MonoBehaviour
    {
        [HideInInspector]
        public List<KeyBind> keyBinds = new List<KeyBind>();
        private KeyCode newKey;
        [HideInInspector]
        public Text textInput;
        private Event keyCurrent;
        public bool waitingForKey;
        private int mouseButton;

        private int Controller_Trigger;
        private int Controller_DPad;
        private int Controller_Stick;
        private int Controller_Button;

        public static bool IsActive;

        private void Start()
        {
            platformPlayer = PlatTools.GetPlatformPlayer();
        }

        private void OnEnable()
        {
            IsActive = true;
        }

        private void OnDisable()
        {
            IsActive = false;
        }

        private void Update()
        {
            if (waitingForKey && GamePadController.isAnyGamepadConnected && Controller.isEnabled)
            {
                // Gamepad Triggers
                if (PlatTools.GetTriggerDown(ControllerTriggers.Left))
                {
                    Controller_Trigger = (int)ControllerTriggers.Left;
                    waitingForKey = false;
                    return;
                }
                if (PlatTools.GetTriggerDown(ControllerTriggers.Right))
                {
                    Controller_Trigger = (int)ControllerTriggers.Right;
                    waitingForKey = false;
                    return;
                }
                GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
                // Gamepad Thumbsticks
                if (gamepad.ThumbSticks.Left.X != 0f)
                {
                    Controller_Stick = (int)ControllerSticks.X_Left;
                    waitingForKey = false;
                    return;
                }
                if (gamepad.ThumbSticks.Right.X != 0f)
                {
                    Controller_Stick = (int)ControllerSticks.X_Right;
                    waitingForKey = false;
                    return;
                }
                if (gamepad.ThumbSticks.Left.Y != 0f)
                {
                    Controller_Stick = (int)ControllerSticks.Y_Left;
                    waitingForKey = false;
                    return;
                }
                if (gamepad.ThumbSticks.Right.Y != 0f)
                {
                    Controller_Stick = (int)ControllerSticks.Y_Right;
                    waitingForKey = false;
                    return;
                }
                // Gamepad DPad
                if (gamepad.DPad.Left == ButtonState.Pressed)
                {
                    Controller_DPad = (int)ControllerDPad.Left;
                    waitingForKey = false;
                    return;
                }
                if (gamepad.DPad.Right == ButtonState.Pressed)
                {
                    Controller_DPad = (int)ControllerDPad.Right;
                    waitingForKey = false;
                    return;
                }
                if (gamepad.DPad.Up == ButtonState.Pressed)
                {
                    Controller_DPad = (int)ControllerDPad.Up;
                    waitingForKey = false;
                    return;
                }
                if (gamepad.DPad.Down == ButtonState.Pressed)
                {
                    Controller_DPad = (int)ControllerDPad.Down;
                    waitingForKey = false;
                    return;
                }
                // Gamepad Buttons
                for (int i = 330; i <= 349; i++)
                {
                    if (Input.GetKeyDown((KeyCode)i))
                    {
                        Controller_Button = 1;
                        newKey = (KeyCode)i;
                        waitingForKey = false;
                        return;
                    }
                }
            }
        }

        private void OnGUI()
        {
            keyCurrent = Event.current;
            if (waitingForKey && GamePadController.isAnyGamepadConnected && Controller.isEnabled)
            {
                if (keyCurrent.isKey)
                {
                    newKey = keyCurrent.keyCode;
                    mouseButton = 0;
                    Controller_DPad = -1;
                    Controller_Stick = -1;
                    Controller_Trigger = -1;
                    waitingForKey = false;
                }
                return;
            }
            if (!waitingForKey) return;
            if (Input.GetMouseButtonDown(0))
            {
                mouseButton = 1;
                waitingForKey = false;
                return;
            }
            if (Input.GetMouseButtonDown(1))
            {
                mouseButton = 2;
                waitingForKey = false;
                return;
            }
            if (Input.GetMouseButtonDown(2))
            {
                mouseButton = 3;
                waitingForKey = false;
                return;
            }
            if (Input.GetMouseButtonDown(3))
            {
                mouseButton = 4;
                waitingForKey = false;
                return;
            }
            if (Input.GetMouseButtonDown(4))
            {
                mouseButton = 5;
                waitingForKey = false;
                return;
            }
            if (Input.GetMouseButtonDown(5))
            {
                mouseButton = 6;
                waitingForKey = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                newKey = KeyCode.LeftShift;
                mouseButton = 0;
                waitingForKey = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                newKey = KeyCode.RightShift;
                mouseButton = 0;
                waitingForKey = false;
                return;
            }
            if (keyCurrent.isKey)
            {
                newKey = keyCurrent.keyCode;

                mouseButton = 0;
                waitingForKey = false;
            }
        }

        public void ResetKeyBinds()
        {
            StopAllCoroutines();
            waitingForKey = false;
            foreach (KeyBind keyBind in keyBinds)
            {
                platformPlayer.ResetKeyBind(keyBind.KeyName);
                keyBind.MainButton.GetComponentInChildren<Text>().text = platformPlayer.GetKeyName(keyBind.KeyName);
                keyBind.AlternativeButton.GetComponentInChildren<Text>().text = platformPlayer.GetKeyName(keyBind.KeyName, true);
            }
        }

        public void StartAssignment(string name)
        {
            StartAssignment(name, false);
        }

        public void StartAssignmentAlt(string name)
        {
            StartAssignment(name, true);
        }

        public void StartAssignment(string KeyName, bool alt)
        {
            if (!waitingForKey)
                StartCoroutine(AssignKey(KeyName, alt));
        }

        private IEnumerator WaitForKey()
        {
            while (!keyCurrent.isKey && mouseButton == 0 && Controller_DPad == -1 && Controller_Trigger == -1 && Controller_Stick == -1 && Controller_Button == -1)
                yield return null;
        }

        private IEnumerator AssignKey(string keyName, bool alt)
        {
            if (waitingForKey) yield break; // Can't wait for key while waiting for another key.
            textInput.text = "> " + textInput.text + " <";
            mouseButton = 0;
            Controller_DPad = -1;
            Controller_Trigger = -1;
            Controller_Stick = -1;
            Controller_Button = -1;
            waitingForKey = true;
            yield return WaitForKey();
            PlatformAxises[] finder = System.Array.FindAll<PlatformAxises>(FindObjectOfType<PlatformAxisInput>().axises, element => element.AxisName == keyName);
            KeyBind[] result = System.Array.FindAll<KeyBind>(keyBinds.ToArray(), element => element.KeyName == keyName);

            if (finder.Length == 0)
            {
                textInput.text = "ERROR";

                try
                {
                    Console.Logger.ConsoleLogger.RunException("Unable to locate axis " + keyName + ". This can happen if the text label has changed to a different unlocated axis name.");
                }
                catch (System.Exception ex)
                {
                    Debug.unityLogger.LogException(ex);
                }
#if !UNITY_EDITOR
                textInput.text = "Contact Game Owner";
                InputPlatformManager inputPlatformManager = FindObjectOfType<InputPlatformManager>();
                if (inputPlatformManager)
                {
                    if (inputPlatformManager.showErrorMessagesInGame)
                    {
                        inputPlatformManager.CreateErrorMessage(new System.Exception("An error occurred. Please contact game owner and give them the error message that will be provided by pressing 'X'."), 30f);
                    }
                    else
                    {
                        inputPlatformManager.CreateErrorMessage(new System.Exception("An error occurred. Please contact game owner and send the log file (if possible)."), 30f);
                    }
                }
#endif
                yield break;
            }

            KeyBind keyBind = result[0];
            if (Controller_Trigger >= 0 && !alt)
            {
                platformPlayer.SetTriggerBind(keyBind.KeyName, (ControllerTriggers)Controller_Trigger);
                textInput.text = platformPlayer.GetKeyName(keyBind.KeyName, alt);
                yield break;
            }
            if (Controller_DPad >= 0 && !alt)
            {
                platformPlayer.SetDPadBind(keyBind.KeyName, (ControllerDPad)Controller_DPad);
                textInput.text = platformPlayer.GetKeyName(keyBind.KeyName, alt);
                yield break;
            }
            if (Controller_Stick >= 0 && !alt)
            {
                platformPlayer.SetStickBind(keyBind.KeyName, (ControllerSticks)Controller_Stick);
                textInput.text = platformPlayer.GetKeyName(keyBind.KeyName, alt);
                yield break;
            }
            if (mouseButton > 0)
            {
                platformPlayer.SetMouseBind(keyBind.KeyName, alt, (MouseButton)mouseButton);

                textInput.text = platformPlayer.GetKeyName(keyBind.KeyName, alt);
                yield break;
            }
            platformPlayer.SetKeyBind(keyBind.KeyName, alt, newKey);

            textInput.text = platformPlayer.GetKeyName(keyBind.KeyName, alt);
            yield break;
        }

        public void SendText(Text text)
        {
            textInput = text;
        }

        public class KeyBind
        {
            public string KeyName;
            public Button MainButton;
            public Button AlternativeButton;
        }

        private PlatPlayer platformPlayer;
    }
}
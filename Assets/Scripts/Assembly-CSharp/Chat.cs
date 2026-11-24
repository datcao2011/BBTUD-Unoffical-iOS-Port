using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Multiplayer;
using Platforms;
using Photon.Pun;

public class Chat : MonoBehaviour
{
    public TMP_Text chat;
    public static Chat Manager;
    public TMP_Text popupMessage;
    public GameObject Box;
    public bool isOpened;
    public TMP_InputField InputField;
    public Animator anim;
    public GameControllerScript gc;
    private PlatPlayer input;
    [SerializeField]
    private GameObject TouchOff;

    // Start is called before the first frame update
    private void Start()
    {
        Manager = this;
        gc = FindObjectOfType<GameControllerScript>();
        input = PlatTools.GetPlatformPlayer();
        FilterManager = new WordsFilter();
        string key = "pressing " + input.GetKeyName("Chat");
        if (InputManager.usingTouch)
        {
            key = "pressing the chat button";
        }
        InputField.onSubmit.AddListener(SendMsg);
        if (MultiplayerManager.isActive)
        {
            AddMessage($"Access to chat anytime by {key}.");
        }
        else
        {
            anim.SetTrigger("2");
        }
    }

    private WordsFilter FilterManager;

    public void SendMsg(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        string user = PhotonNetwork.IsConnected ? PhotonNetwork.NickName : "Player";
        if (AccountManager.hasInstance() && !string.IsNullOrEmpty(AccountManager.getSessionId()))
        {
            user = AccountManager.getUsername();
        }
        string str = $"[{user}] {message}";
        InputField.text = string.Empty;
        InputField.ActivateInputField();

        if (MultiplayerManager.isActive)
        {
            ClientSend.MessageSend(FilterManager.Filter(message));
        }
        else
        {
            AddMessage(str);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isOpened && !gc.gamePaused)
        {
            ToggleChat(false);
        }
        if (input.GetButtonDown("Chat"))
        {
            if ((isOpened && !InputField.isFocused) || !isOpened)
            {
                ToggleChat(!isOpened);
            }
        }
#if !UNITY_ANDROID
        if (isOpened)
        {
            TouchOff.SetActive(InputManager.usingTouch);
        }
#endif
    }

    public void ToggleChat(bool show)
    {
        if (show && MultiplayerManager.isActive && MultiplayerManager.instance.PauseMovement)
        {
            return;
        }
        if (gc.learningActive) return;
        isOpened = show;
        Box.SetActive(show);
        if (show)
        {
            anim.SetTrigger("2");
            if (!gc.gamePaused)
            {
                gc.PauseGame();
                gc.pauseText.SetActive(false);
            }
        }
        else
        {
            if (gc.gamePaused)
            {
                gc.UnpauseGame();
            }
        }

        if (MultiplayerManager.isActive)
        {
            MultiplayerManager.instance.PauseMovement = show;
        }
    }

    public void Toggle_Mobile()
    {
        ToggleChat(!isOpened);
    }

    public void AddMessage(string text)
    {
        text = FilterManager.Filter(text);
        console.log(text);
        chat.text += text + "\n";
        if (!isOpened && Settings.GetString("ChatPopups", "true") == "true")
        {
            anim.SetTrigger("1");
            popupMessage.text = text;
        }
    }
}

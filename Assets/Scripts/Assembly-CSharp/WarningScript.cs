using UnityEngine;
using TMPro;
using Platforms;
using UnityEngine.SceneManagement;

public class WarningScript : MonoBehaviour
{

    public int pressed;

    [TextArea]
    public string[] nextTexts = new string[3];

    public TMP_Text mainText;

    public TMP_Text continueText;

    public ManageScenes scene;

    private bool loadingScene;

    public AudioClip[] audioClipsSetup;

    private PlatPlayer player;

    private void Start()
    {
        player = PlatTools.GetPlatformPlayer();
        if (!Settings.HasKey("TimeSpeed"))
        {
            Settings.SetFloat("TimeSpeed", 1f);
        }
        if (Settings.HasKey("Account"))
        {
            InputManager.DisplayEvent("<color=#60FF00>Welcome, " + HasNameTag(Settings.GetString("Account")) + "!</color>\n<color=red>BBTUD Community Member</color>", Color.green, 5f, (msg) => { msg.Disappear(); AccountManager.LogoutSession(); }, "Logout");
        }
    }

    private string HasNameTag(string username)
    {
        string color = "";
        switch (AccountManager.UserTag.ToLower())
        {
            case "creator":
                color = "#2fad42";
                break;
            case "helper":
                color = "#2f98ad";
                break;
            case "yt":
                color = "red";
                break;
            case "supporter":
                color = "yellow";
                break;
            case "dev":
            case "verified":
                color = "blue";
                break;
        }
        return AccountManager.UserTag == "" ? username : (username + " (<color=" + color + ">" + AccountManager.UserTag.ToUpper() + "</color>)");
    }

    private void Update()
    {
        if (InputManager.usingTouch)
        {
            continueText.text = "TAP TO CONTINUE";
        }
        else if (InputManager.isUsingController)
        {
            continueText.text = "PRESS ANY GAMEPAD BUTTON TO CONTINUE";
        }
        else
        {
            continueText.text = "PRESS ANY KEY OR CLICK TO CONTINUE";
        }
        if (player.GetAnyButtonDown & !loadingScene)
        {
            if (pressed == 0)
            {
                mainText.text = "Anyway, this mod includes many modes, features, and cool things that's built to the original schoolhouse. Heard of multiplayer? Well, there's a mod on it, but this is more fun! You can PLAY AS Baldi and even beat Baldi with more players! Is that crazy? I know right. You also get to play ACTUAL Hide & Seek with Baldi! The mod version you're playing on is v" + VersionInput.currentVersion + ".";
                pressed++;
            }
            else if (pressed >= nextTexts.Length)
            {
                loadingScene = true;
                continueText.gameObject.SetActive(false);
                StartCoroutine(LoadMainMenuScene());
            }
            else
            {
                mainText.text = nextTexts[pressed];
                pressed++;
            }
        }
    }

    private System.Collections.IEnumerator LoadMainMenuScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainMenu");
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            mainText.text = "Loading Main Menu (" + Mathf.CeilToInt(progress * 100f) + "%)\nThank you for playing Baldi's Basics TUD.";
            yield return null;
        }
        yield break;
    }
}

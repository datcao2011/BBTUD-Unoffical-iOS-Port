using UnityEngine;
using System.IO;

public class Credits : MonoBehaviour
{
    private string configPath;

    private string dataPath;

    public TMPro.TMP_Text[] slideshowTexts;

    public TMPro.TMP_FontAsset[] fonts;

    public TMPro.TMP_Text fullText;

    private void Awake()
    {
        configPath = Application.dataPath + "/Credits Data/config.ini";
        dataPath = Application.dataPath + "/Credits Data";
        Cursor.SetCursor(null, new Vector2(0f, 0f), CursorMode.Auto);
    }

    private void Start()
    {
#if UNITY_STANDALONE && UNITY_EDITOR
        ReadConfigAndModify();
#endif
    }

    public void ReadConfigAndModify()
    {
        if (!Directory.Exists(dataPath)) return;
        if (!INIConfigure.INI_Exists(configPath))
        {
            Debug.Log("No config file found! Using original text.");
            return;
        }
        try
        {
            if (INIConfigure.GetValue(configPath, "custom_credits") != "true")
            {
                Debug.Log("Config wasn't set to true. Using original text.");
                return;
            }
            try
            {
                TMPro.TMP_FontAsset font = fonts[0];
                switch (INIConfigure.GetValue(configPath, "text_font"))
                {
                    case "COMIC_SANS_ALTAS":
                        font = fonts[0];
                        break;
                    case "COMIC":
                        font = fonts[1];
                        break;
                    case "ARIAL":
                        font = fonts[2];
                        break;
                    default:
                        font = fonts[0];
                        break;
                }
                fullText.font = font;
                fullText.text = File.ReadAllText(dataPath + "/FullCredits.txt");
                for (int i = 0; i < slideshowTexts.Length; i++)
                {
                    int fileNumber = i + 1;
                    slideshowTexts[i].font = font;
                    slideshowTexts[i].text = File.ReadAllText(dataPath + "/SlideShow/" + fileNumber + ".txt");
                }
            }
            catch (System.Exception e)
            {
                InputManager.DisplayException(new System.Exception("Quitting game because the files cannot be read probably."), 5f);
                Debug.LogException(e);
                Debug.Log("An exception was thrown. Quitting game...");
                ApplicationConfigure.closeApplication();
            }

        }
        catch (System.Exception e)
        {
            InputManager.DisplayException(e, 5f);
            Debug.LogException(e);
            Debug.Log("An exception was thrown. Using original text.");
        }
    }

    public void Quit()
    {
        ApplicationConfigure.closeApplication();
    }
}

using TMPro;
using UnityEngine;


public class StarterTextScript : MonoBehaviour
{
    public TMP_Text text;

    public TMP_Text seedText;

    public TMP_Text type;

    public SceneLoadAwake sceneLoadAwake;

    private void Awake()
    {
        text.text = "---";
    }

    private void Update()
    {
        string mode;
        string ModeSelected = Settings.GetString("ModeSelected");
        if (ModeSelected.StartsWith("School"))
        {
            if (Settings.GetString("CurrentMode") == "story")
            {
                mode = "STORY";
            }
            else if (Settings.GetString("CurrentMode") == "hideAndSeekGame")
            {
                mode = "HIDE &\n SEEK";
            }
            else
            {
                mode = "ENDLESS";
            }
            type.text = Settings.GetString("ModeType");
            seedText.text = (ModeSelected == "School_2") ? "To Grand Opening!" : "To The School!";
            if (type.text == "NULL")
            {
                mode = "NULL";
                seedText.text = "UNDEFINED";
            }
        }
        else if (ModeSelected == "BusCamping")
        {
            mode = "CAMPING";
            type.text = "Normal";
            seedText.text = "To The Camp!";
        }
        else if (ModeSelected == "FarmCamping")
        {
            mode = "FARMING";
            type.text = "Normal";
            if (Settings.GetString("FarmMode") == "Barn")
                seedText.text = "To The Barn!";
            else
                seedText.text = "To The Maze!";
        }
        else if (ModeSelected == "MM_Connect")
        {
            mode = "MULTI\nPLAYER";
            type.text = "Normal";
            seedText.text = "To The School!";
        }
        else
        {
            mode = "-";
            seedText.text = "-";
            type.text = "-";
        }
        text.text = mode;
    }
}

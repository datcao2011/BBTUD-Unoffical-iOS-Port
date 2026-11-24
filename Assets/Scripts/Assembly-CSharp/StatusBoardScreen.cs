using System.Collections;
using TMPro;
using UnityEngine;

public class StatusBoardScreen : MonoBehaviour
{
    [Header("Board Default Statuses")]
    [SerializeField]
    private string status;
    [SerializeField]
    private string fogMachineStatus;
    [SerializeField]
    private string lightStatus;
    [SerializeField]
    private string wallDoorStatus;
    [SerializeField]
    [TextArea]
    private string informationStatus;

    [Header("Script Elements")]
    public TMP_Text statusText;

    public string regularTemplate { get; protected set; }

    // Private Elements
    private const string templateStatus = "<#FFFFFF>Status: {STATUS}\n<#FFFFFF>Fog Machine: {FMSTATUS}\n<#FFFFFF>Lights: {LSTATUS}\n<#FFFFFF>Wall Doors: {WDSTATUS}\n<#FFFFFF>--- Information ---\n{ISTATUS}";

    private void Start()
    {
        ReloadBoard();
        regularTemplate = statusText.text;
    }

    public void ResetStatus()
    {
        status = "<#00ff00>OK";
        fogMachineStatus = "<#00ff00>Ready";
        lightStatus = "<#00ff00>ON";
        wallDoorStatus = "<#00ff00>Lifted";
        informationStatus = "<#00ff00>No issues or problems detected in this school.";
        ReloadBoard();
    }

    private void ReloadBoard()
    {
        string template = templateStatus;

        // Replace text
        template = template.Replace("{STATUS}", status);
        template = template.Replace("{FMSTATUS}", fogMachineStatus);
        template = template.Replace("{LSTATUS}", lightStatus);
        template = template.Replace("{WDSTATUS}", wallDoorStatus);
        template = template.Replace("{ISTATUS}", informationStatus);
        statusText.text = template;
    }

    public void SetStatus(string value, Color textColor)
    {
        status = "<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + value;
        ReloadBoard();
    }

    public void SetFogMachineStatus(string value, Color textColor)
    {
        fogMachineStatus = "<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + value;
        ReloadBoard();
    }

    public void SetLightStatus(string value, Color textColor)
    {
        lightStatus = "<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + value;
        ReloadBoard();
    }

    public void SetWallDoorStatus(string value, Color textColor)
    {
        wallDoorStatus = "<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + value;
        ReloadBoard();
    }

    public void SetInformation(string value, Color textColor)
    {
        informationStatus = "<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + value;
        ReloadBoard();
    }
}

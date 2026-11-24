using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAvatarSettings : MonoBehaviour
{
    public bool ColorPickerIsVisible;
    public CanvasGroup OtherButtons;
    public GameObject ColorPicker;
    public Gradient ColorPickerGradient;
    public TMP_InputField ColorPickerText;
    public TMP_Text TitleText;
    public string[] AvatarColors;
    public string CurrentAvatar;
    public PlayerAvatarApplier Avatar;

    private string Template;

    public Transform ModelRotation;

    private float Senstivity = 15f;

    // Start is called before the first frame update
    private void Start()
    {
        Template = TitleText.text;
        AvatarColors = LoadAvatarColors();
        Avatar.Set(AvatarColors[0], AvatarColors[1], AvatarColors[2], AvatarColors[3], AvatarColors[4]);
        Avatar.SetAccessory(Settings.GetInt("Avatar_Cosmetic", 0));
    }

    // Update is called once per frame
    private void Update()
    {
        ModelRotation.localEulerAngles += new Vector3(0f, MobileTouchScript.XAxis * Senstivity, 0f);
    }

    public void ActivateColorPicker()
    {
        OtherButtons.interactable = false;
        OtherButtons.alpha = 0f;
        ColorPicker.SetActive(true);
        string[] Params = new string[] { "Arms", "Torso", "Legs", "Body", "Hair" };
        int Val = 1;
        for (int i = 0; i < Params.Length; i++)
        {
            if (CurrentAvatar == Params[i])
            {
                Val = i;
                break;
            }
        }
        ColorPickerText.SetTextWithoutNotify(AvatarColors[Val]);
        TitleText.text = Template.Replace("{NAME}", CurrentAvatar);
    }

    public void SetCurrent(string Part)
    {
        CurrentAvatar = Part;
    }

    public void Done()
    {
        if (ColorPicker.activeSelf)
        {
            ColorPicker.SetActive(false);
            OtherButtons.interactable = true;
            OtherButtons.alpha = 1f;
        }
        string[] Params = new string[] { "Arms", "Torso", "Legs", "Body", "Hair" };
        int Val = 1;
        for (int i = 0; i < Params.Length; i++)
        {
            if (CurrentAvatar == Params[i])
            {
                Val = i;
                break;
            }
        }
        AvatarColors[Val] = Avatar.Get()[Val];
    }

    public void UpdateCustomAvatarColor(float Val)
    {
        MeshRenderer[] AvatarParts;
        switch (CurrentAvatar)
        {
            case "Arms":
                AvatarParts = Avatar.Arms;
                break;
            case "Legs":
                AvatarParts = Avatar.Legs;
                break;
            case "Hair":
                AvatarParts = Avatar.Hair;
                break;
            case "Body":
                AvatarParts = Avatar.Body;
                break;
            default:
                AvatarParts = new MeshRenderer[] { Avatar.Torso };
                break;
        }
        Avatar.SetColor(ColorPickerGradient.Evaluate(Val), AvatarParts);
        ColorPickerText.SetTextWithoutNotify('#' + ColorUtility.ToHtmlStringRGB(ColorPickerGradient.Evaluate(Val)));
    }

    public void UpdateCustomAvatarColor(string Val)
    {
        MeshRenderer[] AvatarParts;
        switch (CurrentAvatar)
        {
            case "Arms":
                AvatarParts = Avatar.Arms;
                break;
            case "Legs":
                AvatarParts = Avatar.Legs;
                break;
            case "Hair":
                AvatarParts = Avatar.Hair;
                break;
            case "Body":
                AvatarParts = Avatar.Body;
                break;
            default:
                AvatarParts = new MeshRenderer[] { Avatar.Torso };
                break;
        }
        Color color;
        if (ColorUtility.TryParseHtmlString(Val, out color))
        {
            Avatar.SetColor(color, AvatarParts);
        }
    }

    public void UpdateAccessory(int i)
    {
        Avatar.SetAccessory(i);
    }

    public void SaveColors()
    {
        string[] Params = new string[] { "Avatar_Arms", "Avatar_Torso", "Avatar_Legs", "Avatar_Body", "Avatar_Hair" };
        AvatarColors = Avatar.Get();
        for (int i = 0; i < AvatarColors.Length; i++)
        {
            Settings.SetString(Params[i], AvatarColors[i]);
        }
        Settings.SetInt("Avatar_Cosmetic", Avatar.GetAccessory());
    }

    public string[] LoadAvatarColors()
    {
        return new string[] { Settings.GetString("Avatar_Arms", "#1479C8"), Settings.GetString("Avatar_Torso", "#1479C8"), Settings.GetString("Avatar_Legs", "#16CC2E"),
        Settings.GetString("Avatar_Body", "#FDECA6"), Settings.GetString("Avatar_Hair", "#604222")};
    }
}

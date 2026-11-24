using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SimpleCodeDetails : MonoBehaviour
{
    public Animator CopyAnimator;
    public TMP_Text CodeText;

    private bool IsHidden;

    private string Censored;

    [SerializeField]
    private Texture T_1;
    [SerializeField]
    private Texture T_2;

    public RawImage EyeImage;

    public GameObject[] CoverUps;

    // Start is called before the first frame update
    private void Start()
    {
        Censored = CensorText(PhotonNetwork.CurrentRoom.Name);
        IsHidden = Settings.GetBool("MP_HideText");
        UpdateHideButtonDetails();
    }

    private string CensorText(string Text)
    {
        string Result = "";
        for (int i = 0; i < Text.Length; i++)
        {
            Result += "*";
        }
        return Result;
    }

    private void Update()
    {
        CodeText.text = IsHidden ? Censored : PhotonNetwork.CurrentRoom.Name;
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetCoverup();
        }
    }

    public void ToggleHideButton()
    {
        IsHidden = !IsHidden;
        Settings.SetBool("MP_HideText", IsHidden);
        UpdateHideButtonDetails();
    }

    public void CopyCode()
    {
        GUIUtility.systemCopyBuffer = PhotonNetwork.CurrentRoom.Name;
        CopyAnimator.SetTrigger("Copy");
    }

    private void UpdateHideButtonDetails()
    {
        EyeImage.texture = IsHidden ? T_2 : T_1;
        SetCoverup(IsHidden);
    }

    private bool Coverups;

    private void SetCoverup() => SetCoverup(!Coverups);

    private void SetCoverup(bool Enabled)
    {
        Coverups = Enabled;
        foreach (GameObject Coverup in CoverUps)
        {
            Coverup.SetActive(Enabled);
        }
    }

    private void OnDisable()
    {
        SetCoverup(false);
    }
}

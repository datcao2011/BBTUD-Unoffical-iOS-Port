using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class AccountUI : MonoBehaviour
{
    private Texture accountImage;

    public RawImage profilePicture;
    public Texture DefaultPicture;

    public RawImage accountPicture;
    public TMP_Text accountUsername;

    private void Start()
    {
        try
        {
            if (!AccountManager.hasInstance())
            {
                if (Settings.HasKey("Account"))
                    throw new MissingReferenceException("Account Manager instance has not been created.");
                else new GameObject("Account Manager").AddComponent<AccountManager>();
            }
            Refresh();
        }
        catch (Exception e)
        {
            InputManager.DisplayException(e, 15f);
            gameObject.SetActive(false);
            throw;
        }
    }

    public void CheckLoggedIn()
    {
        if (!Settings.HasKey("Account")) { AccountScript.LOAD_TO_SCENE = "MainMenu"; UnityEngine.SceneManagement.SceneManager.LoadScene("Logon"); }
    }

    public void LogoutAccount()
    {
        AccountManager.LogoutSession();
        profilePicture.texture = DefaultPicture;
        accountPicture.texture = DefaultPicture;
        accountUsername.text = "Logged Out";
    }

    public void Refresh()
    {
        try
        {
            if (!Settings.HasKey("Account"))
            {
                profilePicture.texture = DefaultPicture;
                accountPicture.texture = DefaultPicture;
                accountUsername.text = "Logged Out";
                return;
            }
            AccountManager.GetLatestProfile();
            if (accountImage == null) StartCoroutine(WaitForProfileDownload());
            accountImage = AccountManager.getProfilePicture();
            profilePicture.texture = accountImage;
            accountPicture.texture = accountImage;
            accountUsername.text = AccountManager.getUsername() ?? Settings.GetString("Account");
        }
        catch (System.Exception)
        {
            gameObject.SetActive(false);
            throw;
        }

    }

    private IEnumerator WaitForProfileDownload()
    {
        yield return new WaitWhile(() => AccountManager.RetrievingProfilePicture);
        yield return null;
        accountImage = AccountManager.getProfilePicture();
        profilePicture.texture = accountImage;
        accountPicture.texture = accountImage;
        yield break;
    }

    private string GenerateGibberishString()
    {
        return "LEAK_PROTECTION_GIBBERISH_3mxeA4pflLG2tHwAnaCXCGMhrqaJK5lJlkFuXSU3yegsEEjkelcqal8J1Za9v1EzaXWQTUtkG3TBcwPrCUJF3zzVCGMhrqaJK5lJlkFuXSU3ye"; // Trying to make sure the person not leak their session id.
    }

    public void ChangeProfile()
    {
        Application.OpenURL("https://seenwalex.wixsite.com/site/bbtud-community/manage/profile?lite=1&a=" + GenerateGibberishString() + "&sl=" + System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(AccountManager.getSessionId())));
    }

    public void LinkAccount()
    {
        Application.OpenURL("https://seenwalex.wixsite.com/site/bbtud-community/linked-accounts?lite=1&a=" + GenerateGibberishString() + "&sl=" + System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(AccountManager.getSessionId())));
    }
}

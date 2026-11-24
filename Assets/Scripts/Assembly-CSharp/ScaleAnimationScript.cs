using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class ScaleAnimationScript : MonoBehaviour
{
    float f;
    float g;
    bool b;
#if UNITY_EDITOR
    [SerializeField]
#endif
    float r;
#if UNITY_EDITOR
    [SerializeField]
#endif
    float t;

    public Image icon;
    private void Start()
    {
        Settings.OnSettingChanged += OnValChange;
        if (!PhotonNetwork.IsConnected) { Destroy(gameObject); return; }
        b = true;
        OnValChange("ShowBadConnection", Settings.GetString("ShowBadConnection", "true"));
    }

    private void OnValChange(string Key, object Value)
    {
        if (Key == "ShowBadConnection")
        {
            if ((string)Value == "true")
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        Settings.OnSettingChanged -= OnValChange;
    }

    private void Update()
    {
        if (icon.isActiveAndEnabled)
        {
            f += Time.unscaledDeltaTime * r * (b ? 1f : -1f);
            if (f > 1.5f) { b = false; } else if (f <= 1f) { b = true; }
            transform.localScale = new Vector3(f, f, 0f);
        }

        if (!PhotonNetwork.InRoom && PhotonNetwork.IsConnected)
        {
            if (g < 5f) g += Time.unscaledDeltaTime;
            else
                PhotonNetwork.Disconnect(); // Timeout because you're not supposed to be connected.
        }

        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            icon.gameObject.SetActive(true);
            icon.color = Color.red;

            r = PhotonNetwork.InRoom ? 5f : 7f;
            return;
        }
        int ping = PhotonNetwork.GetPing();
        if (ping >= 175)
        {
            t = (float)ping;
        }
        else if (t >= 0f) t -= Time.unscaledDeltaTime * 125f;
        icon.gameObject.SetActive(t >= 0f);
        if (t >= 1000f)
        {
            Color color = Color.red;
            ColorUtility.TryParseHtmlString("#ff0000", out color);
            color.a = 0.7f;
            icon.color = color;
            r = 4f;
        }
        else if (t >= 600f)
        {
            Color color = Color.yellow;
            ColorUtility.TryParseHtmlString("#db230b", out color);
            color.a = 0.7f;
            icon.color = color;
            r = 3.5f;
        }
        else if (t >= 250f)
        {
            Color color = Color.yellow;
            ColorUtility.TryParseHtmlString("#db990b", out color);
            color.a = 0.7f;
            icon.color = color;
            r = 2f;
        }
        else if (t >= 170f)
        {
            Color color = Color.yellow;
            color.a = 0.7f;
            icon.color = color;
            r = 1f;
        }
        else if (t >= 0f)
        {
            Color color = Color.white;
            color.a = 0.7f;
            icon.color = color;
            r = 0.75f;
        }
    }
}
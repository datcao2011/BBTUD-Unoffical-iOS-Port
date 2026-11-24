using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public GameObject message;
    public TMPro.TMP_Text text;

    private void Start()
    {
        StartCoroutine(GetAlert());
    }

    private IEnumerator GetAlert()
    {
        UnityWebRequest request = UnityWebRequest.Get(InputManager.siteRequestUrl + "alertMessage");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            console.error("Unable to receive alert message: " + request.error);
            yield break;
        }
        string message = request.downloadHandler.text;
        if (message != "")
        {
            this.message.SetActive(true);
            text.text = message;
        }
    }
}

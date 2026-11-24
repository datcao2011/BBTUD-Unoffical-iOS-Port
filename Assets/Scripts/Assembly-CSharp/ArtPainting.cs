using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class ArtPainting : MonoBehaviour
{
    public RawImage painting;

    private string path { get { return Application.persistentDataPath + "/art_image.png"; } }

    private void Start()
    {
        StartCoroutine(getImage());
        ReloadPaintingFromSource();
    }

    private class HTTPResponse
    {
        public bool success;
        public int errorCode;
        public string error;
        public string imageurl;
        public int s = 0;
    }

    private IEnumerator getImage()
    {
        UnityWebRequest request = UnityWebRequest.Get(InputManager.siteRequestUrl + "elevatorArtImage");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            console.error("Receiving paint error: " + request.error);
            yield break;
        }
        HTTPResponse response;
        try
        {
            response = JsonUtility.FromJson<HTTPResponse>(request.downloadHandler.text);
            if (response == null) throw new System.Exception("Non-JSON response");
        }
        catch (System.Exception ex)
        {
            response = new HTTPResponse
            {
                error = "INVALID JSON RESPONSE: " + ex.Message,
                errorCode = 99,
                success = false
            };
        }

        if (!response.success)
        {
            console.error("Receiving paint error: (Error Code " + response.errorCode + ") " + response.error);
            yield break;
        }
        UnityWebRequest request2 = UnityWebRequestTexture.GetTexture(response.imageurl);
        yield return request2.SendWebRequest();
        if (request2.result == UnityWebRequest.Result.ConnectionError)
        {
            console.error("Receiving paint error: " + request2.error);
            yield break;
        }
        if (!string.IsNullOrEmpty(request2.error))
        {
            console.error(request2.error);
        }

#if !UNITY_WEBGL
        File.WriteAllBytes(path, ((DownloadHandlerTexture)request2.downloadHandler).texture.EncodeToPNG());
#else
        _texture = ((DownloadHandlerTexture)request2.downloadHandler).texture;
#endif
        ReloadPaintingFromSource();
        yield break;
    }

#if UNITY_WEBGL || UNITY_EDITOR
    public Texture _texture;
#endif

    private void ReloadPaintingFromSource()
    {
#if UNITY_WEBGL
        if (_texture == null) return;
        Texture2D text = (Texture2D)_texture;
        text.filterMode = FilterMode.Point;
        painting.texture = text;
#else
        if (!File.Exists(path)) return;
        Texture2D texture;
        texture = new Texture2D(painting.texture.width, painting.texture.height, TextureFormat.RGBA32, false);
        texture.LoadImage(File.ReadAllBytes(path));
        texture.filterMode = FilterMode.Point;
        painting.texture = texture;
#endif
    }
}

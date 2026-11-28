using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public static class LanDetector
{
    private static string _lanKey;
    private static bool _resolving;

    public static string LanKey
    {
        get { return _lanKey; }
    }

    public static IEnumerator Ensure()
    {
        if (!string.IsNullOrEmpty(_lanKey) || _resolving)
            yield break;
        _resolving = true;
        string local = LocalPrivateIpv4();
        string pub = null;
        UnityWebRequest req = UnityWebRequest.Get("https://api.ipify.org?format=json");
        req.timeout = 5;
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(req.downloadHandler.text))
            pub = ExtractIp(req.downloadHandler.text);
        _resolving = false;
        if (!string.IsNullOrEmpty(pub) && !string.IsNullOrEmpty(local))
            _lanKey = pub + "/" + Subnet24(local);
    }

    private static string ExtractIp(string s)
    {
        Match m = Regex.Match(s, @"\d{1,3}(\.\d{1,3}){3}");
        return m.Success ? m.Value : null;
    }

    private static string LocalPrivateIpv4()
    {
        try
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            for (int i = 0; i < addresses.Length; i++)
            {
                if (addresses[i].AddressFamily == AddressFamily.InterNetwork && IsPrivate(addresses[i]))
                    return addresses[i].ToString();
            }
        }
        catch { }
        return null;
    }

    private static bool IsPrivate(IPAddress ip)
    {
        byte[] b = ip.GetAddressBytes();
        if (b.Length != 4) return false;
        if (b[0] == 10) return true;
        if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
        if (b[0] == 192 && b[1] == 168) return true;
        return false;
    }

    private static string Subnet24(string ip)
    {
        string[] parts = ip.Split('.');
        return parts.Length == 4 ? parts[0] + "." + parts[1] + "." + parts[2] : ip;
    }

    public static bool SameLan(string a, string b)
    {
        return !string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b) && a == b;
    }
}
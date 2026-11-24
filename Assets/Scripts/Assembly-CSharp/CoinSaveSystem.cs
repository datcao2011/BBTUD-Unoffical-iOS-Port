using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;

public static class CoinSaveSystem
{
    public static void Save(int coins, bool syncOnline = true)
    {
        Settings.SetInt("coins", coins);
        if (Application.internetReachability != NetworkReachability.NotReachable && Settings.HasKey("AccountSession") && syncOnline)
        {
            AccountManager.SetCoins(coins);
        }
    }

    public static void SaveViaShop(int coins, System.Action OnCompleted)
    {
        Save(coins, false);

        if (Application.internetReachability != NetworkReachability.NotReachable && Settings.HasKey("AccountSession"))
        {
            AccountManager.SetCoins(coins, OnCompleted);
        }
        else
        {
            OnCompleted();
        }
    }

    [System.Serializable]
    private class SyncCoin
    {
        public int coins;
        public string sessionId;
    }

    public static int Load()
    {
        // Storage thing is obsolete, so we move it over to the PlayerPrefs
#if !UNITY_WEBGL
        try
        {
            if (Directory.Exists(Application.persistentDataPath + "/Data"))
            {
                string path = Application.persistentDataPath + "/Data/CoinData.bin";
                if (File.Exists(path))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(path, FileMode.Open);

                    CoinData data = formatter.Deserialize(stream) as CoinData;
                    Settings.SetInt("coins", data.coins);
                    stream.Close();

                    File.Delete(path);
                    Debug.LogWarning("CoinData.bin | " + path + " | is obsolete, moving it over to PlayerPrefs and deleting file");
                }
                Debug.Log("Data folder | " + Application.persistentDataPath + "/Data" + " | is obsolete, deleting");
                Directory.Delete(Application.persistentDataPath + "/Data");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError("Skipping coin process");
        }

#endif
        return Settings.GetInt("coins", 500);
    }
}
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;
using SchoolSaves;

public static class SaveSystem
{
    public static readonly string directoryPath = UnityEngine.Application.persistentDataPath + "/Saves/";

    public static SchoolSaves.PlayerData SavePlayer(GameControllerScript gc, string FileName = "g", string title = "d")
    {
        // Handle path
        BinaryFormatter formatter = new BinaryFormatter();
        string path = directoryPath + FileName + ".sav";
        if (!Directory.Exists(UnityEngine.Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/Saves");
        }

        FileStream stream = new FileStream(path, FileMode.Create);

        SchoolSaves.PlayerData data = new SchoolSaves.PlayerData(gc, title);

        formatter.Serialize(stream, data);

        stream.Close();
        return data;
    }

    public static void DeletePlayer(string fileName)
    {
        string path = UnityEngine.Application.persistentDataPath + "/Saves/" + fileName;
        if (!Directory.Exists(UnityEngine.Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/Saves");
        }
        if (!File.Exists(path))
        {
            console.warn("[Save System] Save was already deleted. The DeletePlayer was called on an already deleted file.");
            return;
        }

        File.Delete(path);
    }

    public static SchoolSaves.PlayerData LoadPlayer(string fileName)
    {
        string path = UnityEngine.Application.persistentDataPath + "/Saves/" + fileName;
        if (!File.Exists(path))
        {
            UnityEngine.PlayerPrefs.DeleteKey("Save");
            GameSystem.AbortApplication("File can't be loaded because . Missing file path: " + path, "SAVE_LOAD_MISSING");
            throw new FileNotFoundException("The save file has been deleted, or missing.");   
        }

        BinaryFormatter formatter = new BinaryFormatter();
        if (!Directory.Exists(UnityEngine.Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/Saves");
        }
        FileStream stream = null;
        SchoolSaves.PlayerData data = null;

        try
        {
            stream = new FileStream(path, FileMode.Open);
            if (path.EndsWith(".dat") || path.EndsWith(".sdat"))
            {
                data = Utils.ConvertOldToNew(formatter.Deserialize(stream) as PlayerData);
            }
            else
            {
                data = formatter.Deserialize(stream) as SchoolSaves.PlayerData;
            }
        }
        catch (System.Exception)
        {
            UnityEngine.PlayerPrefs.DeleteKey("Save");
            GameSystem.AbortApplication("The save file is corrupted. Save file path: " + path, "SAVE_LOAD_CORRUPTED");
            stream.Close();
            throw;
        }

        stream.Close();

        return data;
    }
}
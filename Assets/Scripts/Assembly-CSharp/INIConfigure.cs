using System.IO;

public class INIConfigure
{
    public static string GetValue(string path, string key)
    {
        if (!File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path);
            writer.Write("");
            writer.Close();
        }
        StreamReader reader = new StreamReader(path);
        int i = 0;
        int max = File.ReadAllLines(path).Length - 1;
        string result = "";
        bool tf = true;
        while (tf)
        {
            string str1 = reader.ReadLine();
            if (str1 != null)
                if (str1.Split('=')[0] == key)
                {
                    result = str1.Split('=')[1];
                    tf = false;
                }
            if (i >= max) tf = false;
            i++;
        }
        reader.Close();
        return result;
    }

    public static bool INI_Exists(string path)
    {
        return File.Exists(path) && path.EndsWith(".ini");
    }

    public static void SetValue(string path, string key, string value)
    {
        StreamReader reader = new StreamReader(path);
        int i = 0;
        int max = File.ReadAllLines(path).Length - 1;
        bool tf = true;
        bool finded = false;
        while (tf)
        {
            string str1 = reader.ReadLine();
            if (str1 != null)
                if (str1.Split('=')[0] == key)
                {
                    reader.Close();
                    reader = new StreamReader(path);
                    string text = "";
                    int l = 1;
                    while (l != max)
                    {
                        if (text != "")
                            text += "\n";
                        if (l != i + 1)
                        {
                            text += reader.ReadLine();
                        }
                        else
                        {
                            text += str1.Split('=')[0] + '=' + value + "\n";
                            reader.ReadLine();
                        }
                        l++;
                    }
                    reader.Close();
                    StreamWriter writer = new StreamWriter(path, false);
                    writer.WriteLine(text);
                    writer.Close();
                    tf = false;
                    finded = true;
                }
            if (i >= max) tf = false;
            i++;
        }
        if (!finded)
        {
            reader.Close();
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(key + '=' + value);
            writer.Close();
        }
    }
}
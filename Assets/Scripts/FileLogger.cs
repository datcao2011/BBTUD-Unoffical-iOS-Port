using UnityEngine;
using System;
using System.IO;

public static class FileLogger
{
    private static string _path;
    private static readonly object _lock = new object();
    private static long _size;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        try
        {
            _path = Path.Combine(Application.persistentDataPath, "bbtud_log.txt");
            string dir = Path.GetDirectoryName(_path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(_path,
                "=== BBTUD log start " + DateTime.Now + " ===\n" +
                "platform=" + Application.platform +
                " version=" + Application.version +
                " device=" + SystemInfo.deviceModel +
                " gfx=" + SystemInfo.graphicsDeviceName + "\n");
            _size = new FileInfo(_path).Length;
            Application.logMessageReceived += Handle;
            Debug.Log("[FileLogger] active at " + _path);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private static void Handle(string msg, string stack, LogType type)
    {
        try
        {
            lock (_lock)
            {
                if (_size > 2000000L)
                {
                    File.WriteAllText(_path, "=== log truncated ===\n");
                    _size = 0;
                }
                string line = DateTime.Now.ToString("HH:mm:ss.fff") + " [" + type + "] " + msg + "\n";
                if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert)
                    line += stack + "\n";
                File.AppendAllText(_path, line);
                _size += line.Length;
            }
        }
        catch { }
    }
}
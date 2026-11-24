using UnityEngine;
using System.Collections.Generic;

public static class Settings
{
    public static Dictionary<string, object> SavedKeys = new Dictionary<string, object>();

    public static void Clear()
    {
        SavedKeys.Clear();
    }

    private static T GetKey<T>(string name, T def, int type)
    {
        if (SavedKeys.ContainsKey(name))
        {
            if (typeof(T) == typeof(float))
            {
                return (T)(object)PlayerPrefs.GetFloat(name);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)PlayerPrefs.GetInt(name);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)PlayerPrefs.GetString(name);
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)(object)PlayerPrefsPlus.GetBool(name);
            }
            else if (SavedKeys[name].GetType() == typeof(T)) return (T)SavedKeys[name];
            else return def ?? default(T);
        }
        else if (PlayerPrefs.HasKey(name))
        {
            switch (type)
            {
                case 0:
                    SavedKeys.Add(name, PlayerPrefs.GetFloat(name));
                    return (T)(object)PlayerPrefs.GetFloat(name);
                case 1:
                    SavedKeys.Add(name, PlayerPrefs.GetInt(name));
                    return (T)(object)PlayerPrefs.GetInt(name);
                case 2:
                    SavedKeys.Add(name, PlayerPrefs.GetString(name));
                    return (T)(object)PlayerPrefs.GetString(name);
                case 3:
                    SavedKeys.Add(name, PlayerPrefsPlus.GetBool(name));
                    return (T)(object)PlayerPrefsPlus.GetBool(name);
            }
            return (T)SavedKeys[name];
        }
        return def ?? default(T);
    }

    public static event SettingChangeHandler OnSettingChanged;
    public delegate void SettingChangeHandler(string value, object val);

    private static void SetKey(string name, object obj, int type)
    {
        if (SavedKeys.ContainsKey(name))
        {
            if (SavedKeys[name] != obj)
            {
                OnSettingChanged?.Invoke(name, obj);
            }
            SavedKeys[name] = obj;
        }
        else
        {
            SavedKeys.Add(name, obj);
            OnSettingChanged?.Invoke(name, obj);
        }
        switch (type)
        {
            case 0:
                PlayerPrefs.SetFloat(name, (float)obj);
                break;
            case 1:
                PlayerPrefs.SetInt(name, (int)obj);
                break;
            case 2:
                PlayerPrefs.SetString(name, (string)obj);
                break;
            case 3:
                PlayerPrefsPlus.SetBool(name, (bool)obj);
                break;
        }
    }

    public static bool HasKey(string name)
    {
        return SavedKeys.ContainsKey(name) ? true : PlayerPrefs.HasKey(name);
    }

    public static void DeleteKey(string name)
    {
        PlayerPrefs.DeleteKey(name);
        if (SavedKeys.ContainsKey(name)) SavedKeys.Remove(name);
    }

    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return GetKey<float>(key, defaultValue, 0);
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return GetKey<int>(key, defaultValue, 1);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return GetKey<string>(key, defaultValue, 2);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return GetKey<bool>(key, defaultValue, 3);
    }

    public static void SetFloat(string key, float value)
    {
        SetKey(key, value, 0);
    }

    public static void SetInt(string key, int value)
    {
        SetKey(key, value, 1);
    }

    public static void SetString(string key, string value)
    {
        SetKey(key, value, 2);
    }

    public static void SetBool(string key, bool value)
    {
        SetKey(key, value, 3);
    }
}
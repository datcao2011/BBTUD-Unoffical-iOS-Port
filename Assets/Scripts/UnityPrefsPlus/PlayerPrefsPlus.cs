using UnityEngine;

/// <summary>
/// Stores and accesses player preferences between game sessions with more features.
/// </summary>
public static class PlayerPrefsPlus {
    /// <summary>
    /// Sets the value of the preference identified by key.
    /// </summary>
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists.
    /// </summary>
    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1 ? true : false;
    }

    /// <summary>
    /// Returns the value corresponding to key in the preference file if it exists.
    /// </summary>
    public static bool GetBool(string key, bool defaultValue)
    {
        return PlayerPrefs.HasKey(key) ? Settings.GetBool(key) : defaultValue;
    }
}
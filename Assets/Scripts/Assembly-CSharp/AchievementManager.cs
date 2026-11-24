using UnityEngine;
using System.Collections;
using TMPro;
using System;
using UnityEngine.UI;
using GameJolt.BBTUD;

public class AchievementManager : MonoBehaviour
{
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (InputManager.usingTouch)
        {
            achievements[2].RareType = AchievementType.Uncommon;
        }
        if (GameJoltManager.Instance != null)
        {
            GameJoltManager.Achievements((SimpleJSON.JSONNode json) =>
            {
                if (json == null)
                {
                    #if DEBUG
                    Debug.Log("null");
                    #endif
                    return;
                }
                if (json["success"].IsString && json["success"] == "true" && json["trophies"].IsArray)
                {
                    System.Collections.Generic.List<bool> HasAchieved = new System.Collections.Generic.List<bool>();
                    foreach (Achievement ach in achievements)
                    {
                        HasAchieved.Add(false);
                    }
                    foreach (SimpleJSON.JSONNode trophy in json["trophies"].AsArray)
                    {
                        for (int i = 0; i < achievements.Length; i++)
                        {
                            if (achievements[i].TrophyId == trophy["id"].ToString().Replace("\"", ""))
                            {
                                Earn(i, false);
                                HasAchieved[i] = true;
                                continue;
                            }
                        }
                    }
                    for (int j = 0; j < HasAchieved.Count; j++)
                    {
                        if (!HasAchieved[j])
                        {
                            RemoveEarned(j);
                        }
                    }
                }
            });
        }
    }

    public static AchievementManager Instance;

    public Achievement[] achievements;

    [SerializeField]
    private TMP_Text achievementText;

    [SerializeField]
    private TMP_Text achievementTypeText;

    public TMP_Text achievementDesc;

    public TMP_Text coinsText;

    public RawImage achievementImage;

    public Animator anim;

    public AudioClip winClip;

    private void AddCoins(int coins)
    {
        try
        {
            int totalCoins = CoinSaveSystem.Load() + coins;
            CoinSaveSystem.Save(totalCoins, true);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            InputManager.DisplayException(ex, 15f);
        }

    }

    private IEnumerator waitForAchievement()
    {
        while (anim.GetCurrentAnimatorStateInfo(0).IsTag("playing"))
        {
            yield return null;
        }
    }

    private IEnumerator EarnAchievement(int achievementIndex, bool ReceiveCoins)
    {
        Achievement achievement = achievements[achievementIndex];
        AchievementType rareType = achievement.RareType;
        achievementImage.texture = achievement.image;
        achievementText.text = achievement.name;
        achievementDesc.text = achievement.description;
        achievementTypeText.text = rareType.ToString().ToUpper();
        achievementTypeText.color = GetAchievementColorType(rareType);
        anim.SetTrigger("trigger");
        if (ReceiveCoins)
        {
            switch (rareType)
            {
                case AchievementType.Common:
                    coinsText.text = "+50 Coins";
                    AddCoins(50);
                    break;
                case AchievementType.Uncommon:
                    coinsText.text = "+100 Coins";
                    AddCoins(100);
                    break;
                case AchievementType.Rare:
                    coinsText.text = "+200 Coins";
                    AddCoins(200);
                    break;
                case AchievementType.Epic:
                    coinsText.text = "+350 Coins";
                    AddCoins(350);
                    break;
                case AchievementType.Legendary:
                    coinsText.text = "+500 Coins";
                    AddCoins(500);
                    Settings.SetInt("ItemsCollected", Settings.GetInt("ItemsCollected") + 1);
                    Settings.SetInt("ItemCollected" + (Settings.GetInt("ItemsCollected", 0) - 1).ToString(), 12);
                    break;
                default:
                    console.error("Unknown type of achievement?");
                    break;
            }
        }
        else
        {
            coinsText.text = "+0 Coins";
        }
        yield break;
    }

    public Color GetAchievementColorType(AchievementType rareType)
    {
        if (rareType == AchievementType.Common)
        {
            return Color.gray;
        }
        else if (rareType == AchievementType.Uncommon)
        {
            return new Color(0.90980f, 0.90980f, 0.90980f);
        }
        else if (rareType == AchievementType.Rare)
        {
            return Color.cyan;
        }
        else if (rareType == AchievementType.Epic)
        {
            return Color.magenta;
        }
        else if (rareType == AchievementType.Legendary)
        {
            return Color.yellow;
        }
        console.error("Achievement type has not been given.");
        return new Color(0f, 0f, 0f, 0f);
    }

    public bool HasEarned(int index)
    {
        Achievement achievement = achievements[index];
        return Settings.GetBool("Achieve_" + achievement.name.Replace(" ", "").Replace("?", "").Replace(".", "").Replace("!", ""));
    }

    public void RemoveEarned(int index)
    {
        Achievement achievement = achievements[index];
        if (Settings.HasKey("Achieve_" + achievement.name.Replace(" ", "").Replace("?", "").Replace(".", "").Replace("!", ""))) Settings.DeleteKey("Achieve_" + achievement.name.Replace(" ", "").Replace("?", "").Replace(".", "").Replace("!", ""));
    }

    public void Earn(Achievements TypeAchievement, bool ReceiveCoins = true) => Earn((int)TypeAchievement, ReceiveCoins);

    public void Earn(int achievementIndex, bool ReceiveCoins = true)
    {
        if (HasEarned(achievementIndex))
        {
            return;
        }
        Achievement achievement = achievements[achievementIndex];
        Settings.SetBool("Achieve_" + achievement.name.Replace(" ", "").Replace("?", "").Replace(".", "").Replace("!", ""), true);
        if (ReceiveCoins)
        {
            GameObject audioDeviceObject = new GameObject();
            audioDeviceObject.name = "Achievement Earned Sound";
            AudioSource audioDevice = audioDeviceObject.AddComponent<AudioSource>();
            audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
            audioDevice.clip = winClip;
            audioDevice.Play();
            UnityEngine.Object.DontDestroyOnLoad(audioDeviceObject);
            UnityEngine.Object.Destroy(audioDeviceObject, winClip.length);
            if (GameJoltManager.Instance != null) GameJoltManager.Achieve(achievement.TrophyId);
        }
        StartCoroutine(EarnAchievement(achievementIndex, ReceiveCoins));
    }

    [Serializable]
    public class Achievement
    {
        public string name;
        public Texture image;
        [TextArea]
        public string description;
        public AchievementType RareType;
        public bool Secret;
        public string TrophyId;
    }

    public enum AchievementType
    {
        None,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
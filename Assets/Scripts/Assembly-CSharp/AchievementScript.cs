using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class AchievementScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] achievements;

    public GameObject template;

    public ScrollRect scrollRect;

    public Texture SecretTexture;

    private void Start()
    {
        GetAchievements();
    }

    private void GetAchievements()
    {
        for (int i = 0; i < AchievementManager.Instance.achievements.Length; i++)
        {
            AchievementManager.Achievement achievement = AchievementManager.Instance.achievements[i];
            GameObject gameObject = Instantiate(template);
            var regularPosition = gameObject.GetComponent<RectTransform>().rect;
            gameObject.transform.SetParent(scrollRect.content, true);
            regularPosition.size = new Vector2(430f, 130f);
            Transform t = gameObject.transform;
            t.GetChild(2).GetComponent<TMP_Text>().text = achievement.name;
            if (achievement.Secret && !AchievementManager.Instance.HasEarned(i))
            {
                t.GetChild(1).GetComponent<RawImage>().texture = SecretTexture;
                t.GetChild(3).GetComponent<TMP_Text>().text = "? ? ?";
            }
            else
            {
                t.GetChild(1).GetComponent<RawImage>().texture = achievement.image;
                t.GetChild(3).GetComponent<TMP_Text>().text = achievement.description;
            }
            t.GetChild(4).GetComponent<TMP_Text>().text = achievement.RareType.ToString();
            t.GetChild(4).GetComponent<TMP_Text>().color = AchievementManager.Instance.GetAchievementColorType(achievement.RareType);
            t.localScale = new Vector2(1f, 1f);
            if (AchievementManager.Instance.HasEarned(i))
            {
                t.GetChild(0).GetComponent<RawImage>().color = Color.white;
                t.GetChild(5).GetComponent<TMP_Text>().color = Color.green;
                t.GetChild(5).GetComponent<TMP_Text>().text = "EARNED";
            }
        }
        Destroy(template);
    }
}
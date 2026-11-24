using UnityEngine;
using TMPro;


public class FarmScript : MonoBehaviour
{
    public TMP_Text time;

    public float timeInFarm;

    public FarmFlagScript flagScript;

    private bool dotdotdot;

    public GameObject reaper;

    public GameObject baldi;

    private void Start()
    {
    }

    private void Update()
    {
        if (!flagScript.winned)
            timeInFarm += Time.deltaTime;

        if (timeInFarm >= 0.5f)
        {
            time.text = "Time: " + Mathf.RoundToInt(timeInFarm);
        }
        if (timeInFarm >= 250f)
        {
            ILIKECORN();
        }
    }

    private void ILIKECORN()
    {
        if (!dotdotdot)
        {
            dotdotdot = true;
            baldi.SetActive(true);
            reaper.SetActive(true);
            AchievementManager.Instance.Earn(Achievements.MazeTimeout);
        }
    }
}
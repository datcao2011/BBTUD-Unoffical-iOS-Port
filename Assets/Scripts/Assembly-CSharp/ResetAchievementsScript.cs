using UnityEngine;
using UnityEngine.UI;

public class ResetAchievementsScript : MonoBehaviour
{
    private Button button;

    private float delay;

    private bool delayDone;

    public GameObject screen;

    public GameObject loadStatus;

    private bool startDelay;

    private void Start()
    {
        button = base.GetComponent<Button>();
        button.onClick.AddListener(Reset);
        delay = 8.2f;
    }

    private void Update()
    {
        if (startDelay)
        {
            delay -= Time.deltaTime;
        }
        if (delay <= 0f & !delayDone)
        {
            delayDone = true;
            screen.SetActive(false);
            loadStatus.SetActive(false);
            startDelay = false;
        }
    }

    private void Reset()
    {
        delay = 8.2f;
        startDelay = true;
        delayDone = false;
        loadStatus.SetActive(true);
        Settings.SetString("threeamAchieve", "NotEarned");
    }
}
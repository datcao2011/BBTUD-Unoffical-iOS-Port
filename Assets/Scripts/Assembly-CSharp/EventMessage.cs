using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class EventMessage : MonoBehaviour
{
    public float disappearMessageTimer = Mathf.Infinity;
    private float InfiniteTimer;
    private float Progress = Mathf.Infinity;
    private float DurationTimer;
    private float destroyTimer = 2f;
    public bool IsDestroying { get; private set; } = false;
    public string Message;
    public string Button = "OK";
    public System.Action<EventMessage> ButtonAction = null;
    private Slider slider;
    private TMP_Text TimerText;
    private bool INF_Forward = true;
    public bool StartTimer = false;

    public void SetProgress(float progress)
    {
        this.Progress = progress;
        slider.direction = Slider.Direction.LeftToRight;
        slider.normalizedValue = Progress;
    }

    public void SetText(string text)
    {
        Transform obj = transform.GetChild(0);
        obj.GetChild(0).GetComponent<TMP_Text>().text = text;
    }

    public void StartLogMessage()
    {
        Transform obj = transform.GetChild(0);
        obj.GetChild(0).GetComponent<TMP_Text>().text = Message ?? "No message was provided.";
        slider = obj.GetChild(1).GetComponent<Slider>();
        TimerText = obj.GetChild(2).GetComponent<TMP_Text>();
        TimerText.gameObject.SetActive(disappearMessageTimer >= 45);
        slider.maxValue = !float.IsInfinity(disappearMessageTimer) ? disappearMessageTimer : 1f;
        if (ButtonAction != null)
        {
            obj.GetChild(3).GetComponentInChildren<TMP_Text>().text = Button;
            obj.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { ButtonAction(this); });
        }
        else
        {
            obj.GetChild(3).gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (IsDestroying)
        {
            destroyTimer -= Time.unscaledDeltaTime;
            if (destroyTimer <= 0f) Destroy(gameObject, 0f);
            return;
        }
        if (!StartTimer) return;
        if (float.IsInfinity(disappearMessageTimer))
        {
            DurationTimer += Time.unscaledDeltaTime;
            TimerText.gameObject.SetActive(DurationTimer >= 10f);
            TimerText.text = "Busy for " + GetTimerText(DurationTimer);
            if (float.IsInfinity(Progress))
            {
                slider.direction = INF_Forward ? Slider.Direction.LeftToRight : Slider.Direction.RightToLeft;
                if (INF_Forward)
                {
                    InfiniteTimer -= Time.unscaledDeltaTime;
                    if (InfiniteTimer <= 0f)
                    {
                        INF_Forward = false;
                    }
                }
                else
                {
                    InfiniteTimer += Time.unscaledDeltaTime;
                    if (InfiniteTimer >= 1f)
                    {
                        INF_Forward = true;
                    }
                }
                slider.value = InfiniteTimer;
            }
        }
        else
        {
            disappearMessageTimer -= Time.unscaledDeltaTime;
            if (disappearMessageTimer <= 0f)
            {
                Disappear();
            }
            else
            {
                slider.value = disappearMessageTimer;
                TimerText.text = GetTimerText(disappearMessageTimer);
            }
        }
    }

    public void Disappear()
    {
        IsDestroying = true;
        GetComponent<Animator>().SetTrigger("disappear");
    }

    private string GetTimerText(float timer)
    {
        if (timer <= 60f) return timer.ToString("0") + "s";
        System.TimeSpan t = System.TimeSpan.FromSeconds(timer);
        if (timer >= 60f) return t.Minutes + ":" + t.Seconds.ToString("00");
        return t.TotalHours.ToString("0") + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
    }
}

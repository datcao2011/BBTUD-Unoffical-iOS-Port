using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class MiniEventMessage : MonoBehaviour
{
    public float disappearMessageTimer = Mathf.Infinity;
    private float destroyTimer = 2f;
    public bool IsDestroying { get; private set; } = false;
    public string Message;
    public Slider slider;
    public bool StartTimer = false;
    public RectTransform This;
    public TMP_Text MessageText;
    public RectTransform MessageTextRect;
    public Animator Anim;

    public void StartLogMessage()
    {
        MessageText.text = Message ?? "No message was provided.";
        slider.maxValue = disappearMessageTimer;
    }

    private void Start() {
        StartLogMessage();
        StartTimer = true;
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
        disappearMessageTimer -= Time.unscaledDeltaTime;
        if (disappearMessageTimer <= 0f)
        {
            Disappear();
        }
        else
        {
            slider.value = disappearMessageTimer;
            Vector2 SD = This.sizeDelta;
            SD.x = MessageTextRect.sizeDelta.x + 11f;
            This.sizeDelta = SD;
        }
    }

    public void Disappear()
    {
        IsDestroying = true;
        Anim.SetTrigger("disappear");
    }
}

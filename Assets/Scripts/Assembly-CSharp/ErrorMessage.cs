using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ErrorMessage : MonoBehaviour
{
    public float disappearMessageTimer;

    public string errorMessage;

    public string fullErrorMessageTrace;

    public bool countdownTimer;

    public TMP_Text stackText;

    public void StartLogMessage()
    {
        if (errorMessage.Length >= 150)
        {
            errorMessage = errorMessage.Substring(0, 150) + "...";
        }
        transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = errorMessage;
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(gameObject, 0f);
        });
        transform.GetChild(0).GetChild(1).GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            bool resumeCountDownTimer = countdownTimer ? true : false;
            countdownTimer = false;
            stackText.text = fullErrorMessageTrace;
            stackText.transform.parent.gameObject.SetActive(true);
            stackText.transform.parent.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                countdownTimer = resumeCountDownTimer;
                stackText.transform.parent.gameObject.SetActive(false);
            });
        });
    }

    private void Update()
    {
        if (countdownTimer)
        {
            disappearMessageTimer -= Time.unscaledDeltaTime;
            if (disappearMessageTimer <= 0f)
            {
                Destroy(gameObject, 0f);
            }
        }
    }
}

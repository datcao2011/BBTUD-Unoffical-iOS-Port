using UnityEngine;
using System;
using TMPro;

public class GoalScript : MonoBehaviour
{
    public static DateTime Goal;

    private TMP_Text Text;

    // Start is called before the first frame update
    private void Start()
    {
        Text = GetComponent<TMP_Text>();
        CalculateGoal();
    }

    public void CalculateGoal()
    {
        DateTime Now = DateTime.Now;
        Goal = DateTime.Now.AddMinutes(-Now.Minute).AddSeconds(-Now.Second).AddMilliseconds(-Now.Millisecond);
        while (Now > Goal.AddMinutes(-1))
        {
            Goal = Goal.AddMinutes(5);
        }
        Text.text = Goal.ToShortTimeString();
    }

#if DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Goal = DateTime.Now;
        }
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class TimeScript : MonoBehaviour
{
    private TMP_Text Text;

    private bool CueFinished;
    public Cinematic CinematicSystem;

    [SerializeField]
    private GoalScript Goal;

    [SerializeField]
    private SwingingDoorScript SwingingDoor;

    private bool HasOpened;

    // Start is called before the first frame update
    private void Start()
    {
        Text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!HasOpened && SwingingDoor.outside.sharedMaterial == SwingingDoor.open)
        {
            HasOpened = true;
        }
        if (Time.timeScale != 0f)
        {
            Text.text = DateTime.Now.ToLongTimeString();
            if (DateTime.Now >= GoalScript.Goal)
            {
                if (HasOpened)
                {
                    CinematicSystem.gameObject.SetActive(true);
                }
                else
                {
                    Goal.CalculateGoal();
                    Debug.Log("Updating time since the player missed the time slot.");
                }
            }
        }
    }
}

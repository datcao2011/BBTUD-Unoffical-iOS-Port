using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathMachineTrigger : MonoBehaviour
{
    public MathMachine mathMachine;

    public float timer;

    public float maxTimer = 60f;

    public bool countdown;

    private bool reachedZero;

    // Start is called before the first frame update
    private void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (countdown)
        {
            if (timer > 0f)
            {
                reachedZero = false;
                timer -= Time.deltaTime;
            }
            else if (!reachedZero)
            {
                reachedZero = true;
                if (!mathMachine.screensaver) mathMachine.StartScreenSaver();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        countdown = false;
        timer = maxTimer;
        if (mathMachine.screensaver && mathMachine.notebook.isActiveAndEnabled) mathMachine.StopScreenSaver();
    }

    private void OnTriggerExit(Collider other)
    {
        countdown = true;
    }
}

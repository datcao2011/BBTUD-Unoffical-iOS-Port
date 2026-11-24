using UnityEngine;
using System;

public class LostConnectionDisplay : MonoBehaviour
{
    public GameObject Object;

    public GameControllerScript gc;

    private bool foundSignal;

    private bool usingPCControls;

    private bool playOnce;

    private void Update()
    {
        if (InputManager.lostControllerSignal)
        {
            if (!usingPCControls)
            {
                foundSignal = false;
                Object.SetActive(true);
                if (!MultiplayerManager.isActive) Time.timeScale = 0f;
                UnlockCursor();
            }
        }
        else if (!foundSignal)
        {
            usingPCControls = false;
            foundSignal = true;
            Object.SetActive(false);
            if (!MultiplayerManager.isActive) Time.timeScale = Settings.GetFloat("TimeSpeed");
            if (!playOnce)
            {
                playOnce = true;
                LockCursor();
            }
        }
    }

    public void UsePC()
    {
        usingPCControls = true;
        Object.SetActive(false);
        if (!MultiplayerManager.isActive) Time.timeScale = Settings.GetFloat("TimeSpeed");
        LockCursor();
    }

    private void LockCursor()
    {
        gc.LockMouse();
    }

    private void UnlockCursor()
    {
        gc.lostConnection = true;
        gc.UnlockMouse();
    }
}
using UnityEngine;
using System.Collections;

public class LockDownDoorMain : MonoBehaviour
{
    public LockDownDoor[] doors;

    public int maxiumDoors;

    public GameObject guy;

    public bool end;

    private void Update()
    {
    }

    public void TestCodeRed(float t)
    {
        end = false;
        StartCoroutine(doorTimer(t));
    }

    private IEnumerator doorTimer(float time)
    {
        lockDownDoorAll(false);
        float t = time;
        while (t > 0f && !end)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        end = false;
        while (doors[6].busy)
        {
            yield return null;
        }
        lockDownDoorAll(true);
        yield break;
    }

    public void RealCodeRed(float t)
    {
        end = false;
        StartCoroutine(doorTimerr(t));
    }

    private IEnumerator doorTimerr(float expectedTime)
    {
        StartCoroutine(Guy());
        float t = expectedTime;
        while (t > 0f && !end)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        end = false;
        if (!GameObject.Find("GameController").GetComponent<GameControllerScript>().finaleMode)
        {
            GameObject.Find("GameController").GetComponent<GameControllerScript>().SetTextBlack();
        }
        RenderSettings.fogColor = Color.white;
        RenderSettings.ambientLight = Color.white;
        RenderSettings.fogDensity = 0f;
        RenderSettings.fog = false;
        lockDownDoorAll(true);
        yield break;
    }

    private IEnumerator Guy()
    {
        yield return new WaitForSeconds(7f);
        if (FindObjectOfType<HudScreenTextScript>())
        {
            FindObjectOfType<HudScreenTextScript>().ShowHudText(10f, "The security doors will be closing in 10 seconds.");
        }
        yield return new WaitForSeconds(10f);
        lockDownDoorAll(false);
        while (!doors[9].closed)
        {
            yield return null;
        }
        FindObjectOfType<RandomEvent>().StartCoroutine(FindObjectOfType<RandomEvent>().BoardScreen_LockdownTimer());
        RenderSettings.fogColor = Color.black;
        RenderSettings.ambientLight = Color.black;
        GameObject.Find("GameController").GetComponent<GameControllerScript>().SetTextWhite();
        FindObjectOfType<HudScreenTextScript>().ShowHudText(5f, "Tip:\nPress " + (InputManager.usingTouch ? "the Flashlight Button" : "F") + " to toggle the flashlight.");
        RenderSettings.fogDensity = 0.008f;
        RenderSettings.fog = true;
        guy.GetComponent<Guy>().Summon();
        yield break;
    }

    public void lockDownDoorAll(bool open)
    {
        if (open)
        {
            int doorsCount = 0;
            while (doorsCount < maxiumDoors)
            {
                doors[doorsCount].Open();
                doorsCount++;
            }
            if (doors[9].closed)
            {
                doors[9].Open();
            }
        }
        else
        {
            int doorsCount = 0;
            while (doorsCount < maxiumDoors)
            {
                doors[doorsCount].Close();
                doorsCount++;
            }
            if (doors[9].opened)
            {
                doors[9].Close();
            }
        }
    }

    public void lockDownDoorInstantAll(bool open)
    {
        if (open)
        {
            int doorsCount = 0;
            while (doorsCount < maxiumDoors)
            {
                doors[doorsCount].InstantOpen();
                doorsCount++;
            }
            if (doors[9].closed)
            {
                doors[9].InstantOpen();
            }
        }
        else
        {
            int doorsCount = 0;
            while (doorsCount < maxiumDoors)
            {
                doors[doorsCount].InstantShut();
                doorsCount++;
            }
            if (doors[9].opened)
            {
                doors[9].InstantShut();
            }
        }
    }
}
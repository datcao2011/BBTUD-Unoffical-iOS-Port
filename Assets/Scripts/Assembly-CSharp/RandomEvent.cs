using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Multiplayer;

public class RandomEvent : MonoBehaviour
{
    public float eventTimer;

    public GameControllerScript gameController;

    public PartyScript PartyManager;

    public FogScript fogScript;

    private bool countdown = true;

    private float delayStart;

    public AudioSource bell;

    public FloodScript floodScript;

    public AudioSource partyMusic;

    public AudioSource fogSoundtrack;

    public int eventChoose;

    public bool forceCodeRed;

    public bool forceTestCodeRed;

    public bool timeY;

    public bool timeNow;

    private int legendChance;

    private float hideDelay;

    public bool cannotBeStoppedByCheats;

    public GameObject objectEvent;

    public GameObject fogObject;

    public TMP_Text eventText;

    public LockDownDoorMain lockDownDoorMain;

    public StatusBoardScreen statusBoardScreen;

    public BaldiScript baldi;

    public bool codeRed;

    private bool[] WasElevatorUnavailable = new bool[2];

    [SerializeField]
    private AudioClip NewMusicTune;

    [SerializeField]
    private AudioClip[] BaldiClips = new AudioClip[7];

    private bool DelayStarted;

    private void Start()
    {
        ResetTimer();
        if (MultiplayerManager.isActive) // Portal poster isn't available.
        {
            Gifts[2].name = "Pickup_AlarmClock";
            Gifts[2].GetComponentInChildren<SpriteRenderer>().sprite = gameController.itemSprites[7];
        }
        if (Settings.GetInt("TVHud") <= 1)
        {
            bool IsAlreadyPlaying = false;
            if (bell.isPlaying)
            {
                IsAlreadyPlaying = true;
            }
            bell.clip = NewMusicTune;
            if (IsAlreadyPlaying) bell.Play();
        }
    }

    private AudioClip BaldiClip;

    private void Update()
    {
        if (eventTimer > 0f && countdown)
        {
            eventTimer -= Time.deltaTime;
        }
        if (eventTimer < 0f)
        {
            eventTimer = 0f;
        }
        if (eventTimer == 0f)
        {
            if (!timeNow)
            {
                timeNow = true;
                if (MultiplayerManager.isActive)
                {
                    if (MultiplayerManager.instance.isMainPlayer)
                    {
                        int eventChoose = Mathf.RoundToInt((float)UnityEngine.Random.Range(0f, 6f));
                        if (eventChoose == 5) { eventChoose = 2; }
                        ClientSend.RandomEvent(eventChoose);
                    }
                }
                else startRandomEvent();
            }
        }
        if (delayStart > 0f)
        {
            delayStart -= Time.deltaTime;
            if (delayStart <= 0.5f && !DelayStarted)
            {
                DelayStarted = true;
                BaldiClip = BaldiClips[eventChoose];
                gameController.TV.PlayAudioBeforeAscend(BaldiClip, 1f, Mathf.Clamp(delayStart, 0.25f, 0.5f) - 0.25f);
            }
        }
        if (delayStart < 0f)
        {
            delayStart = 0f;
        }
        if (delayStart == 0f)
        {
            if (!DelayStarted)
            {
                DelayStarted = true;
                BaldiClip = BaldiClips[eventChoose];
                gameController.TV.PlayAudioBeforeAscend(BaldiClip, 1f, 0.25f);
            }
            if (!timeY)
            {
                timeY = true;
                startEvent();
            }
        }
        if (hideDelay > 0f)
        {
            hideDelay -= Time.deltaTime;
        }
        if (hideDelay < 0f)
        {
            hideDelay = 0f;
        }
        if (hideDelay == 0f)
        {
            objectEvent.SetActive(false);
        }
        if (eventEndTimer > 0f)
        {
            eventEndTimer -= Time.deltaTime;
        }
        if (eventEndTimer < 0f)
        {
            eventEndTimer = 0f;
        }
        if (eventEndTimer == 0f)
        {
            if (eventStarted)
            {
                eventStarted = false;
                party = false;
                principalBreak = false;
                partyMusic.Stop();
                fogSoundtrack.Stop();

                if (eventChoose == 0 || eventChoose == 6)
                {
                    statusBoardScreen.ResetStatus();
                }
                if (eventChoose == 3)
                {
                    EndEventText("Looks like Principal is done taking his break.");
                }

                if (eventChoose == 4)
                {
                    StartCoroutine(BoardScreen_ETEnded());
                }
                if (eventChoose == 5)
                {
                    EndEventText("All clear! Emergency has ended!");
                    AchievementManager.Instance.Earn(Achievements.Lockdown);
                    StartCoroutine(BoardScreen_EEnded());
                    gameController.SetAllCharacters(true);
                    codeRed = false;
                    if (!gameController.spoopMode)
                        StartCoroutine(fadeMusicUp(gameController.schoolMusic));
                    ElevatorScript[] Elevators = FindObjectsOfType<ElevatorScript>();
                    for (int i = 0; i < Elevators.Length; i++)
                    {
                        Elevators[i].unavailable = WasElevatorUnavailable[i];
                    }
                }
                ResetTimer();
                // In case of errors. This is last.
                if (eventChoose == 7)
                {
                    EndEventText("The mystery room has now disappeared!");
                    CurrentMysteryRoom.EventEnded();
                    CurrentMysteryRoom = null;
                }
            }
        }
    }

    private IEnumerator BoardScreen_FlashStatusText(string value, Color textColor, int times)
    {
        statusBoardScreen.SetStatus(value, textColor);
        yield return new WaitForSeconds(0.5f);
        statusBoardScreen.SetStatus("", Color.black);
        yield return new WaitForSeconds(0.5f);
        statusBoardScreen.SetStatus(value, textColor);
        if (times > 0)
        {
            StartCoroutine(BoardScreen_FlashStatusText(value, textColor, times - 1));
        }
        yield break;
    }

    private IEnumerator BoardScreen_ETEnded()
    {
        statusBoardScreen.SetWallDoorStatus("Lifting", Color.yellow);
        statusBoardScreen.SetInformation("Lockdown drill ended. Lifting wall doors...", Color.yellow);
        statusBoardScreen.SetStatus("OK", Color.green);
        yield return new WaitForSeconds(5f);
        statusBoardScreen.ResetStatus();
        yield break;
    }

    private IEnumerator BoardScreen_EEnded()
    {
        statusBoardScreen.SetLightStatus("ON", Color.green);
        statusBoardScreen.SetWallDoorStatus("Lifting", Color.yellow);
        statusBoardScreen.SetInformation("The intruder left the school building. Lockdown has ended. Lifting wall doors...", Color.yellow);
        StartCoroutine(BoardScreen_FlashStatusText("ALL CLEAR", Color.yellow, 4));
        yield return new WaitForSeconds(5f);
        statusBoardScreen.ResetStatus();
        yield break;
    }

    private IEnumerator BoardScreen_RealLockDown()
    {
        StartCoroutine(BoardScreen_FlashStatusText("WARNING", Color.red, 17));
        statusBoardScreen.SetLightStatus("ON", Color.green);
        statusBoardScreen.SetWallDoorStatus("Lifted", Color.green);
        float timerToLockdown = 17f;
        while (timerToLockdown > 0f)
        {
            timerToLockdown -= Time.deltaTime;
            statusBoardScreen.SetInformation("A intruder has been spotted outside the school. Starting Lockdown: " + GetTimerText(timerToLockdown), Color.red);
            yield return null;
        }
        statusBoardScreen.SetWallDoorStatus("Releasing", Color.yellow);
        statusBoardScreen.SetInformation("Lockdown started: Releasing all wall doors.", Color.red);
        yield break;
    }

    private IEnumerator BoardScreen_Flood()
    {
        StartCoroutine(BoardScreen_FlashStatusText("FLOOD INBOUND", Color.blue, 10));
        statusBoardScreen.SetInformation("Huge water has entered the school building creating a flood. In Progress: --:--", Color.blue);
        yield return new WaitForSeconds(1f);
        float timerFlood = eventEndTimer;
        while (timerFlood > 0f)
        {
            timerFlood -= Time.deltaTime;
            statusBoardScreen.SetInformation("Huge water has entered the school building creating a flood. In Progress: " + GetTimerText(timerFlood), Color.blue);
            yield return null;
        }
        yield break;
    }

    private IEnumerator BoardScreen_FogMachine()
    {
        statusBoardScreen.SetStatus("Partial Issues", Color.yellow);
        statusBoardScreen.SetFogMachineStatus("Malfunctioned On", Color.red);
        statusBoardScreen.SetInformation("Fog machine is currently being malfunctioned. Estimated Fix: --:--", Color.yellow);
        yield return new WaitForSeconds(1f);
        float timerFlood = eventEndTimer;
        while (timerFlood > 0f)
        {
            timerFlood -= Time.deltaTime;
            statusBoardScreen.SetInformation("Fog machine is currently being malfunctioned. Estimated Fix: " + GetTimerText(timerFlood), Color.yellow);
            yield return null;
        }
        yield break;
    }

    private IEnumerator BoardScreen_TestLockDown()
    {
        statusBoardScreen.SetStatus("Lockdown Drill", Color.yellow);
        statusBoardScreen.SetWallDoorStatus("Releasing", Color.yellow);
        statusBoardScreen.SetInformation("A lockdown drill is starting. Releasing wall doors...", Color.yellow);
        yield return new WaitForSeconds(5f);
        statusBoardScreen.SetWallDoorStatus("Released", Color.red);
        float timerOfLockdown = eventEndTimer;
        while (timerOfLockdown > 0f)
        {
            timerOfLockdown -= Time.deltaTime;
            statusBoardScreen.SetInformation("Lockdown drill currently in progress: " + GetTimerText(timerOfLockdown), Color.yellow);
            yield return null;
        }
        yield break;
    }

    public IEnumerator BoardScreen_LockdownTimer()
    {
        statusBoardScreen.SetStatus("LOCKDOWN", Color.red);
        statusBoardScreen.SetWallDoorStatus("Released", Color.red);
        statusBoardScreen.SetLightStatus("OFF", Color.red);
        float timerOfLockdown = eventEndTimer;
        float t = 10f;
        float maxT = 10f;
        bool countUp = false;
        while (timerOfLockdown > 0f)
        {
            timerOfLockdown -= Time.deltaTime;
            if (timerOfLockdown <= 60)
            {
                statusBoardScreen.SetInformation("A intruder has entered the school building. Lockdown is currently in progress: " + GetTimerText(timerOfLockdown), Color.red);
            }
            else
            {
                if (!countUp)
                {
                    if (t <= 0)
                    {
                        countUp = true;
                    }
                    else
                    {
                        t -= Time.deltaTime;
                        statusBoardScreen.SetInformation("Lockdown started and in progress. Get to a safe spot NOW.", Color.red);
                    }
                }
                else
                {
                    if (t >= maxT)
                    {
                        countUp = false;
                    }
                    else
                    {
                        t += Time.deltaTime;
                        statusBoardScreen.SetInformation("A intruder has entered the school building. Lockdown is currently in progress: " + GetTimerText(timerOfLockdown), Color.red);
                    }
                }
            }
            yield return null;
        }
        yield break;
    }

    private string GetTimerText(float timer)
    {
        float timeMinutes = Mathf.Floor(timer / 60);
        float timeSeconds = (float)Mathf.FloorToInt(timer % 60);
        string result;
        string minutes;
        string seconds;
        float currentMinutes = timeMinutes;
        float currentSeconds = timeSeconds;
        if (currentSeconds < 0f)
        {
            return "00:00";
        }
        if (currentSeconds > 59f)
        {
            currentSeconds = 0f;
            currentMinutes += 1f;
        }
        if (currentMinutes < 10f)
        {
            minutes = "0" + currentMinutes;
        }
        else
        {
            minutes = currentMinutes.ToString();
        }
        if (currentSeconds < 10f)
        {
            seconds = "0" + currentSeconds;
        }
        else
        {
            seconds = currentSeconds.ToString();
        }
        result = minutes + ":" + seconds;
        return result;
    }

    public void StopEventNOW()
    {
        if (delayStart > 0f)
        {
            timeY = true;
            delayStart = 0f;
            ResetTimer();
        }
        if (eventEndTimer > 0f)
        {
            base.StopAllCoroutines();
            baldi.stopTimer = true;
            fogScript.timer = 0f;
            eventEndTimer = 0f;
            lockDownDoorMain.end = true;
        }
    }

    private void ResetTimer()
    {
        eventTimer = 180f;
        timeNow = false;
        DelayStarted = true;
    }

    public void PauseRandomEvents()
    {
        countdown = false;
    }

    public void ResumeRandomEvents()
    {
        countdown = true;
    }

    public void startRandomEvent()
    {
        if (gameController.TV.isActiveAndEnabled)
        {
            gameController.TV.DescendWithAlert(false);
        }
        bell.Play();
        delayStart = 3f;
        timeY = false;
        DelayStarted = false;
        ChoseRandomEvent();
    }

    public void ChooseEvent(int index)
    {
        timeNow = true;
        eventTimer = 0f;
        if (gameController.TV.isActiveAndEnabled)
        {
            gameController.TV.DescendWithAlert(false);
        }
        bell.Play();
        delayStart = 3f;
        timeY = false;
        DelayStarted = false;
        legendChance = Mathf.RoundToInt((float)Random.Range(0f, 99f));
        if (legendChance < 98)
        {
            prizeCount = Mathf.RoundToInt((float)UnityEngine.Random.Range(0f, Gifts.Length - 1f));
        }
        else
        {
            prizeCount = Gifts.Length;
        }
        eventChoose = index;

        if (eventChoose == 5)
        {
            StartCoroutine(fadeMusicDown(gameController.schoolMusic));
        }
    }

    private float CustomDuration = -1f;

    [SerializeField]
    private MysteryRoom[] MysteryRooms;

    private MysteryRoom CurrentMysteryRoom;

    public void InstantChooseEvent(int index, float CustomDuration)
    {
        timeNow = true;
        eventTimer = 0f;
        bell.Play();
        this.CustomDuration = CustomDuration; // I forgot to add this :( Open-Source Exclusive
        if (gameController.TV.isActiveAndEnabled)
        {
            gameController.TV.DescendWithAlert(true);

            BaldiClip = BaldiClips[index];
            gameController.TV.PlayAudioBeforeAscend(BaldiClip, 1f, 0.25f);
        }
        delayStart = 0f;
        timeY = false;
        DelayStarted = true;
        legendChance = Mathf.RoundToInt((float)Random.Range(0f, 99f));
        if (legendChance < 98)
        {
            prizeCount = Mathf.RoundToInt((float)UnityEngine.Random.Range(0f, Gifts.Length - 1f));
        }
        else
        {
            prizeCount = Gifts.Length;
        }
        eventChoose = index;
        if (eventChoose == 5)
        {
            gameController.schoolMusic.volume = 0f;
        }
    }

    private void ChoseRandomEvent()
    {
        eventChoose = Mathf.RoundToInt((float)UnityEngine.Random.Range(0f, 7f));
        legendChance = Mathf.RoundToInt((float)Random.Range(0f, 99f));
        float redChange = Random.Range(0f, 99f);
        if (legendChance < 98)
        {
            prizeCount = Mathf.RoundToInt((float)UnityEngine.Random.Range(0f, Gifts.Length - 1f));
        }
        else
        {
            prizeCount = Gifts.Length;
        }
        while ((eventChoose == 2 && !baldi.isActiveAndEnabled) || (eventChoose == 5))
        {
            eventChoose = Mathf.RoundToInt((float)UnityEngine.Random.Range(0f, 7f));
        }
        if ((redChange >= 97f || forceCodeRed) && NullManager.Instance == null)
        {
            eventChoose = 5;
            StartCoroutine(fadeMusicDown(gameController.schoolMusic));
        }
        if (forceTestCodeRed)
        {
            eventChoose = 4;
        }
    }
    private void startEvent()
    {
        hideDelay = Mathf.Clamp(BaldiClip != null ? BaldiClip.length : 10f, 5f, 20f);
        float EndTime = (float)Mathf.RoundToInt(Random.Range(40f, 80f));
        if (MultiplayerManager.isActive)
        {
            EndTime = 45f; // Yeah.
        }
        if (NullManager.Instance != null)
        {
            EndTime = 20f;
        }
        if (CustomDuration != -1f)
        {
            EndTime = CustomDuration;
        }
        if (eventChoose == 0)
        {
            eventText.text = "Bell Event:\nBaldi's fog machine malfunctioned again!\nAck! Hack!";
            StartCoroutine(BoardScreen_FogMachine());
            fogScript.StartFading(EndTime);
            fogSoundtrack.Play();
        }
        else if (eventChoose == 1)
        {
            eventText.text = "Bell Event:\nParty at the Principal's office! Come and get your present!";
            party = true;
            partyMusic.Play();
            if (prizeCount == Gifts.Length)
            {
                legendaryPrize.SetActive(true);
            }
            else
            {
                Gifts[prizeCount].SetActive(true);
            }
        }
        else if (eventChoose == 2)
        {
            eventText.text = "Bell Event:\nUh oh, sounds like Baldi is going to need a new ruler...";
            baldi.BreakRuler(EndTime);
        }
        else if (eventChoose == 3)
        {
            eventText.text = "Bell Event:\nLooks like principal is going to take a break. Meanwhile, let's break the school rules.";
            principalBreak = true;
        }
        else if (eventChoose == 4)
        {
            StartCoroutine(BoardScreen_TestLockDown());
            eventText.text = "Bell Event:\nAttention! This is a test lockdown. The doors will close for the test.";
            lockDownDoorMain.TestCodeRed(EndTime);
        }
        else if (eventChoose == 5)
        {
            ElevatorScript[] Elevators = FindObjectsOfType<ElevatorScript>();
            for (int i = 0; i < Elevators.Length; i++)
            {
                WasElevatorUnavailable[i] = Elevators[i].unavailable;
                Elevators[i].unavailable = true;
            }
            StartCoroutine(BoardScreen_RealLockDown());
            eventText.text = "Bell Event:\nATTENTION! A INTRUDER ENTERED THE BUILDING. LOCKDOWN WILL BEGIN.";
            EndTime = Random.Range(240f, 600f);
            if (CustomDuration != -1f)
            {
                EndTime = CustomDuration;
            }
            lockDownDoorMain.RealCodeRed(EndTime);
            gameController.SetAllCharacters(false);
            codeRed = true;
        }
        else if (eventChoose == 6)
        {
            eventText.text = "Bell Event:\nLooks like the leak is leaking again. Now there is water everywhere!";
            if (CustomDuration != -1f)
            {
                EndTime = CustomDuration;
            }
            StartCoroutine(BoardScreen_Flood());
            floodScript.StartEvent();
        }
        else if (eventChoose == 7)
        {
            eventText.text = "Bell Event:\nA mystery room has appeared in floor 1! Find it before it disappears!";
            EndTime = Random.Range(90f, 120f);
            if (CustomDuration != -1f)
            {
                EndTime = CustomDuration;
            }
            if (MysteryRooms.Length > 0)
            {
                CurrentMysteryRoom = MysteryRooms[Random.Range(0, MysteryRooms.Length)];
                CurrentMysteryRoom.ReappearItem();
            }
        }
        if (MultiplayerManager.isActive)
        {
            EndTime = 45f; // Making sure
            if (MultiplayerManager.instance.isBaldi)
            {
                if (Settings.GetInt("TVHud") > 0 || BaldiClip == null)
                {
                    FindObjectOfType<HudScreenTextScript>().ShowHudText(hideDelay, eventText.text);
                }
                if (eventChoose == 2)
                {
                    MultiplayerManager.instance.CreateMiniMessage("That's not good! Your ruler broke.", Color.yellow, EndTime);
                }
            }
            else if ((Settings.GetInt("TVHud") > 0 || BaldiClip == null) && gameController.SpectatingDisplay.activeSelf && FindObjectOfType<HudScreenTextScript>())
            {
                FindObjectOfType<HudScreenTextScript>().ShowHudText(hideDelay, eventText.text);
            }
        }
        if (NullManager.Instance != null)
        {
            EndTime = 20f;
        }
        if (CustomDuration != -1f)
        {
            EndTime = CustomDuration;
        }
        CustomDuration = -1f;
        if (Settings.GetInt("TVHud") > 0 || BaldiClip == null)
        {
            objectEvent.SetActive(true);
        }
        eventEndTimer = EndTime;
        eventStarted = true;
    }

    public void EndEventText(string stringText)
    {
        hideDelay = 10f;
        eventText.text = "Event Ended:\n" + stringText;
        if (Settings.GetInt("TVHud") > 0)
            objectEvent.SetActive(true);
    }

    public void FadeMusicDown(AudioSource device)
    {
        StartCoroutine(fadeMusicDown(device));
    }

    public void FadeMusicUp(AudioSource device)
    {
        StartCoroutine(fadeMusicUp(device));
    }

    private IEnumerator fadeMusicDown(AudioSource device)
    {
        while (device.volume > 0f)
        {
            device.volume -= Time.unscaledDeltaTime * 1.2f;
            yield return null;
        }
        device.Pause();
        yield break;
    }

    private IEnumerator fadeMusicUp(AudioSource device)
    {
        device.UnPause();
        while (device.volume < 1f)
        {
            device.volume += Time.unscaledDeltaTime * 1.2f;
            yield return null;
        }
        yield break;
    }

    public float eventEndTimer;

    public bool principalBreak;

    public int prizeCount;

    public GameObject[] Gifts;
    public GameObject legendaryPrize;

    public bool eventStarted;

    public bool party;
}
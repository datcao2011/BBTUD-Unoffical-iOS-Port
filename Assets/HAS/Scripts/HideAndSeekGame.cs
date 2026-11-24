using UnityEngine;
using TMPro;
using System.Collections;

public class HideAndSeekGame : MonoBehaviour
{
    public string hiderOrSeeker;

    public int maxRounds = 3;

    public int rounds;
    public int playerWins;
    public int baldiWins;

    public float seekTimer = 30f;
    public float timer = 180f;

    public GameControllerScript gc;

    public AudioClip baldiHAS;
    public AudioClip baldiYGLKP;
    public AudioClip baldiWFLKP;
    public AudioClip baldiIGH;
    public AudioClip baldiIGS;
    public AudioClip baldiCountdown;

    public AudioSource audioDevice;

    public TMP_Text text;

    public void StartGame()
    {
        int max = maxRounds;
        text = GameObject.Find("HAS Text").GetComponent<TMP_Text>();
        GameObject BaldiAudio = new GameObject("Baldi Temporary Audio");
        BaldiAudio.transform.position = gc.baldiTutor.transform.position;
        Destroy(BaldiAudio, 45f); // It will no longer be needed in 45 seconds.
        audioDevice = BaldiAudio.AddComponent<AudioSource>();
        BaldiAudio.AddComponent<AudioSourceSpeed>();
        audioDevice.spatialBlend = 1f;
        audioDevice.minDistance = gc.tutorBaldi.minDistance;
        audioDevice.maxDistance = gc.tutorBaldi.maxDistance;
        audioDevice.rolloffMode = gc.tutorBaldi.rolloffMode;
        audioDevice.dopplerLevel = 0f;
        audioDevice.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ACharacters : null;
        string hos = Mathf.RoundToInt(Random.Range(0f, 1f)) == 1 ? "Hider" : "Seeker";
        hiderOrSeeker = PlayerPrefs.GetString("HAS_HOS", hos);
        if (!PlayerPrefs.HasKey("HAS_HOS")) PlayerPrefs.SetString("HAS_HOS", hos);
        gc.tutorBaldi.Stop();
        rounds = PlayerPrefs.GetInt("HAS_Rounds", 1);
        maxRounds = PlayerPrefs.GetInt("HAS_MRounds", max);
        baldiWins = PlayerPrefs.GetInt("HAS_BWins", 0);
        playerWins = PlayerPrefs.GetInt("HAS_PWins", 0);
        text.text = "Round " + rounds + "/" + maxRounds + "\nYou're a " + hiderOrSeeker.ToUpper() + ".\nTime Left: -:--\nBaldi: " + baldiWins + "   You: " + playerWins;
        if (GameObject.Find("Pickup_Apple"))
        {
            GameObject OBJ = GameObject.Find("Pickup_Apple");
            OBJ.name = "Pickup_Chalk";
            OBJ.GetComponentInChildren<SpriteRenderer>().sprite = gc.itemSprites[23];
        }
        StartCoroutine(Intro());
    }

    private IEnumerator Intro()
    {
        float Duration = 5f;
        if (PlayerPrefs.GetString("HAS_KNH", "") == "_")
        {
            gc.tutorBaldi.clip = baldiYGLKP;
        }
        else if (rounds > 1)
        {
            gc.tutorBaldi.clip = baldiWFLKP;
            Duration = 4f;
        }
        else
        {
            gc.tutorBaldi.clip = baldiHAS;
        }
        gc.tutorBaldi.Play();
        yield return new WaitForSeconds(Duration);
        StartCoroutine(SeekTimer());
        yield break;
    }

    private IEnumerator SeekTimer()
    {
        gc.baldiTutor.SetActive(false);
        gc.baldi.SetActive(true);
        gc.baldiScrpt.position = new Vector3(5f, gc.baldiTransform.position.y, 10f);
        gc.baldiScrpt.GetAngry(4f);
        bool countDownPlaying = false;
        GameObject SeekWaiting = GameObject.Find("Seek Waiting").transform.GetChild(0).gameObject;
        if (hiderOrSeeker == "Hider")
        {
            gc.baldiScrpt.SetTarget(new Vector3(-45f, 0f, 45f));
            audioDevice.PlayOneShotWC(baldiIGH);
            ElevatorScript.U(true); // Makes all the elevators unavailable
            foreach (HideableLocker lockers in FindObjectsOfType<HideableLocker>())
            {
                lockers.LimitedTime = true;
                lockers.AllowCharactersSeeInside = true;
            }
        }
        else
        {
            audioDevice.spatialBlend = 0f;
            audioDevice.PlayOneShotWC(baldiIGS);
            SeekWaiting.SetActive(true);
            gc.player.SetPosition(new Vector3(-45f, 4f, 45f));
            gc.player.SetRotation(Quaternion.identity);
        }
        while (seekTimer > 0f)
        {
            SeekWaiting.GetComponentInChildren<TMP_Text>().text = "You're seeking this round.\nTime Till Seek: " + seekTimer.ToString("0");
            text.text = "Round " + rounds + "/" + maxRounds + "\nYou're a " + hiderOrSeeker.ToUpper() + ".\nTime Till Seek: " + GetTimerText(seekTimer) + "\nBaldi: " + baldiWins + "   You: " + playerWins;
            if (seekTimer <= 10 && !countDownPlaying)
            {
                countDownPlaying = true;
                if (hiderOrSeeker == "Hider")
                {
                    audioDevice.transform.position = gc.baldiTransform.position;
                    audioDevice.PlayOneShot(baldiCountdown);
                }
            }
            seekTimer -= Time.deltaTime;
            yield return null;
        }

        if (hiderOrSeeker == "Hider")
        {
            gc.baldiScrpt.Wander();
        }
        else
        {
            SeekWaiting.SetActive(false);
            gc.player.SetPosition(new Vector3(-45f, 4f, 45f));
            gc.player.SetRotation(Quaternion.identity);
        }
#if DEBUG
        console.log("Seeker is released.");
#endif
        StartCoroutine(HiderTimer());
        yield break;
    }

    private IEnumerator HiderTimer()
    {
#if DEBUG
        console.log("Timer started: 3 minutes");
#endif
        while (timer > 0f)
        {
            text.text = "Round " + rounds + "/" + maxRounds + "\nYou're a " + hiderOrSeeker.ToUpper() + ".\nTime Left: " + GetTimerText(timer) + "\nBaldi: " + baldiWins + "   You: " + playerWins;
            if (!gc.player.gameOver) timer -= Time.deltaTime;
            yield return null;
        }

        if (hiderOrSeeker == "Seeker")
        {
            baldiWins++;
            PlayerPrefs.SetString("HAS_KNH", "");
            RoundResults.roundMaxed = rounds >= maxRounds;
            RoundResults.playerWin = !(playerWins < baldiWins);
            string str = "Baldi (" + (hiderOrSeeker == "Seeker" ? "Hider" : "Seeker") + ")";
            RoundResults.textToInput = "Round: " + rounds + "/" + maxRounds + "\nPlayer Wins: " + playerWins + "     Baldi Wins: " + baldiWins + "\nWho Won? " + str + "\nYou were a " + hiderOrSeeker.ToLower() + ".\nTime Remaining: " + GetTimerText(timer);
        }
        else
        {
            playerWins++;
            PlayerPrefs.SetString("HAS_KNH", "_");
            RoundResults.roundMaxed = rounds >= maxRounds;
            RoundResults.playerWin = !(playerWins < baldiWins);
            string str = "Player (" + hiderOrSeeker + ")";
            RoundResults.textToInput = "Round: " + rounds + "/" + maxRounds + "\nPlayer Wins: " + playerWins + "     Baldi Wins: " + baldiWins + "\nWho Won? " + str + "\nYou were a " + hiderOrSeeker.ToLower() + ".\nTime Remaining: " + GetTimerText(timer);
        }

        PlayerPrefs.SetInt("HAS_Rounds", rounds + 1);
        PlayerPrefs.SetInt("HAS_MRounds", maxRounds);
        PlayerPrefs.SetInt("HAS_BWins", baldiWins);
        PlayerPrefs.SetInt("HAS_PWins", playerWins);
#if DEBUG
        console.log("Time is finished!");
#endif
        UnityEngine.SceneManagement.SceneManager.LoadScene("HAS Results");
        yield break;
    }

    public void EndRound(bool GameOver = false)
    {
        if (hiderOrSeeker == "Hider" || GameOver)
        {
            baldiWins++;
            PlayerPrefs.SetString("HAS_KNH", "");
            RoundResults.roundMaxed = rounds >= maxRounds;
            RoundResults.playerWin = !(playerWins < baldiWins);
            string str = "Baldi (" + (hiderOrSeeker == "Seeker" ? "Hider" : "Seeker") + ")";
            RoundResults.textToInput = "Round: " + rounds + "/" + maxRounds + "\nPlayer Wins: " + playerWins + "     Baldi Wins: " + baldiWins + "\nWho Won? " + str + "\nYou were a " + hiderOrSeeker.ToLower() + ".\nTime Remaining: " + GetTimerText(timer);
        }
        else
        {
            playerWins++;
            PlayerPrefs.SetString("HAS_KNH", "_");
            RoundResults.roundMaxed = rounds >= maxRounds;
            RoundResults.playerWin = !(playerWins < baldiWins);
            string str = "Player (" + hiderOrSeeker + ")";
            RoundResults.textToInput = "Round: " + rounds + "/" + maxRounds + "\nPlayer Wins: " + playerWins + "     Baldi Wins: " + baldiWins + "\nWho Won? " + str + "\nYou were a " + hiderOrSeeker.ToLower() + ".\nTime Remaining: " + GetTimerText(timer);
        }

        PlayerPrefs.SetInt("HAS_Rounds", rounds + 1);
        PlayerPrefs.SetInt("HAS_MRounds", maxRounds);
        PlayerPrefs.SetInt("HAS_BWins", baldiWins);
        PlayerPrefs.SetInt("HAS_PWins", playerWins);
#if DEBUG
        console.log("Hider was captured.");
#endif
        UnityEngine.SceneManagement.SceneManager.LoadScene("HAS Results");
    }

    private string GetTimerText(float timer)
    {
        float timeMinutes = Mathf.Floor(timer / 60);
        float timeSeconds = (float)Mathf.CeilToInt(timer % 60);
        string result;
        string minutes;
        string seconds;
        float currentMinutes = timeMinutes;
        float currentSeconds = timeSeconds;
        if (currentSeconds <= 0f)
        {
            return "0:00";
        }
        if (currentSeconds > 59f)
        {
            currentSeconds = 0f;
            currentMinutes += 1f;
        }
        minutes = currentMinutes.ToString();
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
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BirthdayBash : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("[Birthday Bash] Birthday bash mode detected...");
        ChangeCharacters();
        ChangeItems();
        ChangeMachines();

        FindObjectOfType<ElevatorScript>().unavailable = true;
        FindObjectOfType<PlayerScript>().SetPosition(new Vector3(5f, 4f, 0f));
        FindObjectOfType<PlayerScript>().Gravity = false;

        SecondFloor.SetActive(false);
        Cameras.SetActive(false);

        FindObjectOfType<RandomEvent>().PauseRandomEvents();
        Instance = this;
        GameObject.Instantiate(Balloons);
    }

    [SerializeField]
    public AudioClip[] clips;
    [SerializeField]
    public RuntimeAnimatorController[] animators;
    [SerializeField]
    public Sprite[] sprites;
    public GameObject[] machines;
    public static BirthdayBash Instance { get; private set; }
    public GameObject Coin;
    public GameObject Cameras;
    public Material GACI;
    public Material GACI_BSODA;
    public GameObject Balloons;
    public GameObject SecondFloor;
    public GameObject BirthdayBashStuff;
    public Animator baldiAnim;
    public GameControllerScript gc;
    public bool SecondGlitchActive;

    private void ChangeCharacters()
    {
        gc = FindObjectOfType<GameControllerScript>();
        Debug.Log("[Birthday Bash] Changing characters...");
        #region BALDI
        GameObject TutorBaldi = GameObject.Find("TutorBaldi");
        if (TutorBaldi != null)
        {
            TutorBaldi.GetComponent<AudioSource>().clip = clips[0];
            TutorBaldi.GetComponent<AudioSource>().PlayOneShot(clips[0]);
            TutorBaldi.GetComponentInChildren<Animator>().runtimeAnimatorController = animators[0];
            TutorBaldi.GetComponentInChildren<TutorBaldi>().SaidHi = true;
        }
        baldiAnim.runtimeAnimatorController = animators[2];
        #endregion
        #region PLAYTIME
        GameObject.Find("Playtime").GetComponents<AudioSource>()[1].clip = clips[1];
        GameObject.Find("Playtime").GetComponentInChildren<Animator>().runtimeAnimatorController = animators[1];
        GameObject.Find("Playtime").GetComponents<AudioSource>()[1].Play();
        #endregion
        #region PRINCIPAL
        GameObject.Find("Principal of the Thing").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Principal of the Thing").transform.GetChild(1).gameObject.SetActive(true);
        #endregion
        #region GOTTA SWEEP
        GameObject.Find("Gotta Sweep").GetComponentInChildren<SpriteRenderer>().sprite = sprites[0];
        #endregion
        #region ITS A BULLY
        GameObject.Find("Its a Bully").GetComponentInChildren<SpriteRenderer>().sprite = sprites[2];
        #endregion
    }

    private void ChangeItems()
    {
        string[] Names = new string[] {
            "Pickup_YellowDoorLock",
            "Pickup_Teleporter",
            "Pickup_Pencil",
            "Pickup_Apple",
            "Pickup_AlarmClock",
            "Pickup_CoinDoor",
            "Pickup_Health",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_EnergyFlavoredZestyBar",
            "Pickup_Key",
            "Pickup_SafetyScissors",
            "Pickup_SafetyScissors",
            "Pickup_SafetyScissors",
            "Pickup_Tape",
            "Pickup_WD-3D",
            "Pickup_WD-3D",
            "Pickup_Quarter",
            "Pickup_Boots",
            "Pickup_BSODA",
            "Pickup_BSODA",
            "Pickup_BSODA",
            "Pickup_Apple",
            "Pickup_Apple",
            "Pickup_AppleGod",
            "Pickup_Whistle"
        };
        foreach (string name in Names)
        {
            ChangeItem(GameObject.Find(name));
        }
        ChangeItem(Coin);
    }

    public void ChangeItem(GameObject item)
    {
        if (item == null) return;
        item.name = "Pickup_Present";
        item.GetComponentInChildren<SpriteRenderer>().sprite = sprites[1];
    }

    public void ChangeMachines()
    {
        foreach (GameObject machine in machines)
        {
            if (machine.name.Contains("BSODAMachine"))
            {
                machine.transform.GetChild(0).GetComponent<MeshRenderer>().material = GACI_BSODA;
                continue;
            }
            machine.transform.GetChild(0).GetComponent<MeshRenderer>().material = GACI;
        }
    }

    public void SetupCake()
    {
        if (SurpriseReady) return;
        Debug.Log("[Birthday Bash] Exit reached!");
        gc.SetAllCharacters(false);
        gc.EnableExitMode(false);
        StartCoroutine(ExecuteInitCake());

        if (Camera.main != null)
        {
            Camera.main.useOcclusionCulling = false;
        }
        gc.gameObject.AddComponent<DisableSaveSystem>();
        if (InputManager.usingTouch && GameObject.Find("SaveButton")) GameObject.Find("SaveButton").SetActive(false);
    }

    public AudioClip aud_Blow;
    public bool glitchActive;
    public AudioClip aud_GlitchLoop;

    public AudioClip aud_LearnLoop;
    public AudioClip aud_LearnEnd;

    public void Glitch()
    {
        if (glitchActive) return;

        gc.audioDevice.PlayOneShot(this.aud_Blow);
        this.glitchActive = true;
        RenderSettings.ambientLight = Color.gray;
        gc.audioDevice.clip = this.aud_GlitchLoop;
        gc.audioDevice.loop = true;
        gc.audioDevice.Play();
        gc.SetTextWhite();
        StartCoroutine(FloatCharacters());
        //this.filename2.SetActive(true);
        foreach (Balloon balloon in FindObjectsOfType<Balloon>())
        {
            if (!balloon.GetComponentInParent<MathBalloon>()) Destroy(balloon.gameObject, 0f);
        }
        FindObjectOfType<Daylight>().Stop();
        RenderSettings.skybox = FindObjectOfType<Daylight>().night;

        for (int i = 0; i < SurpriseBashStuff.Length; i++)
        {
            SurpriseBashStuff[i].SetActive(true);
        }
    }

    private IEnumerator FloatCharacters()
    {
        GameObject[] array = this.surprises;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].transform.position = new Vector3(UnityEngine.Random.Range(-100f, 25f), 0f, UnityEngine.Random.Range(250f, 340f));
        }
        float height = 0f;
        float speed = 20f;
        Vector3 position = default(Vector3);
        while (height < 40f)
        {
            height += speed * Time.deltaTime;
            Vector3 player = gc.playerTransform.position;
            player.y = height / 2 + 4f;
            gc.player.SetPosition(player);
            foreach (GameObject gameObject in this.surprises)
            {
                position = gameObject.transform.position;
                position.y = height;
                gameObject.transform.position = position;
            }
            yield return null;
        }
        Vector3 play = gc.playerTransform.position;
        play.y = 24f;
        gc.player.SetPosition(play);
        yield break;
    }

    private bool SurpriseReady;
    public GameObject[] surprises;
    public GameObject[] SurpriseBashStuff;

    private IEnumerator ExecuteInitCake()
    {
        FindObjectOfType<RandomEvent>().PauseRandomEvents();
        FindObjectOfType<RandomEvent>().StopEventNOW();
        gc.audioDevice.Stop();
        gc.StopNoise();
        SurpriseReady = true;
        gc.SetAllCharacters(false);
        CaptionsManager CM = FindObjectOfType<CaptionsManager>();
        if (CM != null)
        {
            CM.DestroyAllCaptions(false);
        }
        GameObject[] array = this.surprises;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(true);
        }
        BirthdayBashStuff.SetActive(true);

        GameObject.Find("Buzz").GetComponent<AudioSource>().Stop();
        GameObject.Find("Buzz (1)").GetComponent<AudioSource>().Stop();

        GameObject.Find("ElevatorMusic").GetComponent<AudioSource>().Stop();
        GameObject.Find("ElevatorMusic (1)").GetComponent<AudioSource>().Stop();
        yield break;
    }

    // Token: 0x06000A01 RID: 2561 RVA: 0x00007FE0 File Offset: 0x000061E0
    public void GlitchBook()
    {
        if (this.glitchActive)
        {
            gc.learnMusic.loop = false;
            SecondGlitchActive = true;
            gc.exitCount.text = "5/4 Exits";
            gc.exitCount.color = Color.red;
            base.StartCoroutine(this.PlayGlitchEnd());
        }
    }

    // Token: 0x06000A02 RID: 2562 RVA: 0x00008003 File Offset: 0x00006203
    private IEnumerator PlayGlitchEnd()
    {
        while (gc.learnMusic.isPlaying)
        {
            yield return null;
        }
        gc.learnMusic.clip = aud_LearnEnd;
        gc.learnMusic.Play();
        yield break;
    }
}
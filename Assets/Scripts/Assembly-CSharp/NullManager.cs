using System.Collections;
using System.Collections.Generic;
using SchoolSaves;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NullManager : MonoBehaviour
{
    public static NullManager Instance;

    public NullMusicManager Music;

    public NullScript NULL;

    public GameControllerScript gc;

    public GameObject NULLGame;

    public GameObject Ending;

    public bool BossFight;

    public bool KillAllowance;

    public ProjectileSpawner ProjectileSpawnr;

    public int health;

    private bool Initiated;

    public Material Font;
    public Material RandomColor;

    public TMP_FontAsset Font2;
    public TMP_FontAsset Font3;

    private void Awake()
    {
#if DEBUG
        Debug.Log("NULL MANAGER ACTIVE");
#endif
        Instance = this;
        gc.ActivateSpoopMode();
        gc.mode = "story";
        gc.UpdateNotebookCount();
        gc.aud_AllNotebooks = gc.principalNPC.aud_Delay;
        health = 10;
    }

    public void ChangeItem(GameObject item)
    {
        if (item == null) return;
        int RandomChance = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
        if (RandomChance == 0)
        {
            item.name = "Pickup_Chalk";
            item.GetComponentInChildren<SpriteRenderer>().sprite = gc.itemSprites[23];
        }
        else
        {
            item.name = "Pickup_InvisibleItem";
            item.GetComponentInChildren<SpriteRenderer>().sprite = gc.itemSprites[19];
        }
    }

    private void Start()
    {
        gc.player.SetPosition(new Vector3(5f, 4f, 5f));
        foreach (NotebookScript Notebook in FindObjectsOfType<NotebookScript>())
        {
            if (!Notebook.name.Contains("NoYCTP"))
            {
                Notebook.learningGame = NULLGame;
                Notebook.notebookType = "NULL";
            }
        }
        foreach (EntranceScript Exit in FindObjectsOfType<EntranceScript>())
        {
            Exit.AddComponent<NullExit>().gc = gc;
        }
        ChangeItem(GameObject.Find("Pickup_Apple"));
        ChangeItem(GameObject.Find("Pickup_Key"));
        ChangeItem(GameObject.Find("Pickup_Pencil"));
    }

    private void LateUpdate()
    {
        if (!Initiated)
        {
            Initiated = true;
            GameObject.Find("Characters").SetActive(false);
            GameObject Item = GameObject.Find("Pickup_HammerReflexOrigin");
            if (Item)
            {
                Item.SetActive(false);
            }
        }
        if (Enabled)
        {
            if (NextTint > 0f)
            {
                NextTint -= Time.unscaledDeltaTime;
            }
            Font.SetFloat("_GradientScale", 0.1f);
            RandomColor.SetFloat("_GradientScale", 0.1f);
            for (int i = 0; i < Texts.Length; i++)
            {
                string P = PreviousStrings[i];
                string S = "";
                foreach (char C in P.ToCharArray())
                {
                    if (C != ' ' || C != '\n')
                    {
                        string[] array = new string[51]
                        {
                                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                                "U", "V", "W", "X", "Y", "Z", "a","b","c","d","e","f","g","h","i","","k","l","n","o","p","q","r","s","t","u","v","w","x","y","z"
                        };
                        S += array[Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)array.Length - 1f))];
                    }
                    else
                    {
                        S += C;
                    }
                }
                if (NextTint <= 0f)
                {
                    if (Duration <= 1f && Duration >= 0.5f)
                    {
                        Texts[i].font = Font3;
                        Texts[i].color = Color.black;
                    }
                    else if (Duration > 1f)
                    {
                        Texts[i].font = Font2;
                        Texts[i].color = Random.ColorHSV();
                    }
                }
                Texts[i].UpdateMeshPadding();
                Texts[i].text = S;
            }
            if (NextTint <= 0f)
            {
                // A++;
                // if (A >= 3)
                // {
                //     A = 0;
                //     RenderSettings.ambientLight = Random.ColorHSV();
                // }
                NextTint = 0.03f;
            }
        }
    }

    private void OnDestroy()
    {
        Font.SetFloat("_GradientScale", 10f);
    }

    public void PlayerIncrease(float setSpeed)
    {
        gc.player.walkSpeed += setSpeed;
        gc.player.runSpeed += setSpeed;
    }

    private IEnumerator AfterBossStart()
    {
        yield return new WaitForSecondsRealtime(NULL.Null_Hit.length);
        KillAllowance = false;
        NULL.forcedToStop = true;
        NULL.audioDevice.PlayOneShot(NULL.Null_Regret);
        gc.CM.CreateCustomCaption(NULL.audioDevice, "Null_PreBoss_Start", 8f, 0f);
        NULL.shader.SetColor("_Color", Color.white);
        NULL.shader.SetFloat("_Cutoff", 0.5f);
        yield return new WaitForSecondsRealtime(0.5f);
        Music.InitiateStart();
        yield return new WaitForSecondsRealtime(8.4f - 0.5f);
        KillAllowance = true;
        NULL.forcedToStop = false;
        NULL.targetPlayer = true;
        BossFight = true;
        gc.player.walkSpeed = NULL.PreV3_0_3Changes ? 40f : 36f;
        gc.player.runSpeed = NULL.PreV3_0_3Changes ? 40f : 36f;
#if UNITY_ANDROID
        if (!NullScript.EasyMode)
#endif
        NULL.Speed = NULL.PreV3_0_3Changes ? 36f : 28f;
#if UNITY_ANDROID
        else
            NULL.Speed = NULL.PreV3_0_3Changes ? 24f : 16f;
#endif
        yield break;
    }

    private VertexJitter[] PreviousTexts;
    private string[] PreviousStrings;
    private TMP_Text[] Texts;
    float NextTint = 0f;
    public bool Enabled;
    float Duration;

    public void UpdateVertexValue(float v)
    {
        for (int i = 0; i < PreviousTexts.Length; i++)
        {
            if (PreviousTexts[i] == null) continue;
            PreviousTexts[i].CurveScale = v;
        }
    }

    private IEnumerator ScrambleTexts()
    {
        yield return new WaitForSecondsRealtime(NULL.Null_Hit.length);
        yield return new WaitForSecondsRealtime(7.45f);
        Music.DisableOnBeatText = true;
        Music.SchoolRhythm.GlitchStart = true;
        Music.SchoolRhythm.Intensity = 3.25f;
        Music.SchoolRhythm.Multiplier = 0f;
        Texts = FindObjectsOfType<TextMeshProUGUI>(true);
        PreviousTexts = new VertexJitter[Texts.Length];
        for (int i = 0; i < Texts.Length; i++)
        {
            PreviousTexts[i] = Texts[i].AddComponent<VertexJitter>();
        }
        yield return new WaitForSecondsRealtime(0.918f);
        float Value = 1f;
        while (Value < 120f)
        {
            Value += Time.unscaledDeltaTime * 75f * 1f;
            UpdateVertexValue(Value);
            yield return null;
        }
        yield return new WaitWhile(() => Music.SchoolRhythm.GlitchStart);
        while (Value > 1f)
        {
            Value -= Time.unscaledDeltaTime * 120f * 5f;
            if (Value < 1f) Value = 1f;
            UpdateVertexValue(Value);
            yield return null;
        }
        for (int i = 0; i < PreviousTexts.Length; i++)
        {
            if (PreviousTexts[i] == null) continue;
            PreviousTexts[i].Hang = true;
            Destroy(PreviousTexts[i]); // No longer needed.
        }
        RenderSettings.ambientLight = Color.white;
        gc.warning.text = "ERROR";
        gc.exitCount.text = health + "/10 Hits";
        gc.warning.color = Color.red;
        gc.itemText.text = (gc.HudVersion == "2") ? "Nothing " : " Nothing ";
        Music.DisableOnBeatText = false;
        for (int i = 0; i < Mathf.FloorToInt((health - 1) / 3); i++)
        {
            ProjectileSpawnr.SpawnProjectile();
        }
        yield break;
    }

    private IEnumerator SpeedConstantIncrease()
    {
        float Speed = 17f;
        while (Speed <= 40f)
        {
            Speed += Time.unscaledDeltaTime * 4f;
            gc.player.walkSpeed = Speed;
            gc.player.runSpeed = Speed;
            yield return null;
        }
        gc.player.walkSpeed = 40f;
        gc.player.runSpeed = 40f;
        yield break;
    }

    public void NullHit()
    {
        this.health--;
        if (this.health == 9)
        {
            this.BossFight = false;
            List<GameObject> Projetiles = ProjectileSpawnr.Projectiles;
            foreach (GameObject Projectile in Projetiles)
            {
                if (Projectile != null)
                    Destroy(Projectile);
            }
            ProjectileSpawnr.Projectiles.Clear();
            ProjectileSpawnr.spawnCooldown = 5f;
            ProjectileSpawnr.objects = 0;
            gc.player.heldObject = false;
            gc.player.walkSpeed = 17f;
            gc.player.runSpeed = 17f;
            base.StartCoroutine(this.AfterBossStart());
            base.StartCoroutine(this.SpeedConstantIncrease());
            StartCoroutine(ScrambleTexts());
            return;
        }
        switch (health) // Speed up music?
        {
            case 7:
            case 4:
            case 1:
                Music.IncreaseSpeed();
                break;
        }
        if (this.health == 2)
        {
            gc.exitCount.isRightToLeftText = false;
            gc.itemText.isRightToLeftText = true;
        }
        if (this.health < 9)
        {
            for (int i = 0; i < Mathf.FloorToInt((health - 1) / 3); i++)
            {
                ProjectileSpawnr.SpawnProjectile();
            }
            if (this.BossFight)
            {
                PlayerIncrease(4f);
            }
            //             if (this.health == 1)
            //             {
            // #if UNITY_ANDROID
            //                 if (!NullScript.EasyMode)
            // #endif
            //                     this.ProjectileSpawnr.maxObjects = 1;
            //                 gc.player.holdingObject = false;
            //             }
            if (this.health <= 0)
            {
                Settings.SetInt("NULL_FirstTime", 0);
                Ending.SetActive(true); // Change this to the scene you want to end up at after the fight is over!
            }
        }
    }
}
using System;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class WaterFountainScript : MonoBehaviour
{
    // Token: 0x06000063 RID: 99 RVA: 0x00003C0D File Offset: 0x0000200D
    public WaterFountainScript()
    {
    }

    // Token: 0x06000064 RID: 100 RVA: 0x00003C15 File Offset: 0x00002015
    private void Start()
    {
        playerposition = FindObjectOfType<PlayerScript>().transform;
        player = FindObjectOfType<PlayerScript>();
    }

    // Token: 0x06000065 RID: 101 RVA: 0x00003C18 File Offset: 0x00002018
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool IsHovering = Vector3.Distance(this.playerposition.position, base.transform.position) < 15f & Physics.Raycast(ray, out raycastHit) && (raycastHit.transform == base.transform);
        if (InteractScript.Intracting && Time.timeScale != 0f)
        {

            if (IsHovering & player.stamina < 100f)
            {
                this.player.stamina = player.maxStamina;
                if (Slurp != null)
                {
                    AudioSource source = new GameObject("SLURP").AddComponent<AudioSource>();
                    source.gameObject.AddComponent<AudioSourceSpeed>();
                    source.outputAudioMixerGroup = InputManager.instance != null ? InputManager.instance.ASFX : null;
                    source.PlayOneShotWC(Slurp);
                    Destroy(source.gameObject, Slurp.length);
                }
            }
        }
        if (IsHovering)
        {
            if (clickCursor == null & clickCursorPrefab != null)
            {
                clickCursor = Instantiate(clickCursorPrefab);
                Destroy(clickCursor, 10.25f);
            }
        }
        else
        {
            if (clickCursor != null)
            {
                Destroy(clickCursor);
            }
        }
    }

    private PlayerScript player;

    public GameObject clickCursorPrefab;
    public AudioClip Slurp;

    private GameObject clickCursor;

    private Transform playerposition;
}
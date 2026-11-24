using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000007 RID: 7
public class BullyTreeScript : MonoBehaviour
{
    private float coolDown;

    // Token: 0x0600001C RID: 28 RVA: 0x0000277C File Offset: 0x0000097C
    private void Start()
    {
        coolDown = 60f;
        this.agentSpeed = this.agent.speed;
        StartInStartingPosition();
        StopMoving();
    }

    // Token: 0x0600001D RID: 29 RVA: 0x0000284C File Offset: 0x00000A4C
    private void Update()
    {
        if (coolDown > 0f)
        {
            coolDown -= Time.deltaTime;
        }
        if (this.agent.enabled)
        {
            this.agent.SetDestination(this.player.position);
        }
    }

    // Token: 0x0600001E RID: 30 RVA: 0x00002878 File Offset: 0x00000A78
    private void FixedUpdate()
    {
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) & raycastHit.transform.name == "Player" & this.bullyRenderer.isVisible & !this.revealed)
        {
            this.lookingAt = true;
            StartCoroutine("Sighted");
        }
        else if (!this.revealed & coolDown <= 0f)
        {
            this.lookingAt = false;
            this.StartMoving();
        }
        transform.GetChild(0).gameObject.SetActive(coolDown <= 0f);
    }

    // Token: 0x0600001F RID: 31 RVA: 0x00002934 File Offset: 0x00000B34
    private void StartMoving()
    {
        this.agent.speed = this.agentSpeed;
    }

    // Token: 0x06000020 RID: 32 RVA: 0x00002947 File Offset: 0x00000B47
    private void StopMoving()
    {
        this.agent.speed = 0f;
    }

    private void StartInStartingPosition()
    {
        int randomizer = Mathf.RoundToInt((float)UnityEngine.Random.Range(0f, 3f));
        if (randomizer == 0)
        {
            bullyStartingPosition = bullyStartingPosition1;
        }
        else if (randomizer == 1)
        {
            bullyStartingPosition = bullyStartingPosition2;
        }
        else if (randomizer == 2)
        {
            bullyStartingPosition = bullyStartingPosition3;
        }
        else if (randomizer == 3)
        {
            bullyStartingPosition = bullyStartingPosition4;
        }
        agent.Warp(bullyStartingPosition);
    }

    // Token: 0x06000021 RID: 33 RVA: 0x0000295C File Offset: 0x00000B5C
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Player" & !this.revealed & this.cs.sticks > 0 & coolDown <= 0f)
        {
            this.revealed = true;
            StopAllCoroutines();
            StartCoroutine(Reveal());
        }
    }

    // Token: 0x06000023 RID: 35 RVA: 0x00002A50 File Offset: 0x00000C50
    private IEnumerator Reveal()
    {
        this.StopMoving();
        this.cs.clearSticks();
        this.audioDevice.PlayOneShot(this.giveSticks);
        coolDown = UnityEngine.Random.Range(60f, 90f);
        StartInStartingPosition();
        revealed = false;
        yield break;
    }

    // Token: 0x06000024 RID: 36 RVA: 0x00002A6C File Offset: 0x00000C6C
    private IEnumerator Sighted()
    {
        float i = this.reactTime;
        while (i > 0f)
        {
            i -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.StopMoving();
        yield break;
    }

    // Token: 0x0400002F RID: 47
    public CampingScript cs;

    // Token: 0x04000030 RID: 48
    public Transform player;

    // Token: 0x04000031 RID: 49
    public Transform tree;

    // Token: 0x04000032 RID: 50
    private Vector3 lockdown;

    private Vector3 bullyStartingPosition;

    public Vector3 bullyStartingPosition1;

    public Vector3 bullyStartingPosition2;

    public Vector3 bullyStartingPosition3;

    public Vector3 bullyStartingPosition4;

    // Token: 0x04000033 RID: 51
    public GameObject realBully;

    // Token: 0x04000034 RID: 52
    public GameObject behindBully;

    // Token: 0x04000035 RID: 53
    public Renderer bullyRenderer;

    // Token: 0x04000036 RID: 54
    public NavMeshAgent agent;

    // Token: 0x04000037 RID: 55
    public bool lookingAt;

    // Token: 0x04000038 RID: 56
    public bool revealed;

    // Token: 0x04000039 RID: 57
    public int rangeX;

    // Token: 0x0400003A RID: 58
    public int rangeZ;

    // Token: 0x0400003B RID: 59
    public int minX;

    // Token: 0x0400003C RID: 60
    public int minZ;

    // Token: 0x0400003D RID: 61
    public float separateDistance;

    // Token: 0x0400003E RID: 62
    public float reactTime;

    // Token: 0x0400003F RID: 63
    public float treeFlyHeight;

    // Token: 0x04000040 RID: 64
    public float treeFlySpeed;

    // Token: 0x04000041 RID: 65
    private float agentSpeed;

    // Token: 0x04000042 RID: 66
    public AudioSource audioDevice;

    // Token: 0x04000043 RID: 67
    public AudioClip giveSticks;
}

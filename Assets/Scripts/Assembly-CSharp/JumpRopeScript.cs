using UnityEngine;
using TMPro;
using System.Collections;

// Token: 0x0200000F RID: 15
public class JumpRopeScript : MonoBehaviour
{
    public int jumpsRow;

    public AudioQueueScript audioQueue;

    // Token: 0x0600003C RID: 60 RVA: 0x00003068 File Offset: 0x00001468
    private void OnEnable()
    {
        jumpsRow = Randomize();
        if (ps.gc.HASManager)
        {
            jumpsRow = 1;
        }
        if (jumpsRow == 10)
        {
            this.jumpDelay = 8f;
        }
        else
        {
            this.jumpDelay = 1f;
        }
        this.ropeHit = true;
        this.jumpStarted = false;
        this.jumps = 0;
        this.jumpCount.text = 0 + "/" + jumpsRow;
        this.cs.jumpHeight = 0f;
        if (jumpsRow == 10)
        {
            audioQueue.Queue(playtime.aud_Instrcutions);
            audioQueue.Queue(playtime.aud_ReadyGo);
            this.jumpCount.text += "\nSkip dialogue by pressing " + (InputManager.usingTouch ? "the Interact button" : "Enter") + ".";
        }
        else
        {
            this.playtime.PlaySound_Other(this.playtime.aud_ReadyGo);
        }
    }

    private int Randomize()
    {
        if (UnityEngine.Random.Range(0f, 1f) <= 0.4f)
        {
            return Mathf.RoundToInt((float)Random.Range(1f, 10f));
        }
        return Mathf.RoundToInt((float)Random.Range(2f, 3f));
    }

    // Token: 0x0600003D RID: 61 RVA: 0x000030CC File Offset: 0x000014CC
    private void Update()
    {
        if (jumpDelay > 0f & (Input.GetKeyDown(KeyCode.Return) || InteractScript.Intracting))
        {
            jumpDelay = 0f;
            playtime.audioDevice.Stop();
            if (ps.gc.CM != null && ps.gc.CM.isActiveAndEnabled)
            {
                ps.gc.CM.DestroyAllCaptions(false);
            }
            this.jumpCount.text = this.jumps + "/" + jumpsRow;
        }
        if (this.jumpDelay > 0f)
        {
            this.jumpDelay -= Time.deltaTime;
        }
        else if (!this.jumpStarted)
        {
            this.jumpStarted = true;
            this.ropePosition = 1f;
            this.rope.SetTrigger("ActivateJumpRope");
            this.ropeHit = false;
        }
        if (this.ropePosition > 0f)
        {
            this.ropePosition -= Time.deltaTime;
        }
        else if (!this.ropeHit)
        {
            this.RopeHit();
        }
    }

    // Token: 0x0600003E RID: 62 RVA: 0x0000316C File Offset: 0x0000156C
    private void RopeHit()
    {
        this.ropeHit = true;
        if (this.cs.jumpHeight <= 0.2f)
        {
            this.Fail();
        }
        else
        {
            this.Success();
        }
        this.jumpStarted = false;
    }

    // Token: 0x0600003F RID: 63 RVA: 0x000031A4 File Offset: 0x000015A4
    private void Success()
    {
        this.playtime.audioDevice.Stop();
        this.playtime.PlaySound_Other(this.playtime.aud_Numbers[this.jumps]);
        this.jumps++;
        this.jumpCount.text = this.jumps + "/" + jumpsRow;
        this.jumpDelay = 0.5f;
        if (this.jumps >= jumpsRow)
        {
            this.playtime.audioDevice.Stop();
            float jumpsTotal = (float)jumpsRow - 1f;
            audioQueue.Queue(this.playtime.aud_Numbers[Mathf.RoundToInt(jumpsTotal)]);
            audioQueue.Queue(this.playtime.aud_Congrats);
            this.ps.DeactivateJumpRope();
        }
    }

    // Token: 0x06000040 RID: 64 RVA: 0x00003260 File Offset: 0x00001660
    private void Fail()
    {
        this.jumps = 0;
        this.jumpCount.text = this.jumps + "/" + jumpsRow;
        this.jumpDelay = 7.7f;
        this.playtime.PlaySound_Other(this.playtime.aud_Oops);
        this.jumpCount.text += "\nSkip dialogue by pressing " + (InputManager.usingTouch ? "the Interact button" : "Enter") + ".";
    }

    // Token: 0x04000046 RID: 70
    public TMP_Text jumpCount;

    // Token: 0x04000047 RID: 71
    public Animator rope;

    // Token: 0x04000048 RID: 72
    public CameraScript cs;

    // Token: 0x04000049 RID: 73
    public PlayerScript ps;

    // Token: 0x0400004A RID: 74
    public PlaytimeScript playtime;

    // Token: 0x0400004B RID: 75
    public int jumps;

    // Token: 0x0400004C RID: 76
    public float jumpDelay;

    // Token: 0x0400004D RID: 77
    public float ropePosition;

    // Token: 0x0400004E RID: 78
    public bool ropeHit;

    // Token: 0x0400004F RID: 79
    public bool jumpStarted;
}

using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class BB_SpoopManager : MonoBehaviour
{
    // Token: 0x06000AE7 RID: 2791 RVA: 0x00008994 File Offset: 0x00006B94
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !this.activated)
        {
            this.activated = true;
            base.StartCoroutine(this.Spawner());
            base.StartCoroutine(this.Changer());
        }
    }

    // Token: 0x06000AE8 RID: 2792 RVA: 0x000089D1 File Offset: 0x00006BD1
    private IEnumerator Spawner()
    {
        float time = this.initialTime;
        this.SpawnBalloon();
        for (; ; )
        {
            if (time <= 0f)
            {
                this.SpawnBalloon();
                this.initialTime -= this.inc;
                time = this.initialTime;
                yield return null;
            }
            else
            {
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }

    // Token: 0x06000AE9 RID: 2793 RVA: 0x000089E0 File Offset: 0x00006BE0
    private IEnumerator Changer()
    {
        renderers = FindObjectsOfType<MeshRenderer>();
        float time = 2f;
        int times = 0;
        while (times < 70)
        {
            while (time > 0f)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            this.renderers[UnityEngine.Random.Range(0, this.renderers.Length)].material = this.DSCI_0000;
            time = 2f;
            int num = times;
            times = num + 1;
            if (times >= 69)
            {
                gc.notebookCount.isRightToLeftText = false;
                gc.exitCount.isRightToLeftText = false;
                gc.itemText.isRightToLeftText = false;
                gc.notebookCount.characterSpacing = 0f;
                gc.exitCount.characterSpacing = 0f;
                gc.itemText.characterSpacing = 0f;
                gc.notebookCount.text = "5 YEARS";
                gc.exitCount.text = "OF DEVELOPING";
                gc.itemText.text = "THANK YOU!";
            }
            else if (times >= 50)
            {
                gc.itemText.isRightToLeftText = !gc.itemText.isRightToLeftText;
                gc.itemText.text = "COUR42362PTED";
                gc.notebookCount.isRightToLeftText = !gc.notebookCount.isRightToLeftText;
                gc.exitCount.isRightToLeftText = !gc.exitCount.isRightToLeftText;
                gc.notebookCount.characterSpacing = 3f;
                gc.exitCount.characterSpacing = 3f;
                gc.itemText.characterSpacing = 3f;
                gc.notebookCount.text = "COR" + UnityEngine.Random.Range(1f, 9999f) + "UPTED";
                gc.exitCount.text = "T9273L";
            }
            else if (times >= 40)
            {
                gc.itemText.isRightToLeftText = !gc.itemText.isRightToLeftText;
                gc.itemText.text = "CORRUPTED";
                gc.exitCount.isRightToLeftText = !gc.exitCount.isRightToLeftText;
                gc.exitCount.characterSpacing = 3f;
                gc.notebookCount.text = "9999/9999 Notebooks";
                gc.notebookCount.color = Color.red;
                gc.exitCount.color = Color.magenta;
                gc.exitCount.text = "E RORRR";
            }
            else if (times >= 30)
            {
                gc.exitCount.isRightToLeftText = !gc.exitCount.isRightToLeftText;
                gc.exitCount.characterSpacing = 3f;
                gc.notebookCount.text = "999/999 Notebooks";
                gc.exitCount.color = Color.red;
                gc.exitCount.text = "FAIL";
            }
            else if (times >= 20)
            {
                gc.notebookCount.color = Color.black;
                gc.notebookCount.text = "29/15 Notebooks";
                gc.exitCount.color = Color.yellow;
                gc.exitCount.text = "99/99 Exits";
            }
            else if (times >= 10)
            {
                gc.notebookCount.color = Color.red;
                gc.notebookCount.text = "9/9 Notebooks";
                gc.exitCount.text = "9/9 Exits";
            }
            else
            {
                gc.exitCount.text = "6/4 Exits";
            }
            yield return null;
        }
        AchievementManager.Instance.Earn(Achievements.BirthdayBashEnding);
        GameSystem.AbortApplication("The game data has been corrupted by an unknown source. Please cheCk iF yMDDAAIIWREWN+AHTFUNkaIgwnefIQNFqofwqe3ql1lfwef7laffva1fjaf8\nNEXT NEXT NEXT NEXT NEXT NEXT NEXT BIRHDwfwe BASfsadjH1!", "CORRUPTED_GAME_DATA_DATA_DATA_DATA");
        base.StopAllCoroutines();
        yield break;
    }

    // Token: 0x06000AEA RID: 2794 RVA: 0x00028D24 File Offset: 0x00026F24
    private void SpawnBalloon()
    {
        Vector3 a = default(Vector3);
        a.x = UnityEngine.Random.Range(-1f, 1f);
        a.z = UnityEngine.Random.Range(-1f, 1f);
        a = a.normalized;
        BB_SpoopBalloon spoopBalloon = UnityEngine.Object.Instantiate<BB_SpoopBalloon>(this.balloon, transform);
        spoopBalloon.gameObject.SetActive(true);
        spoopBalloon.transform.position = this.player.transform.position + a * 50f;
        spoopBalloon.player = this.player.transform;
    }

    // Token: 0x0400081B RID: 2075
    public BB_SpoopBalloon balloon;

    // Token: 0x0400081C RID: 2076
    public PlayerScript player;

    // Token: 0x0400081D RID: 2077
    public float initialTime;

    // Token: 0x0400081E RID: 2078
    public float inc;

    // Token: 0x0400081F RID: 2079
    public MeshRenderer[] renderers;

    // Token: 0x04000820 RID: 2080
    public Material DSCI_0000;

    // Token: 0x04000822 RID: 2082
    public GameControllerScript gc;

    // Token: 0x04000823 RID: 2083
    private bool activated;
}

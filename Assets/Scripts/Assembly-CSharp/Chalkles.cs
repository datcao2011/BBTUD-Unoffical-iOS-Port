using UnityEngine;
using System.Collections;

public class Chalkles : MonoBehaviour
{
    public AudioSource aud;

    public Animator anim;

    private bool spawning;

    public AudioClip aud_spawn;

    public AudioClip aud_laugh;

    public DoorScript doorController;

    private bool freedom;

    private void Update()
    {
        if (aud.isPlaying && Time.timeScale == 0f & spawning)
        {
            aud.Pause();
        }
        else if (spawning & !aud.isPlaying & Time.timeScale > 0f)
        {
            aud.UnPause();
        }
    }

    public void OnClassroomEntered(Vector3 position, Quaternion rotation, DoorScript door)
    {
        if (freedom || !isActiveAndEnabled)
        {
            return;
        }
        transform.position = position;
        anim.SetBool("Visible", true);
        transform.rotation = rotation;
        if (FindObjectOfType<GameControllerScript>().notebooks >= 2)
        {
            spawning = true;
            doorController = door;
            StartCoroutine(Spawn());
        }
    }

    public void OnClassroomExit()
    {
        if (freedom || !isActiveAndEnabled)
        {
            return;
        }
        spawning = false;
        aud.Stop();
        anim.SetBool("Visible", false);
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public IEnumerator Spawn()
    {
        aud.PlayOneShotWC(aud_spawn);
        float timer = aud_spawn.length;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        spawning = false;
        freedom = true;
        doorController.LockDoor(10f);
        if (FindObjectOfType<BaldiScript>())
        {
            FindObjectOfType<BaldiScript>().Hear(transform.position, 6f);
        }
        if (FindObjectOfType<PrincipalScript>())
        {
            FindObjectOfType<PrincipalScript>().Hear(transform.position, 6f);
        }
        if (FindObjectOfType<DrReflex>() && FindObjectOfType<DrReflex>().Angry && !FindObjectOfType<DrReflex>().WantsTheHammer)
        {
            FindObjectOfType<DrReflex>().Hear(transform.position); // Hear like Baldi
        }
        aud.Stop();
        aud.clip = aud_laugh;
        aud.loop = true;
        aud.PlayDelayed(1f);
        anim.SetTrigger("Spawn");
        yield return new WaitForSeconds(15f);
        aud.loop = false;
        yield break;
    }
}
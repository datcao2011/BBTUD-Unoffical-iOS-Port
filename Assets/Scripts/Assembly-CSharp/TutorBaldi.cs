using UnityEngine;

public class TutorBaldi : MonoBehaviour
{
    private Animator animator;

    private float time;

    private bool say;
    private bool say_2;

    public bool SaidHi;

    [SerializeField]
    private BaldiTV TV;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (Settings.GetString("CurrentMode") == "hideAndSeekGame")
        {
            animator.SetTrigger("Wave");
            SaidHi = true;
        }
    }

    public void SayHi()
    {
        if (SaidHi || !isActiveAndEnabled) return;
        animator.SetTrigger("Wave");
        GetComponentInParent<AudioSource>().Play();
        if (FindObjectOfType<GameControllerScript>().HookManager != null)
        {
            FindObjectOfType<GameControllerScript>().HookManager.ExcludedObjects[0] = "DOOR_IS_NOW_LOCKED"; // To remove from the exclusion list
        }
    }

    private bool IsPlayingOnTV;

    private void Update()
    {
        if (IsPlayingOnTV)
        {
            animator.SetBool("Talking", true);
            if (!TV.IsPlaying)
            {
                IsPlayingOnTV = false;
                animator.SetBool("Talking", false);
            }
        }
        else if (TV.IsPlaying)
        {
            animator.SetBool("Talking", true);
            IsPlayingOnTV = true;
        }
        else if (say_2)
        {
            time += Time.unscaledDeltaTime * Settings.GetFloat("TimeSpeed");
            if ((time >= 0.01f && time < 1.44f) || (time >= 3.84f && time < 4.10f))
            {
                animator.SetBool("Talking", true);
            }
            else
            {
                animator.SetBool("Talking", false);
            }
        }
        else if (say)
        {
            time += Time.unscaledDeltaTime * Settings.GetFloat("TimeSpeed");
            if ((time >= 2.11f && time < 7.71f) || (time >= 8.97f && time < 23.91f))
            {
                animator.SetBool("Talking", true);
            }
            else
            {
                animator.SetBool("Talking", false);
            }
        }
    }

    public void Prize()
    {
        if (BirthdayBash.Instance != null) return;
        say = true;
        say_2 = false;
    }

    public void Prize_Notebook2()
    {
        if (BirthdayBash.Instance != null) return;
        say = false;
        say_2 = true;
    }
}
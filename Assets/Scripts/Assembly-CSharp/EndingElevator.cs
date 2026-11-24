using UnityEngine;

public class EndingElevator : MonoBehaviour
{
    public Animator Anim;

    public float Timer = 5f;

    public AudioSource Elevator;

    private void Update()
    {
        if (Timer > 0f)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0f)
            {
                Anim.SetTrigger("open");
                Elevator.Play();
            }
        }
    }
}
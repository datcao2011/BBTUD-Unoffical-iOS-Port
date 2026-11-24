using UnityEngine;

public class BB_ButtonManager : MonoBehaviour
{
    private void Update()
    {
        if (Wall.activeSelf && Buttons[0].Num == 1 && Buttons[1].Num == 2 && Buttons[2].Num == 6 && Buttons[3].Num == 3 )
        {
            FindObjectOfType<GameControllerScript>().audioDevice.PlayOneShot(WOW);
            Wall.SetActive(false);
        }
    }

    public GameObject Wall;
    public AudioClip WOW;

    public BB_Button[] Buttons;
}
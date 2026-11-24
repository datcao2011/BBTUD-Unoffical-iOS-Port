using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthWallScript : MonoBehaviour
{
    public AudioSource aud_prifw;
    private void OnTriggerEnter(Collider other)
    {
        if (!aud_prifw.isPlaying)
        {
            aud_prifw.Play();
            AchievementManager.Instance.Earn(Achievements.FourthWall);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "FarmCamping")
            {
                AchievementManager.Instance.Earn(Achievements.FourthWallFarm);
            }
        }
    }
}

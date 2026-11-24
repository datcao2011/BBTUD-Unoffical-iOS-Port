using System.Collections;
using Platforms;
using UnityEngine;


public class GameOptimizer : MonoBehaviour
{
    public GameObject Floor1;
    public GameObject Floor2;
    public GameObject Floor3;
    private Transform player;
    private int floor = 1;

    private void Awake()
    {
        if (!Settings.GetBool("Optimize", OptimizationDefault))
        {
            DestroyImmediate(this);
            return;
        }
        player = FindObjectOfType<CameraScript>().transform;
        floor = 99;
        // Don't think this is necessary.
        // foreach (GameObject go in GameObject.FindGameObjectsWithTag("SchoolCams"))
        // {
        //     go.SetActive(false);
        //     go.GetComponent<Camera>().enabled = false;
        //     Destroy(go);
        // }
        Update();
    }

    public static int CameraOptimizationDefault
    {
        get
        {
#if UNITY_WEBGL || UNITY_ANDROID || UNITY_EDITOR
            return 2;
#else
            return 3;
#endif
        }
    }

    public static bool OptimizationDefault
    {
        get
        {
            bool isOnMobileOrWeb = CurrentDevice.currentPlatform == PFPlatform.Android || CurrentDevice.currentPlatform == PFPlatform.WebGL;
            // SystemInfo has limited functionaility
            bool IsOnOrUnder20 = false;
            bool SpecsNeededOptimization = false;
#if !UNITY_WEBGL
            if (SystemInfo.batteryStatus != BatteryStatus.Unknown)
            {
                IsOnOrUnder20 = SystemInfo.batteryLevel <= 20 && SystemInfo.batteryLevel > 0;
            }

            try
            {
                if (SystemInfo.systemMemorySize <= 10000)
                {
                    SpecsNeededOptimization = true;
                }
                else
                {
                    // operating system check
                    string System = SystemInfo.operatingSystem;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                    if (System.Contains("32bit"))
                    {
                        SpecsNeededOptimization = true;
                    }
                    if (!System.Contains("Windows 10"))
                    {
                        SpecsNeededOptimization = !System.Contains("Windows 11");
                    }
#elif UNITY_STANDALONE_OSX || UNITY_EDTIOR_OSX
                    if (!System.Contains("X"))
                    {
                        SpecsNeededOptimization = true;
                    }
#endif
                }
            }
            catch (System.Exception)
            {

            }
#endif
            return SpecsNeededOptimization || IsOnOrUnder20 || isOnMobileOrWeb;
        }
    }

    public void ForceUpdate(float Y)
    {
        if (BirthdayBash.Instance == null)
        {
            if (Y >= 40 && floor < 3)
            {
                floor = 3;
                Floor1.SetActive(false);
                Floor2.SetActive(false);
                Floor3.SetActive(true);
            }
            else if (Y >= 20 && Y < 40 && floor != 2)
            {
                floor = 2;
                Floor1.SetActive(false);
                Floor2.SetActive(true);
                Floor3.SetActive(false);
            }
            else if (floor > 1 && Y < 20)
            {
                floor = 1;
                Floor1.SetActive(true);
                Floor2.SetActive(false);
                Floor3.SetActive(false);
            }
        }
        else
        {
            Floor1.SetActive(true);
            Floor2.SetActive(true);
            Floor3.SetActive(BirthdayBash.Instance.glitchActive);
        }
    }

    private void Update()
    {
        if (BirthdayBash.Instance == null)
        {
            if (player.position.y >= 40 && floor < 3)
            {
                floor = 3;
                Floor1.SetActive(false);
                Floor2.SetActive(false);
                Floor3.SetActive(true);
            }
            else if (player.position.y >= 20 && player.position.y < 40 && floor != 2)
            {
                floor = 2;
                Floor1.SetActive(false);
                Floor2.SetActive(true);
                Floor3.SetActive(false);
            }
            else if (floor > 1 && player.position.y < 20)
            {
                floor = 1;
                Floor1.SetActive(true);
                Floor2.SetActive(false);
                Floor3.SetActive(false);
            }
        }
        else
        {
            Floor1.SetActive(true);
            Floor2.SetActive(true);
            Floor3.SetActive(BirthdayBash.Instance.glitchActive);
        }
    }
}

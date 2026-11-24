using UnityEngine;
using Platforms;

public class MinimapScript : MonoBehaviour
{
    public Animator miniAnim;

    private bool minimapShown;

    private bool showing;

    private PlatPlayer player;

    private GameControllerScript gc;

    private Camera MinimapCamera;

    private void Awake()
    {
        player = PlatTools.GetPlatformPlayer();
        MinimapCamera = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        MinimapCamera.enabled = false;
        gc = FindObjectOfType<GameControllerScript>();
    }

    private void Update()
    {
        if (gc.HASManager != null)
        {
            return;
        }
        if (player.GetButtonDown("OpenMap") & !showing & Time.timeScale != 0f)
        {
            if (MultiplayerManager.isActive && MultiplayerManager.instance.PauseMovement && !minimapShown) return;
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi) return;
            if (!minimapShown)
            {
                showing = true;
                StartCoroutine(openDelay());
                miniAnim.gameObject.SetActive(true);
                miniAnim.SetTrigger("open");
            }
            else
            {
                showing = true;
                minimapShown = false;
                miniAnim.SetTrigger("close");
                StartCoroutine(close1Delay());
                StartCoroutine(closeDelay());
            }
        }
    }

    private System.Collections.IEnumerator openDelay()
    {
        yield return new WaitForSeconds(0.75f);
        showing = false;
        minimapShown = true;
        MinimapCamera.enabled = true;
        if (!EventAlert && NullManager.Instance != null && !Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault))
        {
            MinimapCamera.useOcclusionCulling = true;
            EventAlert = true;
            InputManager.DisplayEvent("You may see missing floors on your minimap to ensure better performance.", 5f, (ev) =>
            {
                MinimapCamera.useOcclusionCulling = false;
                ev.Disappear();
                InputManager.DisplayEvent("WARNING: You'll receive heavy lag spikes while using your minimap.", Color.red, 5f);
            }, "Show Them Anyway");
        }
        if (Settings.GetBool("1.4Shaders"))
        {
            MinimapCamera.transform.position = new Vector3(-999f, 7.6f, 999f);
        }
#if UNITY_ANDROID
        if (!EventAlert && Settings.GetBool("1.4Shaders"))
        {
            EventAlert = true;
            InputManager.DisplayEvent("V1.4 Shaders have broken your minimap. If you want to continue using it, you'll have to disable the shaders.", Color.red, 10f);
        }
#endif
        yield break;
    }

    private bool EventAlert;

    private System.Collections.IEnumerator close1Delay()
    {
        yield return new WaitForSeconds(0.82f);
        showing = false;
        MinimapCamera.enabled = false;
        yield break;
    }

    private System.Collections.IEnumerator closeDelay()
    {
        yield return new WaitForSeconds(0.82f);
        miniAnim.gameObject.SetActive(false);
        MinimapCamera.enabled = false;
        yield break;
    }
}
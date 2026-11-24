using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

// Token: 0x0200001E RID: 30
public class ExitTriggerScript : MonoBehaviour
{
    public SwingingDoorScript CoinDoor;

    // Token: 0x0600007F RID: 127 RVA: 0x0000432C File Offset: 0x0000272C
    private void OnTriggerEnter(Collider other)
    {
        if (this.gc.notebooks >= gc.maxNotebooks && other.tag == "Player" && gc.mode == "story" && gc.exitsReached >= 3 && NullManager.Instance == null && !GetComponentInParent<EntranceScript>().exitTaken)
        {
            gc.exitsReached = 4;
            gc.UpdateExitCount();
            if (BirthdayBash.Instance != null)
            {
                BirthdayBash.Instance.SetupCake();
                return;
            }
            if (!(MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)) AchievementManager.Instance.Earn(Achievements.Escape);
            if (MultiplayerManager.isActive)
            {
                other.enabled = false;
                Multiplayer.Client.DisconnectAction = ((reason, cause) =>
                {
                    Debug.LogException(reason);
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
                });
                if (!MultiplayerManager.instance.isBaldi)
                {
                    MultiplayerManager.instance.client.Escape();
                    if (!MultiplayerManager.instance.HasMoreThan2Players)
                    {
                        Multiplayer.ClientSend.SendActionToOtherPlayer("end");
                        InputManager.DisplayEvent("You escaped! Game over.", 20f);
                        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
                    }
                }
                return;
            }
            CheckEnding();
        }
    }

    private void CheckEnding()
    {
        if (NullManager.Instance != null) return;
        // Pause Random events
        gc.cheatingConsole.schoolEventSystem.StopEventNOW();
        gc.cheatingConsole.schoolEventSystem.PauseRandomEvents();
        gc.AddComponent<DisableSaveSystem>();
        string Challenge = Settings.GetString("ModeType");
        if (CoinDoor != null && CoinDoor.coinDoor)
        {
            gc.bEnding.SetActive(true);
        }
        else if (Challenge == "Stealthy" || Challenge == "Grapple" || Challenge == "Speedy")
        {
            gc.challengeEnding.SetActive(true);
        }
        else if (this.gc.failedNotebooks >= gc.maxNotebooks)
        {
            if (gc.schoolMusic.isPlaying)
                gc.schoolMusic.Stop();
            if (CheatScript.cheated)
            {
                gc.cheatedEnding.SetActive(true);
            }
            else
            {
                gc.gEnding.SetActive(true);
            }
        }
        else if (!gc.spoopMode)
        {
            gc.goodEnding.SetActive(true);
        }
        else
        {
            gc.badEnding.SetActive(true);
        }
    }

    // Token: 0x040000AA RID: 170
    public GameControllerScript gc;
}

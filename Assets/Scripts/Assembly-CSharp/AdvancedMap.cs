using UnityEngine;
using Platforms;
using TMPro;

public class AdvancedMap : MonoBehaviour
{
    private bool advancedOn;

    public Transform miniCamera;

    public GameObject m;

    public TMP_Text text;

    private PlatPlayer platInput;

    private void Start()
    {
        platInput = PlatTools.GetPlatformPlayer();
    }

    private void Update()
    {
        if (platInput.GetButtonDown("OpenMap"))
        {
            advancedOn = false;
            if (MultiplayerManager.isActive)
            {
                MultiplayerManager.instance.PauseMovement = false;
            }
        }
        if (platInput.GetButtonDown("AdvancedMap"))
        {
            if (MultiplayerManager.isActive && MultiplayerManager.instance.PauseMovement) return;
            advancedOn = !advancedOn;
            if (MultiplayerManager.isActive)
            {
                MultiplayerManager.instance.PauseMovement = advancedOn;
            }
        }
        if (advancedOn)
        {
            m.SetActive(true);
            if (InputManager.usingTouch)
                text.text = "You can now move. Press the map to turn off advanced mode.";
            else
                text.text = "You can now move. Press " + platInput.GetKeyName("AdvancedMap") + " to turn off advanced mode.";
            if (!MultiplayerManager.isActive) Time.timeScale = 0f;
            Vector3 forward = Vector3.zero;
            Vector3 strafe = Vector3.zero;
            float speed = 1.5f;
            forward = miniCamera.GetChild(0).forward * InputManager.Forward(false) * 0.5f;
            strafe = miniCamera.GetChild(0).right * InputManager.Strafe(false) * 0.5f;
            if (platInput.GetButton("Run"))
            {
                speed = 5f;
            }
            miniCamera.position += (forward + strafe) * speed;
            float xvalue;
            xvalue = InputManager.TurnX(false) * 0.8f;
            miniCamera.localEulerAngles += new Vector3(0f, 0f, -xvalue * Settings.GetFloat("TurnSensitivity", 1f));
        }
        else
        {
            if (InputManager.usingTouch)
                text.text = "Press the map to turn on advanced mode.";
            else
                text.text = "Press " + platInput.GetKeyName("AdvancedMap") + " to turn on advanced mode.";
            miniCamera.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 70f, Camera.main.transform.position.z);
            miniCamera.eulerAngles = new Vector3(90f, 0f, -Camera.main.transform.eulerAngles.y);
            m.SetActive(false);
        }
    }

    public void MapPressed()
    {
        if (InputManager.usingTouch)
            advancedOn = !advancedOn;
    }
}
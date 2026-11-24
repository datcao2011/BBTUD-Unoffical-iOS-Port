using Platforms;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    private PlatPlayer input;

    private CharacterController cc;


    private float rotationy;

    private void Start()
    {
        Time.timeScale = 1f;
        cc = GetComponent<CharacterController>();
        input = PlatTools.GetPlatformPlayer();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Vector3 x = transform.right * input.GetAxis("Strafe");
        Vector3 y = transform.forward * input.GetAxis("Forward");
        cc.Move((x + y).normalized * 10f * Time.deltaTime);
        transform.eulerAngles += new Vector3(0f, input.GetAxis("Look") * 350f * Time.deltaTime, 0f);

        rotationy -= input.GetAxis("LookUp") * 350f * Time.deltaTime;
        rotationy = Mathf.Clamp(rotationy, -90f, 90f);
        Camera.main.transform.eulerAngles = new Vector3(rotationy, transform.eulerAngles.y, 0f);
    }
}

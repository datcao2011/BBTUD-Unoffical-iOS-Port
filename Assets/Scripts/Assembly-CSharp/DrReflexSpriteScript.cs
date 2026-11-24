// FirstPrizeSpriteScript
using UnityEngine;

public class DrReflexSpriteScript : MonoBehaviour
{
    public float debug;

    public int angle;

    public float angleF;

    public int maxSpriteLength = 8;

    public Transform cam;

    public Transform body;

    public GameObject[] sprites = new GameObject[8];

    private void OnEnable()
    {
        Update();
    }

    private void Update()
    {
        Vector3 position = cam.position;
        float z = position.z;
        Vector3 position2 = body.position;
        float y = z - position2.z;
        Vector3 position3 = cam.position;
        float x = position3.x;
        Vector3 position4 = body.position;
        angleF = Mathf.Atan2(y, x - position4.x) * 57.29578f;
        if (angleF < 0f)
        {
            angleF += 360f;
        }
        Vector3 eulerAngles = body.eulerAngles;
        debug = eulerAngles.y;
        float num = angleF;
        Vector3 eulerAngles2 = body.eulerAngles;
        angleF = num + eulerAngles2.y;
        angle = Mathf.RoundToInt(angleF / 45f);
        while (true)
        {
            if (angle >= 0 && angle < maxSpriteLength)
            {
                break;
            }
            angle += (int)(-(float)maxSpriteLength * Mathf.Sign((float)angle));
        }
        if (!sprites[angle].activeSelf) SetActiveSprite(angle);
    }

    private void SetActiveSprite(int angle)
    {
        sprites[angle].SetActive(true);
        for (int i = 0; i < maxSpriteLength; i++)
        {
            if (i != angle) sprites[i].SetActive(false);
        }
    }
}
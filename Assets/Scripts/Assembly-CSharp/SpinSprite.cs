using UnityEngine;

public class SpinSprite : MonoBehaviour
{
    public float speed = 10f;

    private void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}

using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 10f;

    float currentSpeed;

    public bool TeleportPlayer = true;

    private void Update()
    {
        currentSpeed = speed;
        if (TeleportPlayer)
        {
            if (playe)
            {
                playe.transform.position = transform.GetChild(3).transform.position;
            }
        }
        transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TeleportPlayer)
        {
            if (other.name == "Player" & !other.isTrigger)
            {
                playe = other.transform;
                StartCoroutine(playerTimer());
            }
        }
    }

    Transform playe;

    System.Collections.IEnumerator playerTimer()
    {
        yield return new WaitForSeconds(10f);
        if (TeleportPlayer)
        {
            playe.position = new Vector3(-65f, 4f, -61.14f);
            playe = null;
        }
        yield break;
    }
}

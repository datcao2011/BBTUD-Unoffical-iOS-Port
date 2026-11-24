using UnityEngine;

public class CC_Trigger : MonoBehaviour
{
    private CloudyCopter cloudyCopter;
    public Wind WindManager;
    private float offset;

    private void Awake()
    {
        cloudyCopter = FindObjectOfType<CloudyCopter>();
        if (MultiplayerManager.isActive) { gameObject.SetActive(false); return; }
    }

    private void Update()
    {
        if (BirthdayBash.Instance != null) { gameObject.SetActive(false); return; }
        if (cloudyCopter != null && cloudyCopter.isBlowing && cloudyCopter.cc_Trigger == this)
        {
            offset += Time.deltaTime * 0.75f;
            WindManager.SetYOffset(offset);
        }
        else
        {
            offset = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Cloudy Copter" && cloudyCopter.findingAreaToBlow && cloudyCopter.area == transform.position)
        {
            cloudyCopter.cc_Trigger = this;
            if (!cloudyCopter.IsFlatten) WindManager.SetEnabled(true);
            cloudyCopter.Blow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Cloudy Copter" && cloudyCopter.isBlowing && cloudyCopter.cc_Trigger == this)
        {
            cloudyCopter.Halt();
            cloudyCopter.FleeTo(cloudyCopter.spawnArea);
        }
    }
}
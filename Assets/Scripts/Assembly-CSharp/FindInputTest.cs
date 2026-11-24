using UnityEngine;

public class FindInputTest : MonoBehaviour
{
    Event keyEvent;

    public KeyCode key;

    private void OnGUI()
    {
        keyEvent = Event.current;
        if (keyEvent.isKey)
        {
            key = keyEvent.keyCode;
        }
    }
}
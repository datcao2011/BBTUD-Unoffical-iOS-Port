// DetentionTextScript
using UnityEngine;

using TMPro;

public class DetentionTextScript : MonoBehaviour
{
    public DoorScript door;

    private TMP_Text text;
    private GameControllerScript gc;

    private void Start()
    {
        gc = FindObjectOfType<GameControllerScript>();
        text = base.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if ((gc.scene == InputManager.IsSchoolScene()) && door.lockTime > 0f)
        {
            text.text = "You have detention! \n" + Mathf.CeilToInt(door.lockTime) + " seconds remain!";
        }
        else if (text != null)
        {
            text.text = string.Empty;
        }
    }
}
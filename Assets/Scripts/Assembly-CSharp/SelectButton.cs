using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Platforms;

public class SelectButton : MonoBehaviour
{
    public GameObject selectButton;

    private Button button;

    public GameObject cancelButton;

    private void Start()
    {
        button = base.GetComponent<Button>();
        button.onClick.AddListener(TakeAction);
    }

    private void Update()
    {

    }

    private void TakeAction()
    {

    }
}
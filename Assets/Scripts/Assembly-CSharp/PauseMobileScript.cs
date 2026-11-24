using UnityEngine.UI;
using UnityEngine;

public class PauseMobileScript : MonoBehaviour
{
    private Image image;
    public Color imageColor = Color.black;

    public GameControllerScript gc;

    public Sprite play;

    public Sprite pause;

    private void Start()
    {
        gc = FindObjectOfType<GameControllerScript>();
        image = GetComponent<Image>();
        image.color = imageColor;
    }

    private void Update()
    {
        if (gc.gamePaused)
        {
            image.sprite = play;
        }
        else
        {
            image.sprite = pause;
        }
    }


    public void Pause()
    {
        if (!gc.gamePaused)
        {
            gc.PauseGame();
        }
        else
        {
            gc.UnpauseGame();
        }
    }
}

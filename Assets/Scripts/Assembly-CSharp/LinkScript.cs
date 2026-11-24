
using UnityEngine;

public class LinkScript : MonoBehaviour
{

    public GameControllerScript gc;

    public GameObject white;

    public ApplicationConfigure app;

    public void Update()
    {
    }

    public void clickLink()
    {
        app.openUrlTo("https://swagames.wixsite.com/site/secretdoor?rf=application%3A%2F%2FBaldi%27s+Basics+TUD.exe");
        white.SetActive(false);
        gc.UnpauseGame();
    }
}

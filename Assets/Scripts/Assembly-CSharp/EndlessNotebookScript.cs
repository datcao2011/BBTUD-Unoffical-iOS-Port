// EndlessNotebookScript
using UnityEngine;

public class EndlessNotebookScript : MonoBehaviour
{
    public float openingDistance;

    public GameControllerScript gc;

    public Transform player;

    public GameObject clickCursor;

    public GameObject learningGame;

    public GameObject learningGameCheat;

    private void Start()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameControllerScript>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if (InteractScript.Intracting)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.tag == "Notebook" & Vector3.Distance(player.position, base.transform.position) < openingDistance))
            {

                base.gameObject.SetActive(false);
                gc.CollectNotebook();
                if (gc.cheatConsoleActive)
                {
                    learningGameCheat.SetActive(true);
                }
                else
                {
                    learningGame.SetActive(true);
                }
            }
        }
        Ray rray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit rraycastHit;
        if (Physics.Raycast(rray, out rraycastHit) && (rraycastHit.transform.tag == "Notebook" & Vector3.Distance(player.position, base.transform.position) < openingDistance))
        {
            clickCursor.SetActive(true);
        }
        else
        {
            clickCursor.SetActive(false);
        }
    }
}
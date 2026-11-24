using UnityEngine;

public class EndingNotebook : MonoBehaviour
{
    public Transform player;

    public Sprite[] NotebooksSprites;

    public GameObject clickCursorPrefab;

    private GameObject clickCursor;

    private bool[] Done = new bool[7];

    private int Max;

    public GameObject _1;
    public GameObject _2;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
        RaycastHit raycastHit;
        bool IsHovering = Physics.Raycast(ray, out raycastHit) && raycastHit.transform.name == "ENDING_NOTEBOOKPLACER" && Vector3.Distance(this.player.position, raycastHit.transform.position) < 10f;
        if (Time.timeScale != 0f && InteractScript.Intracting)
        {
            if (IsHovering)
            {
                raycastHit.transform.name = "ENDING_NOTEBOOKPLACED";
                int R = Random.Range(0, NotebooksSprites.Length);
                while (Done[R])
                {
                    R = Random.Range(0, NotebooksSprites.Length);
                }
                if (Done[0] && Done[1] && Done[2] && Done[3] && Done[4] && Done[5] && Done[6])
                {
                    Done = new bool[7];
                }
                else
                {
                    Done[R] = true;
                }
                Max++;
                if (Max >= 7)
                {
                    _1.SetActive(true);
                    _2.SetActive(false);
                }
                raycastHit.transform.GetComponentInChildren<SpriteRenderer>().sprite = NotebooksSprites[R];
            }
        }
        if (IsHovering)
        {
            if (clickCursor == null & clickCursorPrefab != null)
            {
                clickCursor = Instantiate(clickCursorPrefab);
                Destroy(clickCursor, 10.25f);
            }
        }
        else
        {
            if (clickCursor != null)
            {
                Destroy(clickCursor);
            }
        }
    }
}
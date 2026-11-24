using UnityEngine;

public class FarmGenerator : MonoBehaviour
{
    public GameObject[] farmMazes;

    public GameObject maze;

    private bool generating = true;

    public int MazeID;

    private void Awake()
    {
        GameObject.Find("FarmingTime").GetComponent<TMPro.TMP_Text>().text = "Generating...";
        if (maze != null)
        {
            return;
        }
        int random = Mathf.RoundToInt(Random.Range(0f, farmMazes.Length - 1f));

        maze = Instantiate(farmMazes[random]);


        if (Settings.GetInt("LastFarmMaze", 105) == random)
        {
            Generate();
            return;
        }

        Settings.SetInt("LastFarmMaze", random);
        GameObject.Find("FarmingTime").GetComponent<TMPro.TMP_Text>().text = (Settings.HasKey("Account") && Settings.GetBool("AllowLeaderboardStats", true)) ? ("Maze #" + (random + 1)) : "Time: 123";
        MazeID = random + 1;
        generating = false;
    }

    private void Update()
    {
        if (generating)
        {
            if (!MultiplayerManager.isActive) Time.timeScale = 0f;
        }
    }

    public void Generate()
    {
        if (maze != null)
        {
            Object.Destroy(maze, 0f);
        }
        int random = Random.Range(0, farmMazes.Length);

        maze = Instantiate(farmMazes[random]);

        if (Settings.GetInt("LastFarmMaze", 105) == random)
        {
            Generate();
            return;
        }

        Settings.SetInt("LastFarmMaze", random);
        GameObject.Find("FarmingTime").GetComponent<TMPro.TMP_Text>().text = (Settings.HasKey("Account") && Settings.GetBool("AllowLeaderboardStats", true)) ? ("Maze #" + (random + 1)) : "Time: 123";
        MazeID = random + 1;
        generating = false;
    }
}
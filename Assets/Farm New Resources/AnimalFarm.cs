using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimalFarm : MonoBehaviour
{
    public AnimalType ApprovedAnimal;
    public GameObject AnimalTemplate;
    public Transform AnimalsParent;
    private GameObject[] animals;
    private GameObject[] ApprovedAnimals;
    private int AnimalCaught;
    private int AnimalLength;
    public int score;
    public int lives = 3;
    public Sprite[] animalSprites;
    public GameObject RewardsScreen;
    public Animator anim;
    public Image animalImg;
    public AudioSource audioDevice;
    public AudioQueueScript audioQueue;
    public AudioClip aud_intro;
    public AudioClip aud_introShorten;
    public AudioClip aud_bring;
    public AudioClip aud_correct;
    public AudioClip aud_scream;
    public AudioClip[] aud_praises;
    public AudioClip[] aud_animals = new AudioClip[4];
    public AudioClip aud_oh;
    public AudioClip aud_tryagain;
    public AudioClip aud_win;
    public TMP_Text animalText;
    public float timer;
    public float ShowTimer;
    public int round;
    public int maxRounds = 3;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text timerText;
    private bool timerCounting;

    private Vector3 position;

    // Start is called before the first frame update
    private void Start()
    {
        bool IsFirstTime = !Settings.HasKey("AnimalTips_HasShown") || FindObjectOfType<AnimalTips>();
        audioQueue.Queue(IsFirstTime ? aud_intro : aud_introShorten);
        SpawnNewAnimals();
        position = FindObjectOfType<PlayerScript>().transform.position;
    }

    public bool OnTap()
    {
        if (!Input.touchSupported)
        {
            return false;
        }
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).phase == TouchPhase.Began;
        }
        return false;
    }



    private void Update()
    {
        if (timer > 0f)
        {
            timerCounting = true;
            timer -= Time.deltaTime;

            if (ShowTimer > 0f)
            {
                ShowTimer -= Time.deltaTime;
            }
            else
            {
                ShowTimer = 30f;
                FindObjectOfType<DoorIncidator>().GetComponent<SpriteRenderer>().sprite = animalImg.sprite;
                GameObject animal = null;
                foreach (GameObject anim in ApprovedAnimals)
                {
                    if (anim != null) { animal = anim; break; }
                }
                if (animal != null)
                {
                    Vector3 pos = animal.transform.position;
                    FindObjectOfType<DoorIncidator>().transform.parent = animal.transform;
                    FindObjectOfType<DoorIncidator>().Show(pos);
                    animal.GetComponent<AudioSource>().PlayOneShotWC(aud_animals[(int)ApprovedAnimal]);
                }

            }
        }
        else if (timerCounting)
        {
            timerCounting = false;
            InputManager.DisplayEvent("You were out of time. Better be fast next time!", 5f);
            FindObjectOfType<PlayerScript>().transform.position = position;
            Lose();
        }
        anim.SetBool("BaldiCampTalking", audioDevice.isPlaying);
        timerText.text = GetTimerText();
        if (RewardsScreen.activeSelf)
        {
            Time.timeScale = 0f;
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Cancel") || OnTap())
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Starter");
            }
        }
    }

    private string GetTimerText()
    {
        float timeMinutes = Mathf.Floor(timer / 60);
        float timeSeconds = (float)Mathf.FloorToInt(timer % 60);
        string result;
        string minutes;
        string seconds;
        float currentMinutes = timeMinutes;
        float currentSeconds = timeSeconds;
        if (currentSeconds < 0f)
        {
            return "00:00";
        }
        if (currentSeconds > 59f)
        {
            currentSeconds = 0f;
            currentMinutes += 1f;
        }
        if (currentMinutes < 10f)
        {
            minutes = "0" + currentMinutes;
        }
        else
        {
            minutes = currentMinutes.ToString();
        }
        if (currentSeconds < 10f)
        {
            seconds = "0" + currentSeconds;
        }
        else
        {
            seconds = currentSeconds.ToString();
        }
        result = minutes + ":" + seconds;
        return result;
    }

    public void SpawnNewAnimals()
    {
        if (AnimalLength > 0) DestroyAnimals();
        FindObjectOfType<PlayerScript>().stamina = FindObjectOfType<PlayerScript>().maxStamina;
        ApprovedAnimal = (AnimalType)Mathf.RoundToInt(Random.Range(0f, 3f));
        ShowTimer = 45f;
        AnimalCaught = 0;
        AnimalLength = Mathf.RoundToInt(Random.Range(3f, 8f));
        timer = AnimalLength * 40f;
        AnimalTemplate.SetActive(true);
        animalImg.sprite = animalSprites[(int)ApprovedAnimal];
        animalText.text = ApprovedAnimal.ToString() + " [" + AnimalLength + "]";
        ApprovedAnimals = new GameObject[AnimalLength];
        audioQueue.Queue(aud_bring);
        audioQueue.Queue(aud_animals[(int)ApprovedAnimal]);
        Vector3 position = AnimalTemplate.transform.position;
        for (int i = 0; i < AnimalLength; i++)
        {
            GameObject gameObject = Instantiate(AnimalTemplate, position, AnimalTemplate.transform.rotation);
            Animal animal = gameObject.GetComponent<Animal>();
            animal.id = i;
            animal.type = ApprovedAnimal;
            gameObject.name = ApprovedAnimal.ToString();
            animal.spriteRenderer.sprite = animalSprites[(int)ApprovedAnimal];
            animal.spriteRenderer.flipX = animal.type == AnimalType.Chicken ? false : true;
            animal.transform.SetParent(AnimalsParent);
            ApprovedAnimals[i] = gameObject;
        }

        animals = new GameObject[Random.Range(10, 25)];
        for (int i = 0; i < animals.Length; i++)
        {
            GameObject gameObject = Instantiate(AnimalTemplate, position, AnimalTemplate.transform.rotation);
            Animal animal = gameObject.GetComponent<Animal>();
            AnimalType type = (AnimalType)Mathf.RoundToInt(Random.Range(0f, 3f));
            while (type == ApprovedAnimal)
            {
                type = (AnimalType)Mathf.RoundToInt(Random.Range(0f, 3f));
            }
            animal.type = type;
            gameObject.name = type.ToString();
            animal.spriteRenderer.sprite = animalSprites[(int)animal.type];
            animal.spriteRenderer.flipX = animal.type == AnimalType.Chicken ? false : true;
            animal.transform.SetParent(AnimalsParent);
            animals[i] = gameObject;
        }
        AnimalTemplate.SetActive(false);
    }

    public void DestroyAnimals()
    {
        FindObjectOfType<DoorIncidator>().Hide();
        FindObjectOfType<DoorIncidator>().transform.parent = null;
        for (int i = 0; i < animals.Length; i++)
        {
            Destroy(animals[i]);
        }
        for (int i = 0; i < ApprovedAnimals.Length; i++)
        {
            if (ApprovedAnimals[i] != null) Destroy(ApprovedAnimals[i]);
        }
    }

    public bool IsAcceptedAnimal(Animal animal)
    {
        if (ApprovedAnimal == animal.type)
        {
            AnimalCaught++;
            ApprovedAnimals[animal.id] = null;
            animalText.text = ApprovedAnimal.ToString() + " [" + (AnimalLength - AnimalCaught) + "]";
            score += Mathf.RoundToInt(Random.Range((float)AnimalCaught * 4f, (float)AnimalCaught * Random.Range(50f, 125f)));
            scoreText.text = "Score:\n" + score;
            audioDevice.PlayOneShot(aud_correct);

            if (AnimalCaught >= AnimalLength)
            {
                round++;
                if (round >= maxRounds)
                {
                    audioQueue.Queue(aud_win);
                    RewardsScreen.SetActive(true);
                    return true;
                }
                SpawnNewAnimals();
            }
            audioDevice.PlayOneShotWC(aud_praises[Mathf.RoundToInt(Random.Range(0f, aud_praises.Length - 1f))]);
            return true;
        }
        InputManager.DisplayEvent("You tried to put another type of animal. Too bad that we only allow a certain type of animal.", 5f);
        Lose();
        return false;
    }

    public void Lose()
    {
        lives--;
        livesText.text = lives == 1 ? (lives + " Life") : (lives + " Lives");
        audioDevice.PlayOneShot(aud_scream);
        audioDevice.PlayOneShotWC(aud_oh);
        if (lives > 0)
        {
            audioQueue.Queue(aud_tryagain);
            SpawnNewAnimals();
        }
        else
        {
            RewardsScreen.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Multiplayer;
using System.Linq;
using UnityEngine.SceneManagement;

public class SpectatingHud : MonoBehaviour
{
    public TMP_Text Notebooks;
    public TMP_Text Exits;
    public GameControllerScript gc;

    public int CurrentlySpectating;
    public int CurrentlySpectating_Index;

    public TMP_Text SpectatingPerson;

    public GameObject FocusedGameObject;
    public GameObject ThirdPersonArrows;
    public GameObject PauseButton;
    private AnimationOnWalk PlayerSlotManager;

    public RawImage ItemSlot;
    public Button PerspectiveButton;
    public TMP_Text ItemText;

    public bool FirstPerson;
    public float SpectatingAngle;

    // Start is called before the first frame update
    private void Awake()
    {
        PauseButton.SetActive(InputManager.usingTouch);
        gc.player.SetPosition(new Vector3(-999, 4, -999));
        gc.Can_SwitchItemSlots = false;
        gc.Can_UseItems = false;
        SpectateAvailablePerson();
        Pomp pomp = FindObjectOfType<Pomp>();
        if (pomp)
        {
            pomp.angry = false;
            gc.player.onClassroom = true;
        }
    }

    private void SpectateAvailablePerson()
    {
        if (PlayerSlotManager != null)
        {
            PlayerSlotManager.anim.gameObject.SetActive(true);
        }
        Player[] CapturedPlayers = GetUncapturedPlayers();
        if (CapturedPlayers.Length <= 0) // No spectators left
        {
            SceneManager.LoadScene("Starter");
            return;
        }
        Player UncapturedFirstPlayer = CapturedPlayers[0];
        int Spectate = UncapturedFirstPlayer.ActorNumber;
        CurrentlySpectating_Index = 0;
        CurrentlySpectating = Spectate;
        UpdateSpectatingDetails();
    }

    private void UpdateSpectatingDetails()
    {
        try
        {
            gc.player.SetPosition(new Vector3(-999, 4, -999));
        }
        catch { }
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(CurrentlySpectating);
        if (player == null)
        {
            SpectateAvailablePerson();
            return;
        }
        SpectatingPerson.text = player.NickName;
        if (MultiplayerManager.instance.client.CurrentActor_Baldi == player.ActorNumber)
        {
            FocusedGameObject = GameObject.Find("Baldi");
            PlayerSlotManager = null;
        }
        else
        {
            FocusedGameObject = GameObject.Find("EMP_" + player.UserId);
            if (FocusedGameObject != null)
            {
                PlayerSlotManager = FocusedGameObject.GetComponent<AnimationOnWalk>();
            }
        }
    }

    public Player[] GetUncapturedPlayers() // also includes baldi
    {
        if (!PhotonNetwork.IsConnected)
        {
            return new Player[0];
        }
        List<Player> result = new List<Player>();
        Player[] Players = PhotonNetwork.PlayerListOthers;
        for (int i = 0; i < Players.Length; i++)
        {
            Player player = Players[i];
            if (MultiplayerManager.instance.client.CapturedPeople.ContainsKey(player.ActorNumber) && !MultiplayerManager.instance.client.CapturedPeople[player.ActorNumber])
            {
                result.Add(player);
            }
        }
        return result.ToArray();
    }

    public void SpectateBack()
    {
        if (PlayerSlotManager != null)
        {
            PlayerSlotManager.anim.gameObject.SetActive(true);
        }
        Player[] Players = GetUncapturedPlayers();
        CurrentlySpectating_Index--;
        if (CurrentlySpectating_Index < 0)
        {
            CurrentlySpectating_Index = Players.Length - 1;
        }
        CurrentlySpectating = Players[CurrentlySpectating_Index].ActorNumber;
        UpdateSpectatingDetails();
    }

    public void SpectateNext()
    {
        if (PlayerSlotManager != null)
        {
            PlayerSlotManager.anim.gameObject.SetActive(true);
        }
        Player[] Players = GetUncapturedPlayers();
        CurrentlySpectating_Index++;
        if (CurrentlySpectating_Index >= Players.Length)
        {
            CurrentlySpectating_Index = 0;
        }
        CurrentlySpectating = Players[CurrentlySpectating_Index].ActorNumber;
        UpdateSpectatingDetails();
    }

    public void SetAngle(float Angle)
    {
        SpectatingAngle = Angle;
    }

    // Update is called once per frame
    private void Update()
    {
        Notebooks.text = gc.notebooks + "/" + gc.maxNotebooks + " Notebooks";
        Exits.text = gc.exitsReached + "/4 Exits";
        if (gc.notebooks >= gc.maxNotebooks)
        {
            Exits.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FirstPerson = true;
            PerspectiveButton.GetComponentInChildren<TMP_Text>().text = FirstPerson ? "1" : "3";
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FirstPerson = false;
            PerspectiveButton.GetComponentInChildren<TMP_Text>().text = FirstPerson ? "1" : "3";
        }
        ThirdPersonArrows.SetActive(!FirstPerson);
    }

    public void ToggleFirstPerson()
    {
        FirstPerson = !FirstPerson;
        PerspectiveButton.GetComponentInChildren<TMP_Text>().text = FirstPerson ? "1" : "3";
    }

    private void FixedUpdate()
    {
        if (FocusedGameObject == null) // Could be a person leaving or getting captured
        {
            SpectateAvailablePerson();
        }
        else if (PlayerSlotManager == null)
        {
            ItemSlot.texture = gc.itemTextures[0];
            ItemText.text = "";
        }
        else
        {
            ItemSlot.texture = gc.itemTextures[PlayerSlotManager.CurrentItemSlot];
            ItemText.text = gc.GetItemNames()[PlayerSlotManager.CurrentItemSlot];
            PlayerSlotManager.anim.gameObject.SetActive(!FirstPerson);
        }
    }
}

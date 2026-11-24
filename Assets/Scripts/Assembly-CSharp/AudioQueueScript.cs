using UnityEngine;

public class AudioQueueScript : MonoBehaviour
{
    private AudioSource audioDevice;

    public int audioInQueue;

    private AudioClip[] audioQueue = new AudioClip[100];

    private void Start()
    {
        audioDevice = base.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioDevice.isPlaying && audioInQueue > 0)
        {
            PlayQueue();
        }
    }

    public void Queue(AudioClip sound)
    {
        audioQueue[audioInQueue] = sound;
        audioInQueue++;
    }

    private void PlayQueue()
    {
        if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
        {
            audioDevice.PlayOneShotWC(audioQueue[0]);
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                Multiplayer.ClientSend.PlaySoundClip(audioDevice.gameObject.name, audioQueue[0].name);
            }
        }
        UnqueueAudio();
    }

    private void UnqueueAudio()
    {
        for (int i = 1; i < audioInQueue; i++)
        {
            audioQueue[i - 1] = audioQueue[i];
        }
        audioInQueue--;
    }

    public void ClearQueue()
    {
        audioInQueue = 0;
    }

    public void PlaySound(AudioClip sound)
    {
        if (!MultiplayerManager.isActive || MultiplayerManager.instance.isMainPlayer)
        {
            audioDevice.PlayOneShotWC(sound);
            if (MultiplayerManager.isActive && MultiplayerManager.instance.isMainPlayer)
            {
                Multiplayer.ClientSend.PlaySoundClip(gameObject.name, sound.name);
            }
        }
    }
}
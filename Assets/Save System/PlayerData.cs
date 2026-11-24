using System;

[Serializable]
#if !UNITY_EDITOR
[Obsolete("No longer being updated. Upgrade it to SchoolSaves.PlayerData from global::PlayerData")]
#endif
public class PlayerData
{
    public float playerHealth;

    public float[] playerPos;

    public float[] baldiPos;

    public float[] prinPos;

    public float[] playtimePos;

    public float[] firstPos;

    public float[] fifthPos;

    public float[] goldenPos;

    public float[] beansPos;

    public float[] pompPos;

    public float[] sweepPos;

    public float[] craftersPos;

    public bool[] notebooksActive;

    public bool[] itemsActive;

    public int notebooks;

    public float stamina;

    public int failedNotebooks;

    public int[] itemsHas;

    public int itemSelected;

    public float[] playerRotation;

    public bool spoopMode;

    public bool finaleMode;

    public float baldiAngry;

    public float baldiTempAngry;

    public float baldiWait;

    public int exitsTaken;

    public string name;

    public string version;

    public PlayerData(GameControllerScript gc, string name)
    {
        this.name = name;
        this.version = VersionInput.currentVersion.ToString();
        stamina = gc.player.stamina;
        playerPos = new float[3]
        {
gc.player.transform.position.x,
gc.player.transform.position.y,
gc.player.transform.position.z
        };
        baldiPos = new float[3]
        {
gc.baldi.transform.position.x,
gc.baldi.transform.position.y,
gc.baldi.transform.position.z
        };
        craftersPos = new float[3]
        {
gc.crafters.transform.position.x,
gc.crafters.transform.position.y,
gc.crafters.transform.position.z
        };
        prinPos = new float[3]
        {
gc.principal.transform.position.x,
gc.principal.transform.position.y,
gc.principal.transform.position.z
        };
        playtimePos = new float[3]
        {
gc.playtime.transform.position.x,
gc.playtime.transform.position.y,
gc.playtime.transform.position.z
        };
        beansPos = new float[3]
        {
gc.beans.transform.position.x,
gc.beans.transform.position.y,
gc.beans.transform.position.z
        };
        pompPos = new float[3]
        {
gc.mrsPomp.transform.position.x,
gc.mrsPomp.transform.position.y,
gc.mrsPomp.transform.position.z
        };
        sweepPos = new float[3]
        {
gc.gottaSweep.transform.position.x,
gc.gottaSweep.transform.position.y,
gc.gottaSweep.transform.position.z
        };
        firstPos = new float[3]
        {
gc.firstPrize.transform.position.x,
gc.firstPrize.transform.position.y,
gc.firstPrize.transform.position.z
        };
        fifthPos = new float[3]
        {
gc.fifthPrize.transform.position.x,
gc.fifthPrize.transform.position.y,
gc.fifthPrize.transform.position.z
        };
        // Removed Golden Baldi as of 5/20/2021 due to request by xTPSx
        //         goldenPos = new float[3]
        //         {
        // gc.goldenBaldi.transform.position.x,
        // gc.goldenBaldi.transform.position.y,
        // gc.goldenBaldi.transform.position.z
        //         };
        playerRotation = new float[3]
        {
gc.player.transform.eulerAngles.x,
gc.player.transform.eulerAngles.y,
gc.player.transform.eulerAngles.z
        };
        itemSelected = gc.itemSelected;
        itemsHas = gc.item;
        notebooks = gc.notebooks;
        failedNotebooks = gc.failedNotebooks;
        itemsActive = new bool[gc.itemsActive.Length];
        for (int i = 0; i < gc.itemsActive.Length; i++)
        {
            itemsActive[i] = gc.itemsActive[i].activeSelf;
        }

        notebooksActive = new bool[gc.notebooksActive.Length];

        exitsTaken = gc.exitsReached;

        finaleMode = gc.finaleMode;

        for (int i = 0; i < gc.notebooksActive.Length; i++)
        {
            notebooksActive[i] = gc.notebooksActive[i].up;
        }

        playerHealth = gc.player.health;

        baldiAngry = gc.baldiScrpt.baldiAnger;

        baldiWait = gc.baldiScrpt.baldiWait;

        baldiTempAngry = gc.baldiScrpt.baldiTempAnger;

        spoopMode = gc.spoopMode;
    }
}
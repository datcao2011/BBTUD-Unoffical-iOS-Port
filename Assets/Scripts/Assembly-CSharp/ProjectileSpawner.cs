using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public GameControllerScript gc;

    public GameObject[] projectiles;

    public List<GameObject> Projectiles;

    private Transform[] AIPoints;

    public float spawnCooldown;

    private bool RunOnTimer = false;

    public int maxObjects;

    public int objects;

    // Start is called before the first frame update
    private void Start()
    {
        AIPoints = gc.baldiScrpt.wanderer.hallways;
        if (NullManager.Instance == null) Destroy(gameObject, 0f);
    }

    // Update is called once per frame
    private void Update()
    {
        if (NullManager.Instance == null) return;
        if (NullManager.Instance.BossFight || (!NullManager.Instance.BossFight && NullManager.Instance.NULL.IsMonologuing))
        {
            if (this.objects > maxObjects)
            {
                this.objects = maxObjects;
            }
            if (RunOnTimer)
            {
                if (this.spawnCooldown > 0f)
                {
                    this.spawnCooldown -= Time.deltaTime;
                }
                if (this.spawnCooldown < 0f)
                {
                    SpawnProjectile();
                    this.spawnCooldown = UnityEngine.Random.Range(5f, 25f);
                }
            }
        }
    }

    public void SpawnProjectile()
    {
    ReCalculate:
        Vector3 Pos = AIPoints[Random.Range(0, AIPoints.Length - 1)].position; // No, this isn't a code mistake. This makes an exclusion to the last spot since it's tricky to accomplish.
        Pos.y += 4f;
        foreach (GameObject Projectile in Projectiles)
        {
            if (Vector3.Distance(Projectile.transform.position, Pos) <= 1f)
            {
#if DEBUG
                Debug.LogWarning("There was supposed to be a spawn in " + string.Format("X: {0} Y: {1} Z: {2}", Pos.x, Pos.y, Pos.z) + ", but a object took its place. Re-calculating...");
#endif
                goto ReCalculate;
            }
        }
        SpawnProjectile(Pos);
    }

    public void SpawnProjectile(Vector3 Where)
    {
        Where.y = 0f;
        if (objects < maxObjects)
        {
            GameObject projectile = Instantiate(projectiles[Random.Range(0, projectiles.Length)], Where, Quaternion.identity);
            projectile.SetActive(true);
            projectile.transform.position += Vector3.up * 4f;
            Projectiles.Add(projectile);
            objects++;
        }
    }
}

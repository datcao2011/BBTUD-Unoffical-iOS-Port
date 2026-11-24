// using UnityEngine;

// public class BadBSODA : MonoBehaviour
// {
//     private float CurrentTime = 8f;

//     private void Update()
//     {
//         CurrentTime -= Time.unscaledDeltaTime * 2f;
//     }

//     private void OnTriggerStay(Collider other)
//     {
//         if (other.tag == "Player")
//         {
//             Destroy(gameObject, 0f);
//             FindObjectOfType<MainBossFight>().TakeDamage((CurrentTime + 0.3f) * Random.Range(1.5f, 3.1f));
//         }
//     }
// }
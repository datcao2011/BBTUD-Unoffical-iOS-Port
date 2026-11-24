// using System;
// using UnityEngine;

// // Token: 0x0200001A RID: 26
// public class BossItem : MonoBehaviour
// {
//     private void Update()
//     {
//         if (InteractScript.Intracting)
//         {
//             Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
//             RaycastHit hit;
//             if (Physics.Raycast(ray, out hit))
//             {
//                 if (hit.transform == base.transform & Vector3.Distance(player.position, base.transform.position) < 10f)
//                 {
//                     Destroy(click);
//                     UnityEngine.Object.Destroy(hit.transform.gameObject);
//                     if (ItemId == 0)
//                     {
//                         FindObjectOfType<MainBossFight>().TakeDamage(-40f);
//                     }
//                     else if (ItemId == 1)
//                     {
//                         FindObjectOfType<PlayerScript>().stamina = FindObjectOfType<PlayerScript>().maxStamina * 2;
//                     }
//                 }
//             }
//         }
//         Ray ray2 = Camera.main.ScreenPointToRay(ReticleScript.Position());
//         RaycastHit hit2;
//         if (Physics.Raycast(ray2, out hit2))
//         {
//             if (hit2.transform == transform & Vector3.Distance(player.position, base.transform.position) < 10f)
//             {
//                 if (click == null)
//                 {
//                     click = Instantiate(clickCursor);
//                     Destroy(click, 13f);
//                 }
//             }
//             else
//             {
//                 if (click != null)
//                     Destroy(click);
//             }
//         }
//     }

//     public Transform player;

//     public int ItemId;

//     public GameObject clickCursor;

//     public GameObject click;
// }
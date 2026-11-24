// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;
// using SimpleJSON;

// public class BossStage2 : MonoBehaviour
// {
// #if UNITY_EDITOR || HAS_EVENT_PLUGGED
//     public GameObject S;
//     public GameObject T;
//     public TMPro.TMP_Text H;
//     public string EDATA_LINK;

//     private System.DateTime Date;
//     private string Val;
//     private bool DateSet;

//     private void Awake()
//     {
//         StartCoroutine(EData());
//     }

//     // Start is called before the first frame update
//     void Start()
//     {
//         T.SetActive(Settings.GetBool("EV001"));
//         S.SetActive(Settings.GetBool("EV002"));
//         H.text = Settings.GetString("EV003");
//         Val = H.text;

//         Date = E004_C(Settings.GetInt("EV004"));
//     }

//     private System.DateTime E004_C(int unixTimeStamp)
//     {
//         // Unix timestamp is seconds past epoch
//         System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
//         dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
//         return dateTime;
//     }

//     private IEnumerator EData()
//     {
//         UnityWebRequest req = UnityWebRequest.Get(EDATA_LINK);
//         yield return req.SendWebRequest();
//         if (req.result != UnityWebRequest.Result.Success)
//         {
//             if (Settings.GetInt("EV004") > 0)
//             {
//                 DateSet = true;
//             }
//             yield break;
//         }
//         try
//         {
//             JSONNode Data = JSON.Parse(req.downloadHandler.text);
//             Settings.SetBool("EV001", Data["stat"].AsInt >= 1);
//             Settings.SetBool("EV002", Data["stat"].AsInt >= 2);
//             Settings.SetString("EV003", Data["news"].Value);
//             Settings.SetInt("EV004", Data["end"].AsInt);
//             Start();
//             if (Settings.GetInt("EV004") > 0)
//             {
//                 DateSet = true;
//             }
//         }
//         catch (System.Exception ex)
//         {
//             if (Settings.GetInt("EV004") > 0)
//             {
//                 DateSet = true;
//             }
//             Debug.LogException(ex);
//             yield break;
//         }
//         yield break;
//     }


//     // Update is called once per frame
//     void Update()
//     {
//         if (InteractScript.Intracting && Time.timeScale != 0f)
//         {
//             FindObjectOfType<PlayerScript>().transform.position = new Vector3(5f, 5f, 75f);
//             FindObjectOfType<PlayerScript>().transform.eulerAngles = new Vector3(180, 90f, -180f);
//         }
//         if (DateSet)
//         {
//             H.text = Val.Replace("{0}", string.Format("{0:dd\\:hh\\:mm\\:ss}", System.DateTime.Now.Subtract(Date)));
//             if (System.DateTime.Now > Date)
//             {
//                 Settings.DeleteKey("EV001");
//                 Settings.DeleteKey("EV002");
//                 Settings.DeleteKey("EV003");
//                 Settings.DeleteKey("EV004");
//                 Settings.SetInt("CC_SStage", 3);
//                 ApplicationConfigure.closeApplication();
//             }
//         }
//     }
// #endif
// }

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Linq;

namespace SchoolSaves
{
    [Serializable]
    public class PlayerData
    {
        #region Save Details
        public string Name;
        public string Version;
        #endregion

        #region Player Attributes
        public float Health;
        public float Stamina;

        // Player
        public float[] Position;
        public float[] Rotation;

        // Item
        public int[] Items;
        public int ItemCurrentlySelected;
        #endregion

        #region Game
        public int Notebooks;
        public int MaxNotebooks;
        public int FailedNotebooks;

        public bool BaldiIsAngry;

        // Baldi
        public float Angry;
        public float TemporaryAngry;
        public float BaldiWait;

        // Exit
        public bool FinaleModeActivated;
        public int Exits;
        public bool[] ExitsReached = new bool[4] { false, false, false, false };

        // Event
        public int EventID = -1;
        public float EventTimer = 0f;
        #endregion

        #region Settings
        public string Param_ModeSelected = "story";
        public string Param_ModeType = "Normal";
        public string Param_Scene = "School";
        public string[] Params_Keys = new string[0];
        public object[] Params_Values = new object[0];
        #endregion

        #region XML
        public string SaveXML;
        public string SaveBIN;
        #endregion

        public PlayerData(GameControllerScript gc, string name)
        {
            if (gc == null) return;
            // Save Details
            Version = VersionInput.currentVersion.ToString();
            Name = name;

            // Player attributes
            Health = gc.player.health;
            Stamina = gc.player.stamina;

            Position = gc.player.transform.position.ToFloat();
            Rotation = gc.player.transform.rotation.eulerAngles.ToFloat();

            // Settings
            Param_ModeSelected = Settings.GetString("CurrentMode", "story");
            Param_ModeType = Settings.GetString("ModeType", "Normal");
            Param_Scene = gc.scene;
            if (Param_ModeSelected == "hideAndSeekGame")
            {
                Dictionary<string, object> Pairs = new Dictionary<string, object>
                {
                    { "HAS_HOS", PlayerPrefs.GetString("HAS_HOS") },
                    { "HAS_Rounds", PlayerPrefs.GetInt("HAS_Rounds", 1) },
                    { "HAS_BWins", PlayerPrefs.GetInt("HAS_BWins", 0) },
                    { "HAS_PWins", PlayerPrefs.GetInt("HAS_PWins", 0) },
                    { "HAS_KNH", PlayerPrefs.GetString("HAS_KNH", "") }
                };
                Params_Keys = Pairs.Keys.ToArray();
                Params_Values = Pairs.Values.ToArray();
            }

            // XML saving
            SaveXML xml = new SaveXML();

            xml.Characters = (Param_ModeType == "NULL") ? new Character[0] : Utils.GetCharacters();
            xml.Items = Utils.GetItems();
            xml.Notebooks = Utils.GetNotebooks();

            SaveBIN = Utils.Base64Encode(Utils.GetCharacterXML(xml));

            // Items
            Items = gc.item;
            ItemCurrentlySelected = gc.itemSelected;

            // Notebooks
            Notebooks = gc.notebooks;
            MaxNotebooks = gc.maxNotebooks;
            FailedNotebooks = gc.failedNotebooks;

            // Baldi
            if (Param_ModeType == "NULL")
            {
                BaldiIsAngry = true;
                Angry = NullManager.Instance.NULL.baldiAnger;
                TemporaryAngry = NullManager.Instance.NULL.baldiTempAnger;
                BaldiWait = NullManager.Instance.NULL.baldiWait;
                Dictionary<string, object> Pairs = new Dictionary<string, object>
                {
                    { "NULLPosition", NullManager.Instance.NULL.transform.position.ToFloat() },
                    { "NULLRotation", NullManager.Instance.NULL.transform.eulerAngles.ToFloat() }
                };
                Params_Keys = Pairs.Keys.ToArray();
                Params_Values = Pairs.Values.ToArray();
            }
            else
            {
                if (Param_ModeSelected == "endless")
                {
                    Dictionary<string, object> Pairs = new Dictionary<string, object>();
                    NotebookScript[] notebooks = GameObject.FindObjectsOfType<NotebookScript>(false);
                    foreach (NotebookScript notebook in notebooks)
                    {
                        Pairs.Add(notebook.name, notebook.respawnTime);
                    }
                    Params_Keys = Pairs.Keys.ToArray();
                    Params_Values = Pairs.Values.ToArray();
                }
                BaldiIsAngry = gc.spoopMode;
                Angry = gc.baldiScrpt.baldiAnger;
                TemporaryAngry = gc.baldiScrpt.baldiTempAnger;
                BaldiWait = gc.baldiScrpt.baldiWait;
            }

            // Exit Mode
            FinaleModeActivated = gc.finaleMode;
            Exits = gc.exitsReached;
            if (FinaleModeActivated)
            {
                EntranceScript[] Entrances = GameObject.FindObjectsOfType<EntranceScript>();
                for (int i = 0; i < Entrances.Length; i++)
                {
                    ExitsReached[i] = Entrances[i].exitTaken;
                }
            }

            // Event
            RandomEvent eventSystem = GameObject.FindObjectOfType<RandomEvent>();
            EventID = (eventSystem.eventEndTimer <= 0f) ? -1 : eventSystem.eventChoose;
            EventTimer = Mathf.Max(0f, eventSystem.eventEndTimer);
        }
    }

    public static class Utils
    {
        public static SchoolSaves.PlayerData ConvertOldToNew(global::PlayerData OldData)
        {
            // Data conversion
            Debug.Log("Converting save \"" + OldData.name + "\" to the new save data format file...");
            PlayerData Result = new PlayerData(null, OldData.name);

            // Save Details
            Result.Version = VersionInput.currentVersion.ToString();
            Result.Name = OldData.name;

            // Player attributes
            Result.Health = OldData.playerHealth;
            Result.Stamina = OldData.stamina;

            Result.Position = OldData.playerPos;
            Result.Rotation = OldData.playerRotation;

            // Item
            Result.Items = OldData.itemsHas;
            Result.ItemCurrentlySelected = OldData.itemSelected;

            // Game
            Result.Notebooks = OldData.notebooks;
            Result.MaxNotebooks = 7;
            Result.FailedNotebooks = OldData.failedNotebooks;
            Result.BaldiIsAngry = OldData.spoopMode;

            // Baldi
            Result.Angry = OldData.baldiAngry;
            Result.TemporaryAngry = OldData.baldiTempAngry;
            Result.BaldiWait = OldData.baldiWait;

            // Exits
            Result.Exits = OldData.exitsTaken;
            Result.FinaleModeActivated = OldData.finaleMode;

            // Variables to XML Conversion
            SaveXML saveXML = new SaveXML();
            // Characters
            #region Character Conversion
            List<Character> list = new List<Character>();
            list.Add(FromPosAndRot("Baldi", OldData.baldiPos, OldData.spoopMode));
            list.Add(FromPosAndRot("1st Prize", OldData.firstPos, true));
            list.Add(FromPosAndRot("5th Prize", OldData.fifthPos, true));
            if (OldData.goldenPos != null && OldData.goldenPos != new float[3])
            {
                list.Add(FromPosAndRot("GoldenBaldi (Removed)", OldData.goldenPos, true));
            }
            list.Add(FromPosAndRot("Playtime", OldData.playtimePos, true));
            list.Add(FromPosAndRot("Arts and Crafters", OldData.craftersPos, true));
            list.Add(FromPosAndRot("Mrs Pomp", OldData.pompPos, true));
            list.Add(FromPosAndRot("Principal of the Thing", OldData.prinPos, true));
            list.Add(FromPosAndRot("Gotta Sweep", OldData.sweepPos, true));
            list.Add(FromPosAndRot("Beans", OldData.beansPos, true));
            saveXML.Characters = list.ToArray();
            #endregion
            // Items
            saveXML.Items = new Item[0];
            // Notebooks
            #region Notebook Conversion
            List<Notebook> list2 = new List<Notebook>();
            list2.Add(FromNotebookTrigger("GeographyNotebook", OldData.notebooksActive[0]));
            list2.Add(FromNotebookTrigger("EnglishNotebook", OldData.notebooksActive[1]));
            list2.Add(FromNotebookTrigger("SpellingNotebook", OldData.notebooksActive[2]));
            list2.Add(FromNotebookTrigger("HistoryNotebook", OldData.notebooksActive[3]));
            list2.Add(FromNotebookTrigger("MathNotebook", OldData.notebooksActive[4]));
            list2.Add(FromNotebookTrigger("GeologyNotebook", OldData.notebooksActive[5]));
            list2.Add(FromNotebookTrigger("ScienceNotebook", OldData.notebooksActive[6]));
            saveXML.Notebooks = list2.ToArray();
            #endregion

            Result.SaveBIN = Base64Encode(GetCharacterXML(saveXML));
            return Result;
        }

        private static Character FromPosAndRot(string name, float[] position, bool Active)
        {
            Character cc = new Character();
            cc.GB_Name = name;
            cc.Position = position;
            cc.Active = Active;
            return cc;
        }

        private static Notebook FromNotebookTrigger(string name, bool up)
        {
            Notebook nb = new Notebook();
            nb.GB_Name = name;
            nb.IsUp = up;
            return nb;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static Character[] GetCharacters() => GetCharacters(GameObject.Find("Characters").GetComponentsInChildren<NPC>(true));

        public static Character[] GetCharacters(NPC[] NPCs)
        {
            List<Character> Result = new List<Character>();
            foreach (NPC npc in NPCs)
            {
                Character cc = new Character();
                cc.GB_Name = npc.name;
                cc.Position = npc.transform.position.ToFloat();
                cc.Rotation = npc.transform.rotation.eulerAngles.ToFloat();
                cc.Active = npc.isActiveAndEnabled;
                Result.Add(cc);
            }
            return Result.ToArray();
        }

        public static Item[] GetItems()
        {
            PickupScript[] items = GameObject.FindObjectsOfType<PickupScript>(true);

            List<Item> Result = new List<Item>();
            foreach (PickupScript item in items)
            {
                Item it = new Item();
                it.GB_IID = item.GetInstanceID();
                it.IsUp = item.isActiveAndEnabled;
                it.Position = item.transform.position.ToFloat();
                Result.Add(it);
            }
            return Result.ToArray();
        }

        public static Notebook[] GetNotebooks()
        {
            NotebookScript[] notebooks = GameObject.FindObjectsOfType<NotebookScript>(false);

            List<Notebook> Result = new List<Notebook>();
            foreach (NotebookScript notebook in notebooks)
            {
                Notebook nb = new Notebook();
                nb.GB_Name = notebook.gameObject.name;
                nb.IsUp = notebook.up;
                Result.Add(nb);
            }
            return Result.ToArray();
        }

        public static string GetCharacterXML(SaveXML xml)
        {
            string result;
            XmlSerializer serializer = new XmlSerializer(typeof(SaveXML));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, xml);
                result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
            }
            return result;
        }

        public static SaveXML GetXMLContent(string xml)
        {
            SaveXML result;
            UnicodeEncoding uniEncoding = new UnicodeEncoding();
            XmlSerializer serializer = new XmlSerializer(typeof(SaveXML));
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            result = (SaveXML)serializer.Deserialize(reader);
            return result;
        }

        public static float[] ToFloat(this UnityEngine.Vector3 vector3)
        {
            return new float[] { vector3.x, vector3.y, vector3.z };
        }

        public static Vector3 ToVector3(this float[] ThreeFloats)
        {
            return new Vector3(ThreeFloats[0], ThreeFloats[1], ThreeFloats[2]);
        }

        public static GameObject FindObjectWithName(string name)
        {
            return GameObject.Find("Characters").FindObject(name);
        }

        private static GameObject FindObject(this GameObject parent, string name)
        {
            NPC[] trs = parent.GetComponentsInChildren<NPC>(true);
            foreach (NPC t in trs)
            {
                if (t.name == name)
                {
                    return t.gameObject;
                }
            }
            return null;
        }

        public static UnityEngine.Object FindObjectFromInstanceID(int aObjectID)
        {
            var methodInfo = typeof(UnityEngine.Object)
                 .GetMethod("FindObjectFromInstanceID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (methodInfo == null)
            {
                throw new MissingMethodException();
            }
            return (UnityEngine.Object)methodInfo.Invoke(null, new object[] { aObjectID });
        }
    }

    public class Character
    {
        [XmlAttribute]
        public string GB_Name;
        public float[] Position;
        public float[] Rotation;
        public bool? Active = null;
    }

    public class Notebook
    {
        [XmlAttribute]
        public string GB_Name;
        public bool IsUp;
    }

    public class Item
    {
        [XmlAttribute]
        public int GB_IID;
        public float[] Position;
        public bool IsUp;
    }

    [XmlRootAttribute("SaveXML", Namespace = "http://www.cpandl.com", IsNullable = false)]
    public class SaveXML
    {
        public Character[] Characters;
        public Notebook[] Notebooks;
        public Item[] Items;
    }
}

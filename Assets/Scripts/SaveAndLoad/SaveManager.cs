using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.SaveAndLoad
{
    public class SaveManager : MonoBehaviour
    {
        public MessagePrinter messagePrinter;

        // Start is called before the first frame update
        void Start() { }

        /// <summary>
        /// Saves the current Scene to the specified pass.
        /// </summary>
        /// <param name="manager">A reference to the constructionPhasesManager to get access to objects lists</param>
        /// <param name="path">The path to the savefile (file is created if not existing)</param>
        public void SaveCurrentScene(ConstructionPhasesManager manager, string path)
        {

            //Check if path is not empty
            if (string.IsNullOrEmpty(path))
            {
                this.SendToMsgPrinter("Szene speichern: Kein Dateipfad übergeben! Speichern abgebrochen.");
                return;
            }


            //Fill SaveData Object with values
            SaveData saveData = new SaveData();
            saveData.PlacedObjects = Serializer.SerializePlacedObjects(manager.PlacedObjects);
            saveData.PlacedObjectsStates = Serializer.SerializePlacedObjectsStates(manager.PlacedObjectsStates);
            //saveData.Width = float.Parse(BsmuSceneManager.GetParam("groundZ"));
            //saveData.Length = float.Parse(BsmuSceneManager.GetParam("groundX"));


            //Serialize to binary            
            FileStream stream = File.Create(path);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, saveData);
            stream.Close();


            #region Serialize to xml (for debuggin purposes)

            /*
            // Can be used for debugging purposes to check the content of the file

            // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
            FileStream writer = File.Create(path + ".xml");

            //Serialize to xml
            DataContractSerializer serializer = new DataContractSerializer(typeof(SaveData));
            MemoryStream streamer = new MemoryStream();

            //Serialize the file
            serializer.WriteObject(streamer, saveData);
            streamer.Seek(0, SeekOrigin.Begin);

            //Save to disk
            writer.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

            // Close the file to prevent any corruptions
            writer.Close();
            streamer.Close();

            this.SendToMsgPrinter("SaveManager: XML file created");

            */

            #endregion

            this.SendToMsgPrinter("Datei erfolgreich gespeichert", 5);

        }


        /// <summary>
        /// Wrapper class to prevent sending to messagePrinter if its not assigned
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="duration">How long the message should be displayed</param>
        private void SendToMsgPrinter(string msg, int duration = 10)
        {
            if (messagePrinter != null)
            {
                messagePrinter.PrintTextBoxMessage(msg, duration);
            }
            else
            {
                Debug.LogError("LoadManager: Message Printer not assigned!");
                Debug.Log(msg);
            }
        }

    }
}

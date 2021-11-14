using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.SaveAndLoad
{
    public class LoadManager : MonoBehaviour
    {

        public ConstructionPhasesManager constructionPhasesManager;
        public MessagePrinter messagePrinter;

        // Start is called before the first frame update
        void Start() { }


        /// <summary>
        /// Deserializes the file from the path and passes the loaded lists to the constructionsPhasesManager.LoadLists-Method
        /// </summary>
        /// <param name="path">Path to the file</param>
        public void LoadScene(string path)
        {
            //Check for potential failures
            if(constructionPhasesManager == null)
            {
                SendToMsgPrinter("LoadManager: ConstructionPhasesMager not referenced! Abort loading.");
                return;
            }
            if(string.IsNullOrEmpty(path)){
                SendToMsgPrinter("LoadManager: path is empty! Abort loading.");
                return;
            }
            if (!File.Exists(path))
            {
                SendToMsgPrinter("LoadManager: file does not exist or is not accessible! Abort loading.");
                return;
            }


            #region Deserialize from binary


            SaveData loadedSaveData = new SaveData();
            FileStream stream = File.OpenRead(path);
            
            try
            {
                // Restore from file
                loadedSaveData = (SaveData)new BinaryFormatter().Deserialize(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                if (ex is DecoderFallbackException)
                {
                    SendToMsgPrinter("LoadManager: Failure at deserializing, malformed file! Abort loading.");
                }
                else
                {
                    SendToMsgPrinter("LoadManager: Failed at converting file! Abort loading.");
                }
                    

                stream.Close();
                return;
            }


            #endregion


            constructionPhasesManager.LoadLists(loadedSaveData.PlacedObjects, loadedSaveData.PlacedObjectsStates);           

        }

        /// <summary>
        /// Wrapper for PrintTextBoxMessage. Checks if the messagePrinter is assigned before sending. If this is not the case, the output is printed to Debug.Log
        /// </summary>
        /// <param name="msg"></param>
        private void SendToMsgPrinter(string msg)
        {
            if(messagePrinter != null)
            {
                messagePrinter.PrintTextBoxMessage(msg);
            }
            else
            {
                Debug.LogError("LoadManager: Message Printer not assigned!");
                Debug.Log(msg);
            }
        }

    }
}

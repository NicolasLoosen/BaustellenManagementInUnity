using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MessagePrinter : MonoBehaviour
    {


        public GameObject textMsgPanel;
        public GameObject textMsgEntryPrefab;


        /// <summary>
        /// Print the passed message in the UI. If no duration is passed, the message be displayed for 10 seconds
        /// </summary>
        /// <param name="message">The Message to display</param>
        /// <param name="duration">Duration in seconds</param>
        public void PrintTextBoxMessage(string message, float duration = 10f)
        {
            GameObject newListEntry = Instantiate(textMsgEntryPrefab, textMsgPanel.transform);
            newListEntry.name = "TextMSg " + DateTime.Now.ToLongTimeString();
            newListEntry.GetComponentInChildren<Text>().text = message;

            StartCoroutine(DestroyMsgTimer(newListEntry, duration));
        }


        private IEnumerator DestroyMsgTimer(GameObject msgBox, float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(msgBox);
        }
    }
}

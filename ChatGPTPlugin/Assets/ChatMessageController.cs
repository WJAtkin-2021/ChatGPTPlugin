using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GPTPlugin
{
    public class ChatMessageController : MonoBehaviour
    {
        public TMP_Text chatTextLabel;

        public void Init(string message)
        {
            chatTextLabel.autoSizeTextContainer = true;
            chatTextLabel.text = message;
        }

        public void Update() 
        {
            Vector2 newSize = chatTextLabel.GetComponent<RectTransform>().sizeDelta;
            newSize.x = GetComponent<RectTransform>().sizeDelta.x;
            newSize.y = chatTextLabel.preferredHeight;

            GetComponent<RectTransform>().sizeDelta = newSize;
            chatTextLabel.GetComponent<RectTransform>().sizeDelta = newSize;
        }
    }
}

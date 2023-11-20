using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPTPlugin
{
    public class ConversationDisplay : MonoBehaviour
    {
        public GameObject MessageRootNode;
        public ScrollRect ScrollRect;

        public GameObject GPTChatTemplate;
        public GameObject UserChatTemplate;

        private GameObject GPTChatTemplateClone;
        private GameObject UserChatTemplateClone;

        // Start is called before the first frame update
        void Start()
        {
            GPTChatTemplateClone = Instantiate(GPTChatTemplate);
            UserChatTemplateClone = Instantiate(UserChatTemplate);

            Destroy(GPTChatTemplate);
            Destroy(UserChatTemplate);
        }

        public void AppendChatMessage(string actor, string message)
        {
            if (actor == "assistant")
            {
                GameObject newMessage = Instantiate(GPTChatTemplateClone, MessageRootNode.transform);
                newMessage.GetComponent<ChatMessageController>().Init($"SadGPT: {message}");
            }
            else if (actor == "user")
            {
                GameObject newMessage = Instantiate(UserChatTemplateClone, MessageRootNode.transform);
                newMessage.GetComponent<ChatMessageController>().Init($"User: {message}");
            }
            else
            {
                throw new NotImplementedException("Invalid actor when appending chat message");
            }

            StartCoroutine(ScrollToBottomDelayed());
        }

        public IEnumerator ScrollToBottomDelayed()
        {
            yield return new WaitForEndOfFrame();

            ScrollRect.ScrollToBottom();
        }
    }
}

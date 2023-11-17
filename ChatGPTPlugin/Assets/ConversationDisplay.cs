using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPTPlugin
{
    public class ConversationDisplay : MonoBehaviour
    {
        public GameObject MessageRootNode;

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
                newMessage.GetComponent<ChatMessageController>().Init($"ChatGPT: {message}");
            }
            else if (actor == "user")
            {
                GameObject newMessage = Instantiate(UserChatTemplateClone, MessageRootNode.transform);
                newMessage.GetComponent<ChatMessageController>().Init($"User: {message}");
            }
        }
    }
}

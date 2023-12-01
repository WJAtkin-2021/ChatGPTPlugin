using System.Collections.Generic;

namespace GPTPlugin
{
    public class GPTContext
    {
        public List<GPTMessage> messages = new List<GPTMessage>();
        public int max_tokens = 2000;
        public string model = "gpt-3.5-turbo-1106";
        //public string model = "gpt-4-0613";

        public GPTContext() { }
        public GPTContext(string systemPrompt)
        {
            messages.Add(new GPTMessage("system", systemPrompt));
        }
    }
}

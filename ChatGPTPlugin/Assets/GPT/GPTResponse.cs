using System.Collections.Generic;

namespace GPTPlugin
{
    public class GPTResponse
    {
        public string id = string.Empty;
        public string Object = string.Empty;
        public int created;
        public string model = string.Empty;
        public List<GPTChoices> choices = new List<GPTChoices>();
        public GPTUsage usage = new GPTUsage();
        public string system_fingerprint = string.Empty;
    }
}

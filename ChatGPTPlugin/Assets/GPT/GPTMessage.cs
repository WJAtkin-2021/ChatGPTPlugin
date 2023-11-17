namespace GPTPlugin
{
    public class GPTMessage
    {
        public string role = string.Empty;
        public string content = string.Empty;

        public GPTMessage() { }

        public GPTMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}

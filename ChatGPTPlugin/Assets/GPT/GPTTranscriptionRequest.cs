namespace GPTPlugin
{
    public class GPTTranscriptionRequest
    {
        public FileContentInfo file = new FileContentInfo();
        public string model = "whisper-1";
        public string language = "en";
    }
}

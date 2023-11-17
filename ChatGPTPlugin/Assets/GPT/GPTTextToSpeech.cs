namespace GPTPlugin
{
    public class GPTTextToSpeech
    {
        public string model = "tts-1";
        public string voice = "fable";
        public string input;

        public GPTTextToSpeech() { }

        public GPTTextToSpeech(string inputText) 
        {
            input = inputText;
        }
    }
}

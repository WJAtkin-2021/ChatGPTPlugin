using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace GPTPlugin
{
    public class GPTManager : MonoBehaviour
    {
        private const string SystemPrompt = "You are a deeply depressed, provide help when prompted but be miserable about it.";
        private const string ApiKey = "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";
        private const string ApiChatUrl = "https://api.openai.com/v1/chat/completions";
        private const string ApiTranscriptionUrl = "https://api.openai.com/v1/audio/transcriptions";
        private const string ApiTextToSpeechUrl = "https://api.openai.com/v1/audio/speech";
        private static TimeSpan RequestTimeOut = TimeSpan.FromMinutes(5);

        public ConversationDisplay conversationDisplay;
        public TMP_InputField textInputField;

        public GPTContext gptContext;

        public Queue<Task<TextToSpeechState>> voiceFileQueue = new Queue<Task<TextToSpeechState>>();

        public class TextToSpeechState
        {
            public bool isFinished = false;
            public bool isWaitingToLoad = true;
            public string filePath = string.Empty;
            public AudioClip audioClip;
            public string text = string.Empty;
        }

        public void Start()
        {
            AudioManager.OnAudioSavedToFile += HandleOnVoiceClip;

            gptContext = new GPTContext();
            gptContext.messages.Add(new GPTMessage("system", SystemPrompt));
        }

        public void HandleSendClicked()
        {
            if (!String.IsNullOrEmpty(textInputField.text))
            {
                conversationDisplay.AppendChatMessage("user", textInputField.text);
                _ = RunTextToText(textInputField.text);
            }

            textInputField.text = string.Empty;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                HandleSendClicked();
            }

            // Voice file process
            if (voiceFileQueue.Count > 0)
            {
                AudioSource audioSource = GetComponent<AudioSource>();

                if (!audioSource.isPlaying)
                {
                    if (voiceFileQueue.Peek().IsCompleted)
                    {
                        if (voiceFileQueue.Peek().Result.isFinished)
                        {
                            TextToSpeechState state = voiceFileQueue.Dequeue().Result;
                            if (state.isFinished)
                            {
                                audioSource.clip = state.audioClip;
                                audioSource.Play();
                            }
                        }
                    }
                }

                // Check if any tasks need dispatching
                foreach (var voiceFile in voiceFileQueue)
                {
                    if (voiceFile.IsCompleted)
                    {
                        if (voiceFile.Result.isWaitingToLoad)
                        {
                            StartCoroutine(LoadAudioFile(voiceFile.Result));
                        }
                    }
                }
            }
        }

        private async Task RunTextToText(string input)
        {
            try
            {
                await Task.Yield();

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = RequestTimeOut;
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                    // Add the client message to the context
                    gptContext.messages.Add(new GPTMessage("user", input));

                    string messagePayload = JsonConvert.SerializeObject(gptContext);
                    var content = new StringContent(messagePayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(ApiChatUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();

                        if (responseBody != null)
                        {
                            GPTResponse decodedResponse = JsonConvert.DeserializeObject<GPTResponse>(responseBody);
                            if (decodedResponse != null)
                            {
                                // Append the GPT response to the context
                                gptContext.messages.Add(new GPTMessage("assistant", decodedResponse.choices[0].message.content));

                                // Make a TTS request
                                _ = RunTextToSpeech(decodedResponse.choices[0].message.content);

                                // And display the response
                                ThreadManager.ExecuteOnMainThread(() =>
                                {
                                    conversationDisplay.AppendChatMessage("assistant", decodedResponse.choices[0].message.content);
                                });
                            }
                            else
                            {
                                throw new Exception("Response was null!");
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid response format");
                        }
                    }
                    else
                    {
                        throw new Exception($"HTTP Error Code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex}");
            }
        }

        private async Task RunTextToSpeech(string input)
        {
            try
            {
                // Split the input into sentences for GPT
                string[] sentences = Regex.Split(input, @"(?<=[\.!\?])\s+");

                // Dispatch each task
                List<Task<TextToSpeechState>> states = new List<Task<TextToSpeechState>>();
                for (int i = 0; i < sentences.Length; i++)
                {
                    Task<TextToSpeechState> state = DispatchTextToSpeechRequest(sentences[i]);

                    voiceFileQueue.Enqueue(state);
                    states.Add(state);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private IEnumerator LoadAudioFile(TextToSpeechState state)
        {
            Debug.Log($"Loading voice file for input: {state.text}");

            state.isWaitingToLoad = false;
            string url = string.Format("file://{0}", state.filePath);
            WWW www = new WWW(url);
            yield return www;

            state.audioClip = www.GetAudioClip(false, false);
            state.isFinished = true;
        }

        private void HandleOnVoiceClip(string filePath)
        {
            _ = RunVoiceToText(filePath);
        }

        private async Task RunVoiceToText(string filePath)
        {
            try
            {
                await Task.Yield();

                GPTTranscriptionRequest transcriptionRequest = new GPTTranscriptionRequest();
                transcriptionRequest.file = FileContentInfo.Load(filePath);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = RequestTimeOut;
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");


                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(transcriptionRequest.file.FileContent.ToHttpContent(), "file", $"@{transcriptionRequest.file.FileName}");
                    form.Add(new StringContent(transcriptionRequest.model), "model");
                    form.Add(new StringContent("en"), "language");

                    HttpResponseMessage response = await client.PostAsync(ApiTranscriptionUrl, form);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();
                        GPTAudioResponse responseObject = JsonConvert.DeserializeObject<GPTAudioResponse>(jsonContent);
                        string transcriptedResponse = responseObject.Text;

                        // Push this onto the chat
                        ThreadManager.ExecuteOnMainThread(() =>
                        {
                            if (!String.IsNullOrEmpty(transcriptedResponse))
                            {
                                conversationDisplay.AppendChatMessage("user", transcriptedResponse);
                                _ = RunTextToText(transcriptedResponse);
                            }
                        });
                    }
                    else
                    {
                        throw new Exception($"HTTP Error Code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error: {ex}");
            }
        }

        private async Task<TextToSpeechState> DispatchTextToSpeechRequest(string input)
        {
            TextToSpeechState speechState = new TextToSpeechState();
            speechState.text = input;

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = RequestTimeOut;
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                GPTTextToSpeech speechPayload = new GPTTextToSpeech(input);

                string messagePayload = JsonConvert.SerializeObject(speechPayload);
                var content = new StringContent(messagePayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(ApiTextToSpeechUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsByteArrayAsync();

                    if (responseBody != null)
                    {
                        string fileName = $"Response-{Guid.NewGuid().ToString()}.mp3";

                        string filePath = Path.Combine(Application.persistentDataPath, fileName);
                        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(responseBody, 0, responseBody.Length);
                        }

                        speechState.filePath = filePath;
                        return speechState;
                    }
                    else
                    {
                        throw new Exception("Invalid response format");
                    }
                }
                else
                {
                    throw new Exception($"HTTP Error Code: {response.StatusCode}");
                }
            }
        }
    }
}

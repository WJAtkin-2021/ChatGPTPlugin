using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

namespace GPTPlugin
{
    public class AudioManager : MonoBehaviour
    {
        private const string TempFileName = "VoiceCommand.wav";
        public static Action<string> OnAudioSavedToFile;
        private AudioClip m_audioClip;

        public void BeginAudioRecording()
        {
            AudioSource source = GetComponent<AudioSource>();
            source.clip = Microphone.Start("", false, 60, 44100);
            m_audioClip = source.clip;
        }

        public void EndAudioRecording() 
        {
            Microphone.End("");

            _ = SaveAudioClip();
        }

        private async Task SaveAudioClip()
        {
            try
            {
                await Task.Yield();

                string filePath = SaveWav.Save(TempFileName, m_audioClip, true);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    OnAudioSavedToFile?.Invoke(filePath);
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}

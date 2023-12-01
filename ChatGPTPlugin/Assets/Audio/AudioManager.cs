using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace GPTPlugin
{
    public class AudioManager : MonoBehaviour
    {
        private const string TempFileName = "VoiceCommand.wav";
        private const float MIN_MIC_LEVEL = 0.0006f;
        private const float AUTO_STOP_MIC_TIME = 2.0f;

        public static Action<string> OnAudioSavedToFile;
        public static Action OnMicTimeout;

        public AudioSource EndRecordingDing;

        private AudioSource m_audioSource;
        private AudioClip m_audioClip;
        private bool m_isRecording = false;
        private float m_quietTime = 0.0f;

        public void BeginAudioRecording()
        {
            m_audioClip = Microphone.Start("", false, 60, 44100);   
            m_audioSource.clip = m_audioClip;
            //m_audioSource.Play();
            m_isRecording = true;
        }

        public void EndAudioRecording() 
        {
            if (m_isRecording)
            {
                m_isRecording = false;
                m_quietTime = 0.0f;
                Microphone.End("");

                _ = SaveAudioClip();
            }
        }

        private void Start()
        {
            m_audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            // Disabled for now
            //if (m_isRecording)
            //{
            //    float[] clipSampleData = new float[1024];
            //    m_audioSource.GetOutputData(clipSampleData, 0);
            //    float currentAverageVolume = clipSampleData.Average();
            //
            //    if (currentAverageVolume >= MIN_MIC_LEVEL)
            //        Debug.LogWarning($"Current MIC level: {currentAverageVolume}");
            //
            //    if (currentAverageVolume > MIN_MIC_LEVEL)
            //    {
            //        m_quietTime = 0.0f;
            //    }
            //    else
            //    {
            //        m_quietTime += Time.deltaTime;
            //    }
            //
            //    if (m_quietTime > AUTO_STOP_MIC_TIME)
            //    {
            //        EndAudioRecording();
            //        OnMicTimeout?.Invoke();
            //        EndRecordingDing.Play();
            //    }
            //}
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

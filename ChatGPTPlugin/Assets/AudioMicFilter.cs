using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPTPlugin
{
    public class AudioMicFilter : MonoBehaviour
    {
        public float RightEar;
        public float LeftEar;
        public float MinVolume = 1f;

        public float ActivityThreshold = 0.01f;

        public float RMS = 0;
        public float DB = 0;
        public float Sum = 0;
        public float MaxVal = 0;

        public event Action OnActivity;

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            float Sum = 0;
            float MaxVal = 0;

            var sampleSize = data.Length / channels;

            for (int i = 0; i < data.Length; i++)
            {
                float value = data[i];
                Sum += value * value;

                MaxVal = Mathf.Max(MaxVal, Mathf.Abs(value));

                if (i % 2 == 0) //left
                {
                    data[i] = data[i] * Mathf.Max(MinVolume, Mathf.Clamp01(LeftEar));
                }
                else
                {
                    data[i] = data[i] * Mathf.Max(MinVolume, Mathf.Clamp01(RightEar));
                }
            }

            RMS = Mathf.Sqrt(Sum / data.Length);

            if (RMS > ActivityThreshold)
                OnActivity?.Invoke();

            DB = Mathf.Max(-160.0f, 20.0f * Mathf.Log10(RMS / RMS));
        }
    }
}

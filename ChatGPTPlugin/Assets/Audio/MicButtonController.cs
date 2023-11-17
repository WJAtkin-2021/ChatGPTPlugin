using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPTPlugin
{
    public class MicButtonController : MonoBehaviour
    {
        public AudioManager audioManager;
        public Sprite offSprite;
        public Sprite onSprite;
        public Image image;

        private bool m_isRecording = false;

        public void HandleButtonClicked()
        {
            if (!m_isRecording)
            {
                image.sprite = onSprite;
                audioManager.BeginAudioRecording();
            }
            else
            {
                image.sprite = offSprite;
                audioManager.EndAudioRecording();
            }

            m_isRecording = !m_isRecording;
        }
    }
}

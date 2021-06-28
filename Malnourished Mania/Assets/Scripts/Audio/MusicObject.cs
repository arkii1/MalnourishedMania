using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class MusicObject : MonoBehaviour
    {
        private AudioSource _audioSource;
        private void Awake()
        {
            if (FindObjectOfType<MusicObject>() != this)
                Destroy(gameObject);

            DontDestroyOnLoad(transform.gameObject);
            _audioSource = GetComponent<AudioSource>();
            PlayMusic();
        }

        public void PlayMusic()
        {
            if (_audioSource.isPlaying) return;
            _audioSource.Play();
        }

        public void StopMusic()
        {
            _audioSource.Stop();
        }
    }
}

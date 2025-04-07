using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class MapSoundExchange : MonoBehaviour
    {
        public AudioClip music;

        public void OnTriggerEnter(Collider other)
        {
            if (SoundManager.Singleton.isPlayingSameSound(music.name) == false)
                SoundManager.Singleton.PlayBGM(music.name);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Playables;

namespace TST
{
    public class IngameStartCinematic : MonoBehaviour
    {
        public static IngameStartCinematic Instance { get; private set; }

        public PlayableDirector sequence;

        public System.Action OnCinematicSequenceFinished;

        private void Awake()
        {
            Instance = this;

            sequence.stopped += _ => OnCinematicSequenceFinished?.Invoke();
            sequence.stopped += _ => Destroy(gameObject);
        }

        public void StartCinematic()
        {
            sequence.Play();
        }

        public void SkipCinematic()
        {
            sequence.Stop();
        }
    }
}

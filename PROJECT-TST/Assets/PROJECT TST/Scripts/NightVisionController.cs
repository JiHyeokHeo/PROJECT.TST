using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TST
{
    public class NightVisionController : MonoBehaviour
    {
        public static NightVisionController Instance { get; private set; }

        private Volume volume;

        public bool IsActiveVision
        {
            get => isActiveVision;
            set
            {
                isActiveVision = value;
                SetActiveNightVision(isActiveVision);
            }

        }

        private bool isActiveVision = false;

        private void Awake()
        {
            Instance = this;
            volume = GetComponent<Volume>();
        }

        private void Update()
        {
            volume.weight = Mathf.Lerp(volume.weight, isActiveVision ? 1 : 0, Time.deltaTime * 5f);
        }

        [Sirenix.OdinInspector.Button("Toggle Night Vision")]
        public void SetActiveNightVision(bool isActive)
        {
            this.isActiveVision = isActive;
        }
    }
}
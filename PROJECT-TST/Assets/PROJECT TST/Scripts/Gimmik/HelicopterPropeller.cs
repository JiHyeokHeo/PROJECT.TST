using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class HelicopterPropeller : MonoBehaviour
    {
        public Transform propellerTrasform;


        private Quaternion rotation;
        public void Awake()
        {
            rotation = Quaternion.Euler(new Vector3(0, 60f, 0));
        }

        void Update()
        {
            propellerTrasform.localRotation = Quaternion.Lerp(propellerTrasform.localRotation, propellerTrasform.localRotation * rotation, Time.deltaTime * 10.0f);
        }
    }
}

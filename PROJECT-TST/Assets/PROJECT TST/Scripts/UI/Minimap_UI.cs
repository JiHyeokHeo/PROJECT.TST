using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class Minimap_UI : UIBase
    {
        public Transform compassTransform;
        public RectTransform[] coordinates = new RectTransform[4];

        public Transform cameraTransform;

        public void Start()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (cameraTransform == null)
                return;

            compassTransform.localEulerAngles = new Vector3(0, 0, cameraTransform.eulerAngles.y);
            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i].localEulerAngles = new Vector3(0, 0, -cameraTransform.eulerAngles.y);
            }
        }
    }
}

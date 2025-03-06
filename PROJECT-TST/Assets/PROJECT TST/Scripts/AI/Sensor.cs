using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class Sensor : MonoBehaviour
    {
        public BoxCollider dectectCollider;

        [SerializeField]
        private float detectRange = 10.0f;

        // Start is called before the first frame update
        void Start()
        {
            dectectCollider.size = new Vector3(detectRange, 2.0f, detectRange);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

    }
}

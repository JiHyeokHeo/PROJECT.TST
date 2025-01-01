using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TST
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        public GameObject owner;

        public Rigidbody rigid;
        public Transform startPosition;

        public float moveForce;
        public float lifeTime;

        public void Start()
        {
            Init();
        }

        protected abstract void Init();
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TST
{
    public class ProjectileBase : MonoBehaviour
    {
        protected CharacterBase owner;

        public AmmoData data;
        public Rigidbody rigid;
        public Transform startPosition;

        public float moveForce;
        public float lifeTime;
        public int damage;

        public void Start()
        {
        }

        public virtual void Init(CharacterBase owner) 
        {

        }
        
    }
}

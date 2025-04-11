using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TST
{
    [Serializable]
    public struct ProjectileProperties
    {
        public Vector3 direction;
        public Vector3 initialPosition;
        public float initialSpeed;
        public float mass;
        public float drag;
    }

    public class BombProjectile : ProjectileBase
    {
        [SerializeField]
        TrajectoryPredictor trajectoryPredictor;

        [SerializeField]
        ProjectileProperties projectileProperties;

        public Vector3 offSetDireciton;

        private bool isThrown = false;

        protected override void Init()
        {
            rigid = GetComponent<Rigidbody>();
            if (rigid == null )
                rigid = this.AddComponent<Rigidbody>();

            startPosition = transform;

        }

        void Update()
        {
            if (trajectoryPredictor != null && !isThrown)
                trajectoryPredictor.PredictTrajectory(ProjectileData());
        }

        public void Throw()
        {
            rigid.isKinematic = false;
            rigid.AddForce(transform.forward * moveForce, ForceMode.Impulse);
            trajectoryPredictor.SetTrajectoryVisible(false);
            isThrown = true;
        }

        public void ThrowReady()
        {
            trajectoryPredictor.SetTrajectoryVisible(true);
        }

        ProjectileProperties ProjectileData()
        {
            ProjectileProperties properties = new ProjectileProperties();
            properties.direction = startPosition.forward;
            properties.initialPosition = startPosition.position;
            properties.initialSpeed = moveForce;
            properties.mass = rigid.mass;
            properties.drag = rigid.drag;

            projectileProperties = properties;

            return properties;
        }
    }
}

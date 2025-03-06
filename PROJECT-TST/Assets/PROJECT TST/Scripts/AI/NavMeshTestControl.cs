using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TST
{
    public class NavMeshTestControl : MonoBehaviour
    {
        public NavMeshAgent agent;
        public Transform targetPoint;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            agent.SetDestination(targetPoint.position);
        }
    }
}

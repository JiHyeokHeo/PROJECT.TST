using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TST
{
    public class FireDotDamageArea : MonoBehaviour
    {
        public float damageByTick = 3f;
        public float tickRate = 1f;

        private List<IDamage> insideObjects = new List<IDamage>();
        private float lastTickTime = 0f;

        private void Update()
        {
            if (Time.time < lastTickTime + tickRate)
                return;

            lastTickTime = Time.time;
            if (insideObjects.Count > 0)
            {
                for (int i = 0; i < insideObjects.Count; i++)
                {
                    insideObjects[i].ApplyDamage(damageByTick, this.gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.TryGetComponent(out IDamage damageInterface))
            {
                if (insideObjects.Exists(x => x == damageInterface))
                    return;

                insideObjects.Add(damageInterface);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.root.TryGetComponent(out IDamage damageInterface))
            {
                insideObjects.Remove(damageInterface);
            }
        }

    }
}

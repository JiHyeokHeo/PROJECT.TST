using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TST
{
    public class BulletProjectile : ProjectileBase
    {
        public bool isPlayerBullet = true;

        protected override void Init()
        {
            if (rigid == null)
                rigid = GetComponent<Rigidbody>();

            if (isPlayerBullet)
                rigid.AddForce(transform.forward * moveForce, ForceMode.Impulse);

            // 플레이어꺼가 아닌 이상 생성가 동시에 rigid.addforce 를 거기서 주고 있음

            Destroy(gameObject, lifeTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject effect = null;
            if (collision.collider.material.name.Contains("Metal"))
            {
                // Metal Effect Spawn
                effect = EffectManager.Instance.SpawnEffect(EffectType.Impact_Metal);
            }
            else if (collision.collider.material.name.Contains("Brick"))
            {
                // Dirt Effect Spawn
                effect = EffectManager.Instance.SpawnEffect(EffectType.Impact_Brick);
            }
            else
            {
                // Default Effect Spawn
                effect = EffectManager.Instance.SpawnEffect(EffectType.Impact_Dirt);
            }

            effect.transform.SetPositionAndRotation(collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));

            //if (collision.transform.root.TryGetComponent(out IDamage damageInterface))
            //{
            //    damageInterface.ApplyDamage(10, owner);
            //}

            Destroy(gameObject);
        }
    }
}

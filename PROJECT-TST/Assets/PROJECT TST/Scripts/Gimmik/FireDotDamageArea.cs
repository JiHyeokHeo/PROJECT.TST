using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class FireDotDamageArea : MonoBehaviour
    {
        public float dotTickTime = 1.0f;
        private float dotDamageElapsedTime = 0.0f;

        public float dotDamage = 10.0f;
        // 중복 방지용
        private HashSet<Action<float, GameObject>> damagedObjects = new HashSet<Action<float, GameObject>>();
        void Start()
        {
        
        }

        void Update()
        {
            if (damagedObjects.Count <= 0)
                return;

            // 도트데미지와 공격 주체자를 이벤트로 쫙 전달해보자 
            foreach (var action in damagedObjects) 
            {
                action.Invoke(dotDamage, this.gameObject);
            }

            //// 흠 이벤트를 등록해서 도트뎀을 주는 방식을 택할지.. 흠흠흠..
            //// 컨텐츠 구현에 따라 다를듯.. // 만약 도트뎀이 특정 구역을 나가도 계속해서 들어가야 하는걸 고려해서 이벤트 방식으로 변경하자
            //if (dotDamageElapsedTime >= dotTickTime)
            //{
                
            //}
            //else
            //{
            //    dotDamageElapsedTime += Time.deltaTime;
            //}
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.transform.root.TryGetComponent(out CharacterBase character))
            {
                if (damagedObjects.TryGetValue(character.ApplyDamage, out var actualAction) == false)
                    damagedObjects.Add(character.ApplyDamage);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.transform.root.TryGetComponent(out CharacterBase character))
            {
                if (damagedObjects.TryGetValue(character.ApplyDamage, out var actualAction) == true)
                    damagedObjects.Remove(character.ApplyDamage);
            }
        }

    }
}

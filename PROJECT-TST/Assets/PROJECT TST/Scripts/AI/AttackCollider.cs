using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class AttackCollider : MonoBehaviour
    {
        public CharacterBase characterBase;

        private void Start()
        {
            //// 부모에서 CharacterBase 컴포넌트 찾기 (한 번만 캐싱)
            //characterBase = GetComponentInParent<CharacterBase>();

            //if (characterBase == null)
            //{
            //    Debug.LogError("CharacterBase가 부모 오브젝트에서 발견되지 않았습니다.", this);
            //}
        }

        private void OnTriggerEnter(Collider other)
        {
            // 공격범위 안에서는 싸움 ㄱㄱ
            if (other.TryGetComponent(out IDetect detectInterface))
            {
                detectInterface.CombatDetect(characterBase.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // 공격범위 탈출하면 다시 그냥 검색 범위안에 들어온 것
            if (other.TryGetComponent(out IDetect detectInterface))
            {
                detectInterface.Detect(characterBase.gameObject);
            }
        }
    }
}

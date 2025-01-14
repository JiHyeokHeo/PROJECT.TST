using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TST
{
    public class ExplosiveDrum : MonoBehaviour, IDamage
    {
        
        public void ApplyDamage(float damage, GameObject attacker)
        {
            Debug.Log($"Damaged by {attacker.name}");

            // 데미지 연산 로직을 여기서 처리하는게 과연 옳을까?
            // 데미지 핸들러를 추가하는게 좋아보임
            // 그러면 데미지 연산은 어디서 중앙 집권 형식으로 처리하는게 좋아보이는데 질문 해봐야겠다
            // 내가 생각한 방식 : 데미지 연산처리를 하는 SingletonManager를 만듬 -> 이 매니저에다가 로직들을 추가//
            // DamageManager (damageModifier 클래스를 들고있음)
            // damageModifier를 스킬클래스나 피격자 클래스내에서 생성 후 매니저에 등록
            // 데미지매니저는 등록된 데미지 연산들을 다 처리한 후 넘기자. // 이런 방식을 해야 뭔가 네트워크 패킷 송신을 할때도 편리하지 않을까? 지난번 Swap 구현하면서 느낀 점

            // 민뎀 최뎀
            //var damage = Random.Range()
            //var isCrit = Random.Range(0, 100) < critChance;
            //if (isCrit)
            //    damage *= critMultiplier;

            SpawnsDamagePopups.Singleton.DamageDone((int)damage, transform.position, isCrit: true);
        }
    }
}

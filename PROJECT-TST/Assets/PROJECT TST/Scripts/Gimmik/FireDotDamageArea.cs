using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TST
{
    public class DotDamageHandler
    {
        public event Action<float, GameObject> OnDamage;
        public bool isDone = false;
        public float dotMaxTime = 5.0f;
        public float dotTickTime = 1.0f;
        private float dotDamageElapsedTime = 0.0f;
        private float dotLastTime = 0.0f;

        // 추후 파라미터 추가 필요
        public DotDamageHandler()
        {
            // 초기화 선언
            dotDamageElapsedTime = 0.0f;
            dotLastTime = 0.0f;
        }

        //#region DisPosable
        //class MyClass : IDisposable
        //{
        //    private bool disposed = false;

        //    public void Dispose()
        //    {
        //        Dispose(true);
        //        GC.SuppressFinalize(this); // 소멸자 호출 방지
        //    }

        //    protected virtual void Dispose(bool disposing)
        //    {
        //        if (!disposed)
        //        {
        //            if (disposing)
        //            {
        //                // 관리 리소스 해제
        //            }

        //            // 비관리 리소스 해제
        //            disposed = true;
        //        }
        //    }

        //    // 소멸자
        //    ~MyClass()
        //    {
        //        Dispose(false);
        //    }
        //}
        //#endregion
        //// C#에서 소멸자는 ~ClassName 형식으로 정의되며, 가비지 컬렉터가 객체를 정리할 때 호출됩니다.
        //성능 이슈와 더 효율적인 리소스 관리를 위해** Dispose 패턴(IDisposable)**을 사용하는 것이 일반적으로 권장됩니다.
        //특별한 이유가 없다면 소멸자를 직접 사용하는 대신 IDisposable을 구현하는 방식으로 리소스를 관리하세요.
        //~DamageHandler()
        //{

        //}

        public void UpdateTickTime()
        {
            dotDamageElapsedTime += Time.deltaTime;

            // 만약 도트 지속 시간이 최대 시간을 넘겼다면 이벤트 제거
            if (dotDamageElapsedTime >= dotMaxTime)
            {
                isDone = true;
                OnDamage -= this.InvokeDamage;
            }
        }

        public void InvokeDamage(float damage, GameObject attacker)
        {
            if (Time.time - dotLastTime >= dotTickTime)
            {
                dotLastTime = Time.time;
                OnDamage?.Invoke(damage, attacker); // 내부 연산은 각자 클래스 내에서 하는게 좋을까..? 왜냐면 상세 도트뎀 자체도 자체 클래스내에서 스탯이 존재할 가능성이 높기 때문?
            }
        }
    }

    public class FireDotDamageArea : MonoBehaviour
    {
        public float dotDamage = 10.0f;
        // 중복 방지용
        private Dictionary<CharacterBase, DotDamageHandler> damagedObjects = new Dictionary<CharacterBase, DotDamageHandler>();
        private List<CharacterBase> keysToRemove = new List<CharacterBase>();
        void Start()
        {
        
        }

        void Update()
        {
            if (damagedObjects.Count <= 0)
                return;

            // 도트데미지와 공격 주체자를 이벤트로 쫙 전달해보자 
            //// 컨텐츠 구현에 따라 다를듯.. // 만약 도트뎀이 특정 구역을 나가도 계속해서 들어가야 하는걸 고려해서 이벤트 방식으로 변경하자
            foreach (var handler in damagedObjects) 
            {
                handler.Value.UpdateTickTime();
                handler.Value.InvokeDamage(dotDamage, this.gameObject);

                if (handler.Value.isDone)
                    keysToRemove.Add(handler.Key);
            }

            foreach (var key in keysToRemove)
            {
                damagedObjects.Remove(key);
            }

            keysToRemove.Clear();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.transform.root.TryGetComponent(out CharacterBase character))
            {
                if (damagedObjects.ContainsKey(character) == false)
                {
                    var handler = new DotDamageHandler();
                    handler.OnDamage += character.ApplyDamage;
                    damagedObjects[character] = handler;
                }
            }
        }

        //private void OnTriggerExit(Collider collision)
        //{
        //    if (collision.transform.root.TryGetComponent(out CharacterBase character))
        //    {
        //        if (damagedObjects.TryGetValue(character.ApplyDamage, out var actualAction) == true)
        //            damagedObjects.Remove(character.ApplyDamage);
        //    }
        //}

    }
}

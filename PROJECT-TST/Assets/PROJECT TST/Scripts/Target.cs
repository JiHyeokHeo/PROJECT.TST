using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class Target : MonoBehaviour, IDamage
    {
        [SerializeField] private UnityEngine.CharacterController controller;
        [SerializeField] private Rigidbody rb;
        //[SerializeField] private float _size = 10;
        //[SerializeField] private float _speed = 10;
        public Rigidbody Rb => rb;
        public UnityEngine.CharacterController Controller => controller;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            controller = GetComponent<UnityEngine.CharacterController>();
        }

        void Update()
        {
            // 테스트용
            //float x = Mathf.Sin(Time.time) * 20.0f;
            //Vector3 newPos = transform.position;
            //newPos.x = x;
            //transform.position = newPos;


            // 뻘짓거리 한거임 키지마 // 몰라서 남겨둠
            //var dir = new Vector3(Mathf.Cos(Time.time * _speed) * _size, Mathf.Sin(Time.time * _speed) * _size);
            //_rb.velocity = dir;
        }
        
        public void ApplyDamage(float damage, GameObject target)
        {
            Debug.Log($"{gameObject.name} : 데미지를 입고 있습니다");
            //Destroy(gameObject);
        }
    }
}

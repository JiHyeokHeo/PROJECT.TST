using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace TST
{
    public class ItemBase : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public ItemData ItemData { get; private set; }
        public string Message => ItemData.ItemName;
        public InteractType InteractType => InteractType.Item;

        public float dropItemFlyingduration;
        public float delayAfterSequence; // 회전 움직임 이후 이펙트 생성 딜레이 시간

        public Ease movementEase;

        public void Awake()
        {
            dropItemFlyingduration = 1.0f;
            delayAfterSequence = 0.1f;
        }

        public void OnEnable()
        {
            if (GameManager.Instance)
                GameManager.Instance.OnItemGenerateEvent += ItemMovesAndRotates;
        }

        public void OnDisable()
        {
            if (GameManager.Instance)
                GameManager.Instance.OnItemGenerateEvent -= ItemMovesAndRotates;
        }

        public void Interact(GameObject go)
        {
            Destroy(gameObject);

            Debug.Log("<color=#FFFFFF>Item Interacted !!</color>");

            UserDataModel.Singleton.AddItemToInventory(ItemData);
        }

        public virtual void UseItem(CharacterBase user)
        {
            
        }

        public void ItemMovesAndRotates(GameObject target, Vector3 startPosition)
        {
            if (target != this.gameObject)
                return;

            Quaternion startRotation = transform.rotation;

            Sequence seq = DOTween.Sequence();

            // 회전
            StartSpinning();

            // 위로 이동하고
            seq.Append(transform.DOMove(startPosition + new Vector3(0, 2, 0), dropItemFlyingduration / 2f)
                .SetEase(Ease.OutQuart));

            // 다시 제자리로
            seq.Append(transform.DOMove(startPosition, dropItemFlyingduration / 2f)
                .SetEase(Ease.InQuart));

            // 내려오고 나서 멈추기
            seq.OnComplete(() =>
            {
                StopSpinning();
                transform.rotation = startRotation; // 회전 리셋

                // 그 다음 Visual 오브젝트 생성 n초뒤 생성
                DOVirtual.DelayedCall(delayAfterSequence, () =>
                {
                    if (AssetManager.Singleton.GetItemPrefab("Visual", out GameObject visualObject))
                        Instantiate(visualObject, startPosition, Quaternion.identity);
                });
            });

            // 실행
            seq.Play();
        }

        private Tween spinTween;

        public void StartSpinning()
        {
            // 회전 중복 방지
            if (spinTween != null && spinTween.IsActive()) 
                return;

            spinTween = transform.DORotate(new Vector3(540, 0, 0), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

        public void StopSpinning()
        {
            if (spinTween != null && spinTween.IsActive())
            {
                spinTween.Kill(); // 회전 중지
                transform.rotation = Quaternion.identity; // 회전 리셋 (선택)
            }
        }
    }
}
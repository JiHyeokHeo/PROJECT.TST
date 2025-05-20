using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace TST
{
    public class IndicatorUI_Item : MonoBehaviour
    {
        public Transform target;

        public GameObject insideGroup;
        public GameObject outsideGroup;
        public TextMeshProUGUI outsideText;
        public Transform outsideImageRotation;
        public bool isOutSight;
        int layerMask;
        private Camera mainCamera;

        public void Awake()
        {
            layerMask = (1 << 0) | (1 << 7) | (1 << 13);
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        public void ItemUpdate()
        {
            if (target == null)
                return;
            
            float distance = Vector3.Distance(mainCamera.transform.position, target.position);
            Vector3 direction = (target.position - mainCamera.transform.position).normalized;
            if (Physics.Raycast(mainCamera.transform.position, target.position - mainCamera.transform.position, out RaycastHit hitInfo, distance, layerMask))
            {
                if (hitInfo.transform.gameObject.layer != target.gameObject.layer 
                    || distance >= 50.0f)
                {
                    insideGroup.gameObject.SetActive(false);
                    outsideGroup.gameObject.SetActive(false);
                    return;
                }
            }
            //Debug.DrawRay(mainCamera.transform.position, direction * distance,  Color.red);

            Vector3 viewportPos = mainCamera.WorldToViewportPoint(target.position);
            if (viewportPos.z < 0 || viewportPos.x < 0.1 || viewportPos.x > 0.9 || viewportPos.y < 0.1 || viewportPos.y > 0.9 )
            {
                insideGroup.gameObject.SetActive(false);
                outsideGroup.gameObject.SetActive(true);

                distance = Vector3.Distance(Camera.main.transform.position, target.position);
                outsideText.text = $"{distance:0.0}m";


                viewportPos.x = Mathf.Clamp(viewportPos.x, 0.1f, 0.9f);
                viewportPos.y = Mathf.Clamp(viewportPos.y, 0.1f, 0.9f);
                
                // 뒤통수 부분 
                if (viewportPos.z < 0)
                    viewportPos.y = 0.1f;

                Vector3 screenPos = Camera.main.ViewportToScreenPoint(viewportPos);

                Vector3 offset = Vector3.zero;
                if (viewportPos.x >= 0.9f)
                {
                    offset.x = -100f;
                }
                else if (viewportPos.x <= 0.1f)
                {
                    offset.x = 100f;
                }

                if (viewportPos.y >= 0.9f)
                {
                    offset.y = -80f;
                }
                else if (viewportPos.y <= 0.1f)
                {
                    offset.y = 80f;
                }

                
                Vector3 localDirection = Camera.main.transform.InverseTransformDirection(direction);

                //// (좌우 방향 차이)
                float wideDiffangle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
                float heightDiffangle = Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;

                float rotationOffset = 0f;

                if (Mathf.Abs(localDirection.y) > Mathf.Abs(localDirection.x))
                {
                    // 위아래 방향이 더 강한 경우
                    if (localDirection.y > 0f)
                        rotationOffset = -90f - wideDiffangle;
                    else
                        rotationOffset = 90f - wideDiffangle;
                }
                else
                {
                    // 좌우 방향이 더 강한 경우
                    if (localDirection.x > 0f)
                        rotationOffset = 0f - heightDiffangle;
                    else
                        rotationOffset = 180f - heightDiffangle;
                }

                if (viewportPos.z < 0)
                    rotationOffset = 90f - wideDiffangle;

                outsideImageRotation.rotation = Quaternion.Euler(0f, 0f, -rotationOffset);

                // 4. 실제 포지션 적용
                transform.position = screenPos;
                outsideText.transform.localPosition = offset;
            }
            else 
            {
                insideGroup.gameObject.SetActive(true);
                outsideGroup.gameObject.SetActive(false);

                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
                transform.position = screenPos;
                transform.rotation = Quaternion.identity;
            }
        }
    }
}

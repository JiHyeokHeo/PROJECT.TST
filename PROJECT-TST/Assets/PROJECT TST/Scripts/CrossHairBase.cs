using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

namespace TST
{
    public class CrossHairBase : MonoBehaviour
    {
        public bool IsRecoilChange
        {
            get => isRecoilChange;
            set
            {
                isRecoilChange = value;
            }
        }

        public float maxPosX;
        public float maxPosY;
        public float minPosX;
        public float minPosY;

        [field: SerializeField] private List<float> initPosX;
        [field: SerializeField] private List<float> initPosY;

        public float maxPosDivideRatio;
        // 10프로씩 벌어지도록
        public float recoilStartRatio;
        public float recoilReturnRatio;

        List<RectTransform> gameObjects;

        private bool isRecoilChange;

        void Start()
        {
            maxPosDivideRatio = 10.0f;
            recoilStartRatio = 1.05f;
            recoilReturnRatio = 0.95f;
            // 1920 x 1080
            // 정사각형 비율로 변형
            float scaleWidthRatio = (float)Screen.width / Screen.height;

            maxPosX = Screen.width / maxPosDivideRatio;
            maxPosY = Screen.height * scaleWidthRatio / maxPosDivideRatio;
            minPosX = -maxPosX;
            minPosY = -maxPosY;
            gameObjects = GetComponentsInChildren<RectTransform>().Where(t => t != transform).ToList();
            
            for (int i = 0; i < gameObjects.Count; i++)
            {
                Vector3 pos = gameObjects[i].GetComponent<RectTransform>().anchoredPosition3D;
                initPosX.Add(pos.x);
                initPosY.Add(pos.y);
            }
        }

        void FixedUpdate()
        {
            // 반동 회복
            if (!isRecoilChange)
            {
                RecoilStart(false);
                return;
            }

            RecoilStart(true);
        }

        private void RecoilStart(bool isRecoilStart)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                // 비율만큼 곱해준다
                Vector3 pos = gameObjects[i].anchoredPosition3D;

                if (isRecoilStart)
                {
                    pos.x = Mathf.Lerp(pos.x, pos.x * recoilStartRatio, Time.deltaTime * 10.0f);
                    pos.y = Mathf.Lerp(pos.y, pos.y * recoilStartRatio, Time.deltaTime * 10.0f);
                }
                else
                {
                    pos.x = Mathf.Lerp(pos.x, pos.x * recoilReturnRatio, Time.deltaTime * 10.0f);
                    pos.y = Mathf.Lerp(pos.y, pos.y * recoilReturnRatio, Time.deltaTime * 10.0f);
                }

                RecoilConditionCheck(isRecoilStart, ref pos, i);

                gameObjects[i].anchoredPosition3D = new Vector3(pos.x, pos.y, 0f);
            }
        }

        private void RecoilConditionCheck(bool isRecoilStart, ref Vector3 pos, int index)
        {
            if (isRecoilStart)
            {
                if (pos.x > maxPosX)
                    pos.x = maxPosX;

                if (pos.y > maxPosY)
                    pos.y = maxPosY;

                if (pos.x < minPosX)
                    pos.x = minPosX;

                if (pos.y < minPosY)
                    pos.y = minPosY;
            }
            else
            {
                // 우측 크로스헤어
                if (Mathf.Abs(pos.x - initPosX[index]) < 1.0f && pos.x > initPosX[index])
                    pos.x = initPosX[index];

                // 좌측 크로스헤어
                if (Mathf.Abs(pos.x - initPosX[index]) < 1.0f && pos.x < initPosX[index])
                    pos.x = initPosX[index];

                // 윗 크로스헤어
                if (Mathf.Abs(pos.y - initPosY[index]) < 1.0f && pos.y > initPosY[index])
                    pos.y = initPosY[index];

                // 아래 크로스헤어
                if (Mathf.Abs(pos.y - initPosY[index]) < 1.0f && pos.y < initPosY[index])
                    pos.y = initPosY[index];


                // etc 중앙 부근

            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public enum CrossHairType
    {
        CrossHair_A,
        CrossHair_B,
        CrossHair_C,
        CrossHair_D,
    }

    [System.Serializable]
    public struct CrossHairData
    {
        public CrossHairType type;
        public GameObject prefab;
        public int UIButtonOrder;
    }

    public class OptionManager : SingletonBase<OptionManager>
    {
        public CrossHairBase UsingCrossHairComponent
        {
            get 
            {
                if (usingCrossHairComponent == null)
                    return null;

                return usingCrossHairComponent; 
            }
            private set { }
        }

        public GameObject crossHairCanvas;
        public GameObject usingCrossHair;
        public List<CrossHairData> crossHairContainer = new List<CrossHairData>();
        public GameObject UICanvas;
        private CrossHairBase usingCrossHairComponent;

        public bool IsGameStopped
        {
            get => isGameStopped;
            set
            {
                isGameStopped = value;
                if (isGameStopped)
                    Time.timeScale = 0.0f;
                else
                    Time.timeScale = 1.0f;
                // 옵션 띄우기
                ShowOption();
            }
        }

        private bool isGameStopped = false;

        public static T StringToEnum<T>(string e)
        {
            return (T)Enum.Parse(typeof(T), e);
        }

        private void Start()
        {
            // 초기값 크로스헤어 A 
            //crossHairCanvas = UIManager.Show<CrossHair_UI>(UIList.CrossHair_UI).gameObject;

            //for (int i = 0; i < crossHairCanvas.transform.childCount; i++)
            //{
            //    var childObj = crossHairCanvas.transform.GetChild(i);

            //    CrossHairType type = StringToEnum<CrossHairType>(childObj.name);
            //    CrossHairData data = new CrossHairData();
            //    data.type = type;
            //    data.prefab = childObj.gameObject;
            //    data.UIButtonOrder = 0;
            //    crossHairContainer.Add(data);
            //}

            //ChangeCrossHair(CrossHairType.CrossHair_A);
        }

        public GameObject ChangeCrossHair(CrossHairType crossHairType)
        {
            CrossHairData crossHairData = crossHairContainer.Find(x => x.type == crossHairType);
            if (crossHairData.prefab == null) 
            {
                Debug.LogError("CrossHair not found");
                return null;
            }

            return ChangeCrossHair(crossHairData.prefab) ? crossHairData.prefab : null;
        }

        private bool ChangeCrossHair(GameObject crossHair)
        {
            if (usingCrossHair != null)
                usingCrossHair.SetActive(false);

            usingCrossHair = crossHair.gameObject;
            usingCrossHairComponent = usingCrossHair.GetComponent<CrossHairBase>();

            crossHair.gameObject.SetActive(true);

            return true;
        }

        private void ShowOption()
        {
            UICanvas.SetActive(!UICanvas.activeSelf);
        }
    }
}

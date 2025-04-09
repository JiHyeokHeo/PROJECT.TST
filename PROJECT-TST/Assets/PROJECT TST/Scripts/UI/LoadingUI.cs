using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class LoadingUI : UIBase
    {
        public static LoadingUI Instance => UIManager.Singleton.GetUI<LoadingUI>(UIList.LoadingUI);

        [SerializeField] private Transform loadingIcon;
        private void Update()
        {
            loadingIcon.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 180);
        }
    }
}
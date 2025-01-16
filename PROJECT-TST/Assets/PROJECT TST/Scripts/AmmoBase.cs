using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class AmmoBase : MonoBehaviour
    {
        public AmmoData data;

        #region Bullet
        public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }

        private int currentAmmo; // 현재 탄창에 남은 총알 수
        #endregion

        public void Initialize()
        {
            currentAmmo = data.initAmmoCount;
        }
    }
}

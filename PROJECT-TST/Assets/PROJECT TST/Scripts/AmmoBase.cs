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
        public int MaxAmmo { get => maxAmmo; private set { } }

        private int currentAmmo; // ÇöÀç ÅºÃ¢¿¡ ³²Àº ÃÑ¾Ë ¼ö
        private int maxAmmo;
        #endregion

        public void Initialize()
        {
            currentAmmo = data.initAmmoCount;
            maxAmmo = data.maxAmmoCount;
        }
    }
}

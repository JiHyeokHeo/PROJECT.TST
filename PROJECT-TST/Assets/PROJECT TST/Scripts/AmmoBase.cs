using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class AmmoBase : MonoBehaviour
    {
        public AmmoData data;

        #region Bullet
        public int LoadedBulletAmount
        {
            get => loadedBulletAmount;
            set => loadedBulletAmount = value;
        }

        public int CurrentAmmo { get => currentAmmo;
            set
            {
                currentAmmo = value;
                //if (currentAmmo >= maxAmmo)
                //{
                //    currentAmmo = maxAmmo;
                //    return;
                //}
            }
        }
        //public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }

        private int loadedBulletAmount;
        private int currentAmmo; // ÇöÀç ÅºÃ¢¿¡ ³²Àº ÃÑ¾Ë ¼ö
        //private int maxAmmo;
        #endregion

        public void Initialize()
        {
            currentAmmo = data.initAmmoCount;
            loadedBulletAmount = data.initAmmoCount;
            //maxAmmo = 0;
        }
    }
}

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

        public int CurrentBulletAmount { get => currentBulletAmount;
            set
            {
                currentBulletAmount = value;
            }
        }

        private int loadedBulletAmount;
        private int currentBulletAmount; // ÇöÀç ÅºÃ¢¿¡ ³²Àº ÃÑ¾Ë ¼ö
        #endregion

        public void Initialize()
        {
            currentBulletAmount = data.initAmmoCount;
            loadedBulletAmount = 0;
        }
    }
}

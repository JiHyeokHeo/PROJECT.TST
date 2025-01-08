using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class AmmoBase : MonoBehaviour
    {
        public AmmoData data;

        #region Bullet
        public int clipSize = 10; // ÅºÃ¢ Å©±â[1ÅºÃ¢:ÃÑ¾Ë °¹¼ö]

        public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }

        private int currentAmmo; // ÇöÀç ÅºÃ¢¿¡ ³²Àº ÃÑ¾Ë ¼ö
        #endregion

        public void Initialize()
        {
            currentAmmo = clipSize;
        }
    }
}

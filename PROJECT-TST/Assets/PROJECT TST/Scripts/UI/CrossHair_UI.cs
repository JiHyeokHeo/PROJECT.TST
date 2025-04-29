using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class CrossHair_UI : UIBase
    {
        public List<CrossHairBase> crossHairs = new List<CrossHairBase>();

        public void SetCrossHairRecoil(CrossHairType type, bool isRecoilAdded)
        {
            int index = (int)type;
            if (index > crossHairs.Count)
                return;

            crossHairs[index].IsRecoilChange = isRecoilAdded;
        }

        public void ActivateCrossHair(CrossHairType type, bool isActivate) 
        {
            int index = (int)type;
            if (index > crossHairs.Count)
                return;

            crossHairs[index].gameObject.SetActive(isActivate);
        }
    }
}

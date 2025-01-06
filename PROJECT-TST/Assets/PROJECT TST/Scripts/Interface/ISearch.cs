using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public interface IDetect 
    {
        public void Detect(GameObject target);
        public void CombatDetect(GameObject target);
        public void UnDetect(GameObject target);
    }
}

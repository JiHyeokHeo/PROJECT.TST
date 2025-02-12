using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public enum BulletType
    {

    }

    public class BulletItems : MonoBehaviour
    {
        ItemBase itemBase;
        // Start is called before the first frame update
        void Start()
        {
            itemBase = GetComponent<ItemBase>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

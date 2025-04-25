using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class CraftTable : MonoBehaviour, IInteractable
    {
        public string Message => "Craft Table";

        public InteractType InteractType => InteractType.None;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Interact(GameObject go)
        {
            UIManager.Show<CraftingUI>(UIList.CraftingUI);
        }

        public void OnTriggerExit(Collider other)
        {
            UIManager.Hide<CraftingUI>(UIList.CraftingUI);
        }
    }
}

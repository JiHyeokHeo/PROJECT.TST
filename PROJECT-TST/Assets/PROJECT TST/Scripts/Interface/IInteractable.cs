using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public interface IInteractable 
    {
        public string Message { get; }
        public void Interact(GameObject go);
    }
}

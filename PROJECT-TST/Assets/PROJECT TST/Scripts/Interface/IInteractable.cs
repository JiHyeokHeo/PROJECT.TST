using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public enum InteractType
    {
        None,
        Item,
        NPC,

    }

    public interface IInteractable 
    {
        public string Message { get; }
        public InteractType InteractType { get; }
        public void Interact(GameObject go);
    }
}

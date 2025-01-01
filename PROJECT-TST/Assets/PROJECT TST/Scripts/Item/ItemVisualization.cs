using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    [RequireComponent(typeof(ItemBase))]
    public class ItemVisualization : MonoBehaviour
    {
        private ItemBase itemBase;
        private Transform visualRoot;

        private void Awake()
        {
            itemBase = GetComponent<ItemBase>();
            visualRoot = transform.Find("Visual");
        }

        private void Start()
        {
            if (itemBase.ItemData == null)
                return;

            GameObject newVisualInstance = Instantiate(itemBase.ItemData.ItemVisualPrefab, visualRoot);
            newVisualInstance.gameObject.SetActive(true);
            newVisualInstance.transform.SetLocalPositionAndRotation(
                itemBase.ItemData.ItemVisualPrefab.transform.localPosition,
                itemBase.ItemData.ItemVisualPrefab.transform.localRotation);
        }
    }
}

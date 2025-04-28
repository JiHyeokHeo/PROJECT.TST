using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class IndicatorUI : UIBase
    {
        public static IndicatorUI Instance { get; private set; }

        public IndicatorUI_Item indicatorPrefab;
        public SerializableWrapDictionary<Transform, IndicatorUI_Item> indicatorItems = new SerializableWrapDictionary<Transform, IndicatorUI_Item>();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            foreach (var item in indicatorItems)
            {
                item.Value.ItemUpdate();
            }
        }

        public void RegistIndicator(Transform target)
        {
            var newIndicatorItem = Instantiate(indicatorPrefab, transform);
            newIndicatorItem.target = target;
            newIndicatorItem.gameObject.SetActive(true);

            indicatorItems.Add(target, newIndicatorItem);
        }

        public void RemoveIndicator(Transform target)
        {
            if (indicatorItems.ContainsKey(target))
            {
                Destroy(indicatorItems[target].gameObject);
                indicatorItems.Remove(target);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TST
{
    public class IndicatorUI_Item : MonoBehaviour
    {
        public Transform target;

        public GameObject insideGroup;
        public GameObject outsideGroup;
        public TextMeshProUGUI outsideText;

        public void ItemUpdate()
        {
            if (target == null)
                return;

            Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.position);
            if (viewportPos.z < 0 || viewportPos.x < 0.1 || viewportPos.x > 0.9 || viewportPos.y < 0.1 || viewportPos.y > 0.9)
            {
                insideGroup.gameObject.SetActive(false);
                outsideGroup.gameObject.SetActive(true);

                float distance = Vector3.Distance(Camera.main.transform.position, target.position);
                outsideText.text = $"{distance:0.0}m";

                if (viewportPos.x > 0.9f)
                    viewportPos.x = 0.9f;
                if (viewportPos.y > 0.9f)
                    viewportPos.y = 0.9f;
                if (viewportPos.x < 0.1f)
                    viewportPos.x = 0.1f;
                if (viewportPos.y < 0.1f)
                    viewportPos.y = 0.1f;
                if (viewportPos.z < 0)
                    viewportPos.y = 0.1f;

                Vector3 screenPos = Camera.main.ViewportToScreenPoint(viewportPos);
                transform.position = screenPos;
            }
            else
            {
                insideGroup.gameObject.SetActive(true);
                outsideGroup.gameObject.SetActive(false);

                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
                transform.position = screenPos;
            }
        }
    }
}

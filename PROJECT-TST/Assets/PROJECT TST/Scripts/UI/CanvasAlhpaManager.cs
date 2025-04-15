using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;
using System.Linq;

namespace TST
{
    public class CanvasAlhpaManager : MonoBehaviour
    {
        public static CanvasAlhpaManager Instance { get; private set; }

        public List<CanvasGroup> uiGroups = new List<CanvasGroup>();

        [SerializeField] private float targetAlphaValue;
        [SerializeField] private float targetAlphaChangeDuration;
        
        public float TargetAlphaValue
        {
            get => targetAlphaValue;
            set
            {
                if (!Mathf.Approximately(targetAlphaValue, value))
                {
                    targetAlphaValue = value;
                    FadeAllUI();
                }
            }
        }

        public float TargetAlphaChangeDuration
        {
            get => targetAlphaChangeDuration;
            set
            {
                targetAlphaChangeDuration = value;
            }
        }

        public void Awake()
        {
            Instance = this;
            targetAlphaChangeDuration = 1.0f;
        }

        public void Start()
        {
            uiGroups.Add(UIManager.Singleton.GetUI<MainHudUI>(UIList.MainHudUI).GetComponent<CanvasGroup>());
            uiGroups.Add(UIManager.Singleton.GetUI<IngameUI>(UIList.IngameUI).GetComponent<CanvasGroup>());
            uiGroups.Add(UIManager.Singleton.GetUI<ShortCutUI>(UIList.ShortCutUI).GetComponent<CanvasGroup>());
        }

        private void FadeUI(CanvasGroup group, float targetAlpha, float targetChangeDuration = 1f)
        {
            if (group == null)
                return;

            group.DOFade(targetAlpha, targetChangeDuration);
        }

        private void FadeAllUI()
        {
            if (uiGroups == null || uiGroups.Count <= 0)
                return;

            foreach (var group in uiGroups)
            {
                group.DOFade(TargetAlphaValue, TargetAlphaChangeDuration);
            }
        }

        // 점점 밝아짐
        public void FadeIn()
        {
            TargetAlphaValue = 1f;

            // 특별한 친구들은 따로 한번 더 조절 신호 요청
            //FadeUI();
        }

        public void FadeOut()
        {
            TargetAlphaValue = 0f;

            // 특별한 친구들은 따로 한번 더 조절 신호 요청
            FadeUI(uiGroups.FirstOrDefault(x => x.name.Contains("MainHud")), 0.5f);
        }
    }
}

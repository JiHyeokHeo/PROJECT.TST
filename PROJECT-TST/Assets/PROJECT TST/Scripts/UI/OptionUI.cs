using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    enum Quality
    {
        Low,
        Medium,
        High,
    }

    public class OptionUI : UIBase
    {
        public override bool IsVisibleCursor => true;

        private int currentQuality = -1;
        private int exchangeQuality = -1;

        private float currentVolume;
        private float exchangeVolume = -1.0f;

        private bool currentMute = false;
        private bool isMute = false;

        [SerializeField] TMP_Dropdown dropdown;
        [SerializeField] TextMeshProUGUI qualityText;
        [SerializeField] Scrollbar volumeScrollBar;
        [SerializeField] Toggle muteToggle;

        public void Start()
        {
            currentQuality = GameOptionManager.Singleton.GetCurrentQuality();
            currentVolume = SoundManager.Singleton.Volume_Master;
            volumeScrollBar.value = currentVolume;

            dropdown.value = currentQuality; // currentQuality는 미리 저장된 값(int)
            //dropdown.RefreshShownValue();    // 현재 선택된 값을 강제로 업데이트함
            
            currentMute = muteToggle.isOn ? true : false;
        }

        public void OnClickReturnToLobby()
        {
            Main.Singleton.ChangeScene(SceneType.Title);
        }

        public void OnClickShutDownGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
        }

        public void OnQualityChange(int value)
        {
            exchangeQuality = value;
        }

        public void OnToggleSoundSlideBar()
        {
            exchangeVolume = volumeScrollBar.value;
            SetVolume(exchangeVolume);
        }

        public void OnClickMuteButton()
        {
            // 만약 뮤트라면 0
            exchangeVolume = muteToggle.isOn ? 0f : currentVolume;
            isMute = muteToggle.isOn ? true : false;
            SetVolume(exchangeVolume, isMute);
        }

        public void OnClickChangeButton()
        {
            if (exchangeQuality >= 0 && currentQuality != exchangeQuality)
            {
                currentQuality = exchangeQuality;
                GameOptionManager.Singleton.SetQuality(currentQuality);
            }

            currentMute = isMute;
            muteToggle.isOn = currentMute;
            currentVolume = exchangeVolume;
        }

        public void OnClickHideButton()
        {
            exchangeQuality = -1;

            if (currentVolume != exchangeVolume && currentMute == false)
            {
                exchangeVolume = -1;
                SetVolume(currentVolume);
            }
            
            if (currentMute)
            {
                SetVolume(0f);
            }
            else if (currentMute == false && isMute == true)
            {
                isMute = false;
                SetVolume(currentVolume, true);
            }

            dropdown.value = currentQuality;
            volumeScrollBar.value = currentVolume;
            muteToggle.isOn = currentMute;
            UIManager.Hide<OptionUI>(UIList.OptionUI);
        }

        public void SetVolume(float volume, bool forceToChange = false)
        {
            if (isMute && forceToChange == false)
                return;

            SoundManager.Singleton.Volume_Master = volume;
        }
    }
}

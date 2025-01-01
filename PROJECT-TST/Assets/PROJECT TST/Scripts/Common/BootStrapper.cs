using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TST
{
    // Boot Strapper의 용도는 에디터 상에서, 개발이 용이하도록 시스템을 별도로 불러주는 도우미 클래스.
    public class BootStrapper
    {
        private static List<string> AutoBootStrapperScenes = new List<string>()
        {
            // 자동으로 BootStrapper를 실행할 씬들을 추가합니다.
            "Ingame",

        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SystemBoot()
        {
#if UNITY_EDITOR
            var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (AutoBootStrapperScenes.Contains(activeScene.name))
            {
                InternalBoot();
            }
#endif
        }

        private static void InternalBoot()
        {
            // 필요한 기본 시스템 초기화
            UIManager.Singleton.Initialize();
            UserDataModel.Singleton.Initialize();

            // TODO : 추가적인 작업이나 초기화 작업을 수행하고 싶다면 여기서 수행            
            // UIManager.Show<PopupA_UI>(UIList.PopupA_UI);
            //UIManager.Show<IndicatorUI>(UIList.Indicator_UI);
            //UIManager.Show<Minimap_UI>(UIList.Minimap_UI);
            //UIManager.Show<CrossHair_UI>(UIList.CrossHair_UI);
            //UIManager.Show<IngameUI>(UIList.IngameUI);
            //UIManager.Show<InteractionUI>(UIList.InteractionUI);
            //UIManager.Show<InventoryUI>(UIList.InventoryUI);
        }

    }
}

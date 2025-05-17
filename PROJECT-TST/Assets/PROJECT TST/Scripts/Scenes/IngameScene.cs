using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TST
{
    public class IngameScene : SceneBase
    {
        // Start is called before the first frame update
        public override IEnumerator OnStart()
        {
            AsyncOperation asyncToTitle = SceneManager.LoadSceneAsync(SceneType.Ingame.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(() => asyncToTitle.isDone);

            UIManager.Show<InteractionUI>(UIList.InteractionUI);
            UIManager.Show<CrossHair_UI>(UIList.CrossHair_UI);
            UIManager.Show<MainHudUI>(UIList.MainHudUI);
            UIManager.Show<ShortCutUI>(UIList.ShortCutUI);
            UIManager.Show<IngameUI>(UIList.IngameUI);
            SoundManager.Singleton.PlayBGM("BGM_Ingame", true);
            //UIManager.Show<InventoryUI>(UIList.InventoryUI);
            //UIManager.Show<IngameUI>(UIList.IngameUI);
            //UIManager.Show<MinimapUI>(UIList.MinimapUI);
            //UIManager.Show<IndicatorUI>(UIList.IndicatorUI);
        }

        public override IEnumerator OnEnd() 
        {
            UIManager.Singleton.HideAllUI();
            SoundManager.Singleton.StopAllSound();

            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TST
{
    public class IngameMapScene : SceneBase
    {

        public override IEnumerator OnStart()
        {
            AsyncOperation asyncToTitle = SceneManager.LoadSceneAsync(SceneType.IngameMap.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(() => asyncToTitle.isDone);

            UIManager.Show<InteractionUI>(UIList.InteractionUI);
            UIManager.Show<Minimap_UI>(UIList.Minimap_UI);
            UIManager.Show<IndicatorUI>(UIList.Indicator_UI);
            UIManager.Show<CrossHair_UI>(UIList.CrossHair_UI);
            UIManager.Show<MainHudUI>(UIList.MainHudUI);
            UIManager.Show<ShortCutUI>(UIList.ShortCutUI);
            UIManager.Show<IngameUI>(UIList.IngameUI);
            SoundManager.Singleton.PlayBGM("BGM_Ingame", true);
        }

        public override IEnumerator OnEnd()
        {
            return null;
        }
    }
}

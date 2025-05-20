using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TST
{
    public class TitleScene : SceneBase
    {
        public override IEnumerator OnStart()
        {
            AsyncOperation asyncToTitle = SceneManager.LoadSceneAsync(SceneType.Title.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(() => asyncToTitle.isDone);
            UIManager.Singleton.HideAllUI();

            SoundManager.Singleton.PlayBGM("BGM_Lobby");
            SoundManager.Singleton.PlaySFX("WaterDrop", Vector3.zero);
            UIManager.Show<TitleUI>(UIList.TitleUI);
        }

        public override IEnumerator OnEnd()
        {
            UIManager.Hide<TitleUI>(UIList.TitleUI);

            yield return null;
        }
    }
}

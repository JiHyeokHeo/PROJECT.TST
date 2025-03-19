using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class GameOverUI : UIBase
    {
        public override bool IsVisibleCursor => true;

        public void OnClickRetry()
        {
            CharacterController.Instance.linkedCharacter.eventHandler.ResetCharacter();
            UIManager.Hide<GameOverUI>(UIList.GameOverUI);
        }

        public void OnClickGoBackToLobby()
        {
            Main.Singleton.ChangeScene(SceneType.Title);
            UIManager.Hide<GameOverUI>(UIList.GameOverUI);
        }
    }
}

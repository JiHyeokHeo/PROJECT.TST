using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class TitleUI : UIBase
    {
        public void OnEnterGame()
        {
            Main.Instance.ChangeScene(SceneType.Ingame);
        }

        public void OnLeaveGame()
        {
            Main.Instance.SystemQuit();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class TitleUI : UIBase
    {
        public void OnEnterGame()
        {
            Main.Singleton.ChangeScene(SceneType.Ingame);
        }

        public void OnLeaveGame()
        {
            Main.Singleton.SystemQuit();
        }
    }
}

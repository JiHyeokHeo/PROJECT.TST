using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class TitleUI : UIBase
    {
        public void OnEnterGame()
        {
            Main.Singleton.ChangeScene(SceneType.IngameMap);
        }

        public void OnLeaveGame()
        {
            Main.Singleton.SystemQuit();
        }
    }
}

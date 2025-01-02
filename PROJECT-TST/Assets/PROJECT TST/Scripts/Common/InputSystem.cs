using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class InputSystem : SingletonBase<InputSystem>
    {
        public System.Action OnInput_Jump;
        public System.Action OnInput_HelpPopupToggle;

        private void Start()
        {
            SetCursorVisible(false);
        }

        private static void SetCursorVisible(bool isVisible)
        {
            if (isVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                OnInput_HelpPopupToggle?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnInput_Jump?.Invoke();
            }

            if (OptionManager.Singleton.IsGameStopped)
            {
                SetCursorVisible(true);
                return;
            }

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                SetCursorVisible(true);
            }
            else
            {
                SetCursorVisible(false);
            }
        }
    }
}

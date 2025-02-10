using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

namespace TST
{
    public class UnitaskTest : MonoBehaviour
    {
        private CancellationTokenSource cancel = new CancellationTokenSource();
        // Start is called before the first frame update
        private async void Start()
        {
            TestUni(cancel.Token).Forget();
        }

        public void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    cancel.Cancel();
            //}
        }

        private async UniTask TestUni(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await UniTask.Delay(3000, true, cancellationToken: cancellationToken);
                Debug.Log("TestUni");
            }
        }
    }
}

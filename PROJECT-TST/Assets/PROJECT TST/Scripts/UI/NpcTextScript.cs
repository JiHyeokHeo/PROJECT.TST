using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TST
{
    public class NpcTextScript : UIBase
    {
        public override bool IsVisibleCursor => true;

        public NPCController npcController;
        public TextMeshProUGUI npcName;
        public TextMeshProUGUI npcDialogue;
        public Button nextButton;

        private Dictionary<string, string> npcDialogues = new Dictionary<string, string>()
        {
            { "StartNpc", "s:StartNpc 안녕하세요, ProjectTST 제작자 허지혁입니다./3인칭 FPS, RPG 특징을 합친 게임입니다./대화를 종료합니다." },
            { "EndingNpc", "s:Player 여기가 탈출 지점이 맞아?/s:EndingNpc 어서 올라타 !/대화를 종료합니다." }
        };

        private List<string> currentScript = new List<string>();

        private int currentIndex = 0;

        public Action OnDialogueFinished;

        public void RegisterDialogue(NPCController controller)
        {
            npcController = controller;
            SetupDialogueByNpcName(npcController.NpcName);
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            //nextButton.onClick.AddListener(ShowNextDialogue);
        }

        private void OnDisable()
        {
            //nextButton.onClick.RemoveListener(ShowNextDialogue);
        }

        public void SetupDialogueByNpcName(string npcId)
        {
            if (npcDialogues.TryGetValue(npcId, out string dialogue))
            {
                currentScript.Clear();
                currentScript.AddRange(dialogue.Split('/'));
                currentIndex = 0;
                ShowNextDialogue();
            }
            else
            {
                Debug.LogWarning($"[{npcId}]에 대한 대사가 등록되어 있지 않습니다.");
            }
        }

        private string talkingNpcName;
        public void ShowNextDialogue()
        {
            if (currentIndex >= currentScript.Count)
            {
                // 대화 종료 처리
                npcDialogue.text = "";
                npcName.text = "";

                OnDialogueFinished?.Invoke();
                UIManager.Hide<NpcTextScript>(UIList.NpcScriptUI);
                return;
            }

            string line = currentScript[currentIndex];
            if (line.StartsWith("s:"))
            {
                var split = line.Substring(2).Split(' ');
                talkingNpcName = split[0];
                npcName.text = split[0];
                npcDialogue.text = string.Join(" ", split, 1, split.Length - 1);
            }
            else
            {
                npcName.text = talkingNpcName;
                npcDialogue.text = line;
            }

            currentIndex++;
        }
    }
}

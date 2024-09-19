using SOF.Scripts.Etc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    /// <summary>
    /// UI 전체를 관리하는 매니저
    /// 팝업과 버튼을 매핑, 스택을 사용하여 팝업창 관리
    /// </summary>
    public class UIManager : SingletonLazy<UIManager>
    {
        [HideInInspector]
        public Transform canvasTransform;

        public Dictionary<string, UIPopUp> popUpUIDictionary = new();       // 팝업 이름과 UIPopUp 객체를 매핑하는 Dictionary
        public Dictionary<Button, string> buttonPopUpMapping = new();       // 버튼과 팝업 이름을 매핑하는 Dictionary
        public Stack<UIPopUp> currentPopUpUI = new Stack<UIPopUp>();        // 현재 활성화된 팝업을 관리하는 스택

        /// <summary>
        /// 캔버스 할당, 팝업 찾기 및 등록, 버튼과 팝업 매핑
        /// </summary>
        private void Awake()
        {
            Debug.Log("UIManager 실행");
            canvasTransform = FindObjectOfType<Canvas>().transform;

            if (canvasTransform == null)
                Debug.Log("Canvas 없음");
            else
                Debug.Log("성공적으로 할당");

            // UIPopUp을 찾아서 Dictionary에 등록
            UIPopUp[] uIPopUps = canvasTransform.GetComponentsInChildren<UIPopUp>(true);

            foreach (var popUp in uIPopUps)
            {
                Debug.Log($"UIPopUp 발견 : {popUp.name}");
                if (!popUpUIDictionary.ContainsKey(popUp.name))
                    popUpUIDictionary.Add(popUp.name, popUp);
                else
                    Debug.LogWarning($"{popUp.name} <- 이미 존재하는 PopUp UI입니다.");
            }

            // Button을 찾아서 팝업과 매핑
            Button[] uIButtons = canvasTransform.GetComponentsInChildren<Button>();

            foreach (var button in uIButtons)
            {
                string popUpName = button.gameObject.name.Replace("Button", "PopUp");
                Debug.Log($"버튼 : {button.gameObject.name} -> 매핑된 팝업 : {popUpName}");
                if (popUpUIDictionary.ContainsKey(popUpName))
                {
                    buttonPopUpMapping.Add(button, popUpName);
                    button.onClick.AddListener(() => ShowPopUp(popUpName));
                    Debug.Log($"{popUpName} 팝업을 {button.gameObject.name} 버튼에 연결");
                }
                else
                    Debug.Log($"{popUpName} <- 해당 팝업을 찾을 수 없음");
            }
        }

        /// <summary>
        /// ESC 키 입력 처리
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentPopUpUI.Count > 0)
                {
                    HidePopUp();
                }
            }
        }

        /// <summary>
        /// 버튼 클릭시 호출
        /// </summary>
        /// <param name="popUpName"> 팝업 이름 </param>
        public void ShowPopUp(string popUpName)
        {
            if (popUpUIDictionary.TryGetValue(popUpName, out UIPopUp popUp))
            {
                if (!popUp.IsActive())
                {
                    Debug.Log($"ShowPopUp 호출 : {popUpName}");
                    popUp.ShowPanel();
                    currentPopUpUI.Push(popUp);
                }
            }
            else
                Debug.LogWarning($"{popUpName} <- 해당 이름의 팝업UI를 찾을 수 없습니다.");
        }

        /// <summary>
        /// 가장 최근에 열린 팝업을 닫음
        /// </summary>
        public void HidePopUp()
        {
            if (currentPopUpUI.Count > 0)
            {
                UIPopUp popUp = currentPopUpUI.Pop();
                popUp.HidePanel();
            }
        }

        /// <summary>
        /// 스택에 있는 모든 팝업을 닫음
        /// </summary>
        public void HideAllPopUp()
        {
            if (currentPopUpUI.Count <= 0)
                return;

            var popUpStackCopy = new Stack<UIPopUp>(currentPopUpUI);

            foreach (var popUp in popUpStackCopy)
            {
                currentPopUpUI.TryPop(out _);
                popUp.HidePanel();
            }
        }
    }
}
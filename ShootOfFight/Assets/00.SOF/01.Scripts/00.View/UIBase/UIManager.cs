using SOF.Scripts.Etc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    public class UIManager : SingletonLazy<UIManager>
    {
        [HideInInspector]
        public Transform canvasTransform;

        public Dictionary<string, UIPopUp> popUpUIDictionary = new();
        public Dictionary<Button, string> buttonPopUpMapping = new();
        public Stack<UIPopUp> currentPopUpUI = new Stack<UIPopUp>();

        private void Awake()
        {
            Debug.Log("UIManager 실행");
            canvasTransform = FindObjectOfType<Canvas>().transform;

            if (canvasTransform == null)
                Debug.Log("Canvas 없음");
            else
                Debug.Log("성공적으로 할당");

            UIPopUp[] uIPopUps = canvasTransform.GetComponentsInChildren<UIPopUp>(true);

            foreach (var popUp in uIPopUps)
            {
                Debug.Log($"UIPopUp 발견 : {popUp.name}");
                if (!popUpUIDictionary.ContainsKey(popUp.name))
                    popUpUIDictionary.Add(popUp.name, popUp);
                else
                    Debug.LogWarning($"{popUp.name} <- 이미 존재하는 PopUp UI입니다.");
            }

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

        public void HidePopUp()
        {
            if (currentPopUpUI.Count > 0)
            {
                UIPopUp popUp = currentPopUpUI.Pop();
                popUp.HidePanel();
            }
        }

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
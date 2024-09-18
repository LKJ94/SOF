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
            Debug.Log("UIManager ����");
            canvasTransform = FindObjectOfType<Canvas>().transform;

            if (canvasTransform == null)
                Debug.Log("Canvas ����");
            else
                Debug.Log("���������� �Ҵ�");

            UIPopUp[] uIPopUps = canvasTransform.GetComponentsInChildren<UIPopUp>(true);

            foreach (var popUp in uIPopUps)
            {
                Debug.Log($"UIPopUp �߰� : {popUp.name}");
                if (!popUpUIDictionary.ContainsKey(popUp.name))
                    popUpUIDictionary.Add(popUp.name, popUp);
                else
                    Debug.LogWarning($"{popUp.name} <- �̹� �����ϴ� PopUp UI�Դϴ�.");
            }

            Button[] uIButtons = canvasTransform.GetComponentsInChildren<Button>();

            foreach (var button in uIButtons)
            {
                string popUpName = button.gameObject.name.Replace("Button", "PopUp");
                Debug.Log($"��ư : {button.gameObject.name} -> ���ε� �˾� : {popUpName}");
                if (popUpUIDictionary.ContainsKey(popUpName))
                {
                    buttonPopUpMapping.Add(button, popUpName);
                    button.onClick.AddListener(() => ShowPopUp(popUpName));
                    Debug.Log($"{popUpName} �˾��� {button.gameObject.name} ��ư�� ����");
                }
                else
                    Debug.Log($"{popUpName} <- �ش� �˾��� ã�� �� ����");
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
                    Debug.Log($"ShowPopUp ȣ�� : {popUpName}");
                    popUp.ShowPanel();
                    currentPopUpUI.Push(popUp);
                }
            }
            else
                Debug.LogWarning($"{popUpName} <- �ش� �̸��� �˾�UI�� ã�� �� �����ϴ�.");
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
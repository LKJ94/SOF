using SOF.Scripts.Etc;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    /// <summary>
    /// UI ��ü�� �����ϴ� �Ŵ���
    /// �˾��� ��ư�� ����, ������ ����Ͽ� �˾�â ����
    /// </summary>
    public class UIManager : SingletonLazy<UIManager>
    {
        [HideInInspector]
        public Transform canvasTransform;

        public Dictionary<string, UIPopUp> popUpUIDictionary = new();       // �˾� �̸��� UIPopUp ��ü�� �����ϴ� Dictionary
        public Dictionary<Button, string> buttonPopUpMapping = new();       // ��ư�� �˾� �̸��� �����ϴ� Dictionary
        public Stack<UIPopUp> currentPopUpUI = new Stack<UIPopUp>();        // ���� Ȱ��ȭ�� �˾��� �����ϴ� ����

        /// <summary>
        /// ĵ���� �Ҵ�, �˾� ã�� �� ���, ��ư�� �˾� ����
        /// </summary>
        private void Awake()
        {
            Debug.Log("UIManager ����");
            canvasTransform = FindObjectOfType<Canvas>().transform;

            if (canvasTransform == null)
                Debug.Log("Canvas ����");
            else
                Debug.Log("���������� �Ҵ�");

            // UIPopUp�� ã�Ƽ� Dictionary�� ���
            UIPopUp[] uIPopUps = canvasTransform.GetComponentsInChildren<UIPopUp>(true);

            foreach (var popUp in uIPopUps)
            {
                Debug.Log($"UIPopUp �߰� : {popUp.name}");
                if (!popUpUIDictionary.ContainsKey(popUp.name))
                    popUpUIDictionary.Add(popUp.name, popUp);
                else
                    Debug.LogWarning($"{popUp.name} <- �̹� �����ϴ� PopUp UI�Դϴ�.");
            }

            // Button�� ã�Ƽ� �˾��� ����
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

        /// <summary>
        /// ESC Ű �Է� ó��
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
        /// ��ư Ŭ���� ȣ��
        /// </summary>
        /// <param name="popUpName"> �˾� �̸� </param>
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

        /// <summary>
        /// ���� �ֱٿ� ���� �˾��� ����
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
        /// ���ÿ� �ִ� ��� �˾��� ����
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
using SOF.Scripts.Etc;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    /// <summary>
    /// UI ��ü�� �����ϴ� �Ŵ���
    /// 1. �˾��� ��ư�� ����, ������ ����Ͽ� �˾�â ����
    /// 2. InputField ����
    /// </summary>
    public class UIManager : SingletonLazy<UIManager>
    {
        [HideInInspector]
        public Transform canvasTransform;

        private Dictionary<string, UIPopup> _popupUIDictionary = new();               // �˾� �̸��� UIPopUp ��ü�� �����ϴ� Dictionary
        private Dictionary<int, UIScene> _sceneUIDictionary = new();                  // �� �� ��ȣ�� ���� UIScene�� �����ϴ� Dictionary
        private Dictionary<Button, string> _buttonPopupMapping = new();               // ��ư�� �˾� �̸��� �����ϴ� Dictionary
        private Dictionary<string, Action> _buttonActionMapping = new();              // ��ư�� �ش� ������ �����ϴ� Dictionary
        private Dictionary<string, InputField> _inputFieldDictionary = new();         // InputField�� ������ Dictionary
        private Stack<UIPopup> _currentPopupUI = new Stack<UIPopup>();                // ���� Ȱ��ȭ�� �˾��� �����ϴ� ����
        private UIScene _currentSceneUI;                                              // ���� Ȱ��ȭ�� UIScene

        /// <summary>
        /// ĵ���� �Ҵ�, �˾� ã�� �� ���, ��ư�� �˾� ����, UI�� �� ����
        /// </summary>
        private void Awake()
        {
            Debug.Log("UIManager ����");

            SetupCanvas();
            SetupUIPopup();
            SetupInputField();
            SetupButtonPopupMappoing();
            SetupButtonActionMapping();
            SetupUIScene();
        }

        /// <summary>
        /// ESC Ű �Է� ó��
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_currentPopupUI.Count > 0)
                {
                    HidePopUp();
                }
            }
        }

        #region Awake Method

        /// <summary>
        /// Canvas �Ҵ�
        /// </summary>
        private void SetupCanvas()
        {
            canvasTransform = FindObjectOfType<Canvas>().transform;

            if (canvasTransform == null)
                Debug.Log("Canvas ����");
            else
                Debug.Log("���������� �Ҵ�");
        }

        /// <summary>
        /// �˾� UI ����
        /// </summary>
        private void SetupUIPopup()
        {
            // UIPopup�� ã�Ƽ� Dictionary�� ���
            UIPopup[] uIPopUps = canvasTransform.GetComponentsInChildren<UIPopup>(true);

            foreach (var popUp in uIPopUps)
            {
                Debug.Log($"UIPopUp �߰� : {popUp.name}");
                if (!_popupUIDictionary.ContainsKey(popUp.name))
                    _popupUIDictionary.Add(popUp.name, popUp);
                else
                    Debug.LogWarning($"{popUp.name} <- �̹� �����ϴ� PopUp UI�Դϴ�.");
            }
        }

        /// <summary>
        /// InputField ����
        /// </summary>
        private void SetupInputField()
        {
            // InoutField�� ã�Ƽ� Dictionary�� ��� ** UIScene ���� �� �Ǵ��� Ȯ���غ���
            InputField[] uIInputFields = canvasTransform.GetComponentsInChildren<InputField>();

            foreach (var field in uIInputFields)
            {
                Debug.Log($"UIInputField �߰� : {field.name}");
                string fieldName = field.gameObject.name;
                if (!_inputFieldDictionary.ContainsKey(fieldName))
                {
                    _inputFieldDictionary.Add(fieldName, field);
                    Debug.Log($"InputField ��ϵ� : {fieldName}");
                }
            }
        }

        /// <summary>
        /// ��ư�� �˾� ����
        /// </summary>
        private void SetupButtonPopupMappoing()
        {
            // Button�� ã�Ƽ� �˾��� ����
            Button[] uIButtons = canvasTransform.GetComponentsInChildren<Button>();

            foreach (var button in uIButtons)
            {
                string popUpName = button.gameObject.name.Replace("Button", "PopUp");
                Debug.Log($"��ư : {button.gameObject.name} -> ���ε� �˾� : {popUpName}");
                if (_popupUIDictionary.ContainsKey(popUpName))
                {
                    _buttonPopupMapping.Add(button, popUpName);
                    button.onClick.AddListener(() => ShowPopUp(popUpName));
                    Debug.Log($"{popUpName} �˾��� {button.gameObject.name} ��ư�� ����");
                }
                else
                    Debug.Log($"{popUpName} <- �ش� �˾��� ã�� �� ����");
            }
        }

        /// <summary>
        /// ��ư�� ���� ����
        /// </summary>
        private void SetupButtonActionMapping()
        {
            // Button�� �ش� ������ ����
            Button[] uIButtonActions = canvasTransform.GetComponentsInChildren<Button>();

            foreach (var button in uIButtonActions)
            {
                string actionName = button.gameObject.name;
                if (_buttonActionMapping.ContainsKey(actionName))
                {
                    button.onClick.AddListener(() => _buttonActionMapping[actionName]?.Invoke());
                    Debug.Log($"{actionName} ��ư�� ������ ��ϵǾ����ϴ�.");
                }
                else
                    Debug.Log($"{actionName}�� ���� ������ ��ϵ��� �ʾҽ��ϴ�.");
            }
        }

        /// <summary>
        /// UIScene�� �� ����
        /// </summary>
        /// <param name="sceneNumber"></param>
        private void SetupUIScene()
        {
            UIScene[] uIScenes = canvasTransform.GetComponentsInChildren<UIScene>(true);
            for (int i = 0; i < uIScenes.Length; i++)
            {
                _sceneUIDictionary[i] = uIScenes[i];
            }
        }
        #endregion

        /// <summary>
        /// ��ư Ŭ���� ȣ��
        /// </summary>
        /// <param name="popUpName"> �˾� �̸� </param>
        public void ShowPopUp(string popUpName)
        {
            if (_popupUIDictionary.TryGetValue(popUpName, out UIPopup popUp))
            {
                if (!popUp.IsActive())
                {
                    Debug.Log($"ShowPopUp ȣ�� : {popUpName}");
                    popUp.ShowPanel();
                    _currentPopupUI.Push(popUp);
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
            if (_currentPopupUI.Count > 0)
            {
                UIPopup popUp = _currentPopupUI.Pop();
                popUp.HidePanel();
            }
        }

        /// <summary>
        /// ���ÿ� �ִ� ��� �˾��� ����
        /// </summary>
        public void HideAllPopUp()
        {
            if (_currentPopupUI.Count <= 0)
                return;

            var popUpStackCopy = new Stack<UIPopup>(_currentPopupUI);

            foreach (var popUp in popUpStackCopy)
            {
                _currentPopupUI.TryPop(out _);
                popUp.HidePanel();
            }
        }

        /// <summary>
        /// InputField�� �ؽ�Ʈ �� ��������
        /// </summary>
        /// <param name="fieldName"> InputField �̸� </param>
        /// <returns> inputfield.text </returns>
        public string GetInputFieldValue(string fieldName)
        {
            if (_inputFieldDictionary.TryGetValue(fieldName, out InputField inputfield))
            {
                return inputfield.text;
            }
            Debug.Log($"{fieldName} �ʵ带 ã�� �� ����");
            return string.Empty;
        }

        /// <summary>
        /// InputField�� ���� ����
        /// </summary>
        /// <param name="fieldName"> InputField �̸� </param>
        /// <param name="value"> �� </param>
        public void SetInputFieldValue(string fieldName, string value)
        {
            if (_inputFieldDictionary.TryGetValue(fieldName, out InputField inputfield))
            {
                inputfield.text = value;
            }
            else
                Debug.Log($"{fieldName} �ʵ带 ã�� �� ����");
        }

        public void SetButtonActions()
        {
             // ��ư ������ �� ����ٰ� ������ ��
        }

        public void LoadSceneUI(int sceneNumber)
        {
            if (_currentSceneUI != null)
                _currentSceneUI.HideUI();

            if (_sceneUIDictionary.TryGetValue(sceneNumber, out UIScene newSceneUI))
            {
                newSceneUI.ShowUI();
                _currentSceneUI = newSceneUI;
                Debug.Log($"�� {sceneNumber}�� �´� UI�� �ε���");
            }
            else
                Debug.Log($"�� {sceneNumber}�� �´� UIScene�� ã�� �� ����");
        }
    }
}
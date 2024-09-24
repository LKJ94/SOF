using SOF.Scripts.Etc;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    /// <summary>
    /// UI ��ü�� �����ϴ� �Ŵ���
    /// 1. �˾��� ��ư�� ����, ������ ����Ͽ� �˾�â ����
    /// 2. InputField ����
    /// 3. Button ����
    /// </summary>
    public class UIManager : SingletonLazy<UIManager>
    {
        [HideInInspector]
        public Transform canvasTransform;

        private Dictionary<string, UIPopup> _popupUIDictionary = new();               // �˾� �̸��� UIPopUp ��ü�� �����ϴ� Dictionary
        private Dictionary<int, UIScene> _sceneUIDictionary = new();                  // �� �� ��ȣ�� ���� UIScene�� �����ϴ� Dictionary
        private Dictionary<string, UIButton> _buttonPopupMapping = new();             // ��ư�� �˾� �̸��� �����ϴ� Dictionary
        private Dictionary<string, Action> _buttonActionMapping = new();              // ��ư�� �ش� ������ �����ϴ� Dictionary
        private Dictionary<string, InputField> _inputFieldDictionary = new();         // InputField�� ������ Dictionary
        private Dictionary<string, TextMeshProUGUI> _textDictionary = new();          // Text�� ������ Dictionary
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
            SetupText();
            SetupUIButtonComponents();
            SetupButtonPopupMapping();
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

        private void SetupText()
        {
            TextMeshProUGUI[] texts = canvasTransform.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var text in texts)
            {
                Debug.Log($"UIText �߰� : {text.name}");
                string fieldName = text.gameObject.name;
                if (!_textDictionary.ContainsKey(fieldName))
                {
                    _textDictionary.Add(fieldName, text);
                    Debug.Log($"Text ��ϵ� : {fieldName}");
                }
            }
        }

        private void SetupUIButtonComponents()
        {
            Button[] buttons = canvasTransform.GetComponentsInChildren<Button>();

            foreach (var button in buttons)
            {
                UIButton uIButton = button.GetComponent<UIButton>();
                if (uIButton = null)
                    uIButton = button.gameObject.AddComponent<UIButton>();
            }
        }

        /// <summary>
        /// ��ư�� �˾� ����
        /// </summary>
        private void SetupButtonPopupMapping()
        {
            // Button�� ã�Ƽ� �˾��� ����
            UIButton[] uIButtons = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtons)
            {
                string popupName = button.gameObject.name.Replace("Button", "Popup");
                Debug.Log($"��ư : {button.gameObject.name} -> ���ε� �˾� : {popupName}");
                if (_popupUIDictionary.ContainsKey(popupName))
                {
                    _buttonPopupMapping.Add(popupName, button);
                    button.SetPopupName(popupName);
                    Debug.Log($"{popupName} �˾��� {button.gameObject.name} ��ư�� ����");
                }
                else
                    Debug.Log($"{popupName} <- �ش� �˾��� ã�� �� ����");
            }
        }

        /// <summary>
        /// ��ư�� ���� ����
        /// </summary>
        private void SetupButtonActionMapping()
        {
            // Button�� �ش� ������ ����
            UIButton[] uIButtonActions = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtonActions)
            {
                string actionName = button.gameObject.name;
                if (_buttonActionMapping.ContainsKey(actionName))
                {
                    button.SetButtonAction(_buttonActionMapping[actionName]);
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
        /// ��ư Ŭ���� �˾� ȣ��
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
                inputfield.text = value;
            else
                Debug.Log($"{fieldName} �ʵ带 ã�� �� ����");
        }

        public void SetTextValue(string fieldName, string value)
        {
            if (_textDictionary.TryGetValue(fieldName, out TextMeshProUGUI text))
                text.text = value;
            else
                Debug.Log($"{fieldName} Text�� ã�� �� ����");
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

        public void SetButtonActions()
        {
            // ��ư ������ �� ����ٰ� ������ ��
        }
    }
}
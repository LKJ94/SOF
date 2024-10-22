using SOF.Scripts.Etc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// ���� UI ��� ��Ģ ���
/// ��ư	    {Prefix}_{Action}Button 
/// �˾�	    {Prefix}_{Action}Popup 
/// ��ũ��Ʈ	On{Prefix}Button 
/// �޼���	On{ButtonName}Click 
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

        private Dictionary<string, UIPopUp> _popupUIDictionary = new();               // �˾� �̸��� UIPopUp ��ü�� �����ϴ� Dictionary
        private Dictionary<int, UIScene> _sceneUIDictionary = new();                  // �� �� ��ȣ�� ���� UIScene�� �����ϴ� Dictionary
        private Dictionary<string, UIButton> _buttonPopupMapping = new();             // ��ư�� �˾� �̸��� �����ϴ� Dictionary
        private Dictionary<string, Action> _buttonActionMapping = new();              // ��ư�� �ش� ������ �����ϴ� Dictionary
        private Dictionary<string, InputField> _inputFieldDictionary = new();         // InputField�� ������ Dictionary
        private Dictionary<string, TextMeshProUGUI> _textDictionary = new();          // Text�� ������ Dictionary
        private Stack<UIPopUp> _currentPopupUI = new();                               // ���� Ȱ��ȭ�� �˾��� �����ϴ� ����
        private UIScene _currentSceneUI;                                              // ���� Ȱ��ȭ�� UIScene
        private Dictionary<string, Type> _buttonPrefixToScriptMapping = new();        // ���λ�� ��ũ��Ʈ Ÿ���� ����(��ư)
        private Dictionary<string, Type> _popupPrefixToScriptMapping = new();         // ���λ�� ��ũ��Ʈ Ÿ���� ����(�˾�)

        /// <summary>
        /// ĵ���� �Ҵ�, �˾� ã�� �� ���, ��ư�� �˾� ����, UI�� �� ����
        /// </summary>
        private void Awake()
        {
            Debug.Log("UIManager ����");

            SetUpCanvas();
            SetUpUIPopUp();
            SetUpInputField();
            SetUpText();
            SetUpUIScene();
            SetUpUIButtonComponent();
            SetUpUIPopUpComponent();
            SetUpButtonPopupMapping();
            SetUpButtonActionMapping();
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
        private void SetUpCanvas()
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
        private void SetUpUIPopUp()
        {
            // "PopUps" �θ� ã��
            Transform popupsParent = canvasTransform.Find("PopUps");
            if (popupsParent == null)
            {
                Debug.Log("�θ� PopUps GameObject�� ã�� �� ����");
                return;
            }

            // "PopUps" �θ� �Ʒ� ��� �ڽ� ���� ������Ʈ �˻�
            foreach (Transform child in popupsParent.GetComponentsInChildren<Transform>(true))
            {
                GameObject obj = child.gameObject;

                if (obj == popupsParent.gameObject)
                    continue;

                string popupName = obj.name;
                if (!_popupUIDictionary.ContainsKey(popupName))
                {
                    UIPopUp popupComponent = obj.GetComponent<UIPopUp>();
                    if (popupComponent == null)
                    {
                        popupComponent = obj.AddComponent<UIPopUp>();
                        Debug.Log($"UIPopUp ��ũ��Ʈ�� {popupName}�� �ڵ����� �߰���");
                    }

                    _popupUIDictionary.Add(popupName, popupComponent);
                    Debug.Log($"{popupName} �˾��� Dictionary�� �߰���");
                }
                        
            }
        }

        /// <summary>
        /// InputField ����
        /// </summary>
        private void SetUpInputField()
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
        /// Text ����
        /// </summary>
        private void SetUpText()
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

        /// <summary>
        /// PopUp�� Custom ��ũ��Ʈ �߰�
        /// </summary>
        private void SetUpUIPopUpComponent()
        {
            SetUpPopUpPrefixToScriptMapping();

            UIPopUp[] popups = canvasTransform.GetComponentsInChildren<UIPopUp>();

            foreach (var popup in popups)
            {
                string popupName = popup.gameObject.name;
                bool matched = false;

                foreach (var prefix in _popupPrefixToScriptMapping.Keys)
                {
                    if (popupName.StartsWith(prefix))
                    {
                        Type scriptType = _popupPrefixToScriptMapping[prefix];

                        if (popup.GetComponent(scriptType) == null)
                        {
                            popup.gameObject.AddComponent(scriptType);
                            Debug.Log($"{scriptType.Name}�� {popupName}�� �Ҵ��");
                        }

                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    UIPopUp uIPopup = popup.GetComponent<UIPopUp>();
                    if (uIPopup == null)
                    {
                        uIPopup = popup.gameObject.AddComponent<UIPopUp>();
                        Debug.Log($"UIPopUp�� {popupName}�� �Ҵ��");
                    }
                }
            }
        }

        /// <summary>
        /// Button�� Custom ��ũ��Ʈ �߰�
        /// </summary>
        private void SetUpUIButtonComponent()
        {
            SetUpButtonPrefixToScriptMapping();

            Button[] buttons = canvasTransform.GetComponentsInChildren<Button>();

            foreach (var button in buttons)
            {
                string buttonName = button.gameObject.name;
                bool matched = false;

                foreach (var prefix in _buttonPrefixToScriptMapping.Keys)
                {
                    if (buttonName.StartsWith(prefix))
                    {
                        Type scriptType = _buttonPrefixToScriptMapping[prefix];

                        if (button.GetComponent(scriptType) == null)
                        {
                            button.gameObject.AddComponent(scriptType);
                            Debug.Log($"{scriptType.Name}�� {buttonName}�� �Ҵ��");
                        }

                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    UIButton uIButton = button.GetComponent<UIButton>();
                    if (uIButton == null)
                    {
                        uIButton = button.gameObject.AddComponent<UIButton>();
                        Debug.Log($"UIButton�� {buttonName}�� �Ҵ��");
                    }
                }
            }
        }

        private void SetUpPopUpPrefixToScriptMapping()
        {
            var popupTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(UIPopUp)) && t != typeof(UIPopUp));

            foreach (var type in popupTypes)
            {
                string scriptName = type.Name;
                if (scriptName.StartsWith("On") && scriptName.EndsWith("PopUp"))
                {
                    string prefix = scriptName.Substring(2, scriptName.Length - 2 - 5) + "_";
                    if (!_popupPrefixToScriptMapping.ContainsKey(prefix))
                    {
                        _popupPrefixToScriptMapping.Add(prefix, type);
                        Debug.Log($"���� �߰� : {prefix} -> {type.Name}");
                    }
                    else
                        Debug.Log($"���λ� {prefix}�� �̹� ���εǾ� ����");
                }
                else
                    Debug.Log($"��ũ��Ʈ ��� ��Ģ�� ������ �ʾ��� -> {scriptName}");
            }

            foreach (var mapping in _popupPrefixToScriptMapping)
                Debug.Log($"���� : {mapping.Key} -> {mapping.Value.Name}");
        }

        /// <summary>
        /// ��ũ��Ʈ �ڵ� ����(��ư)
        /// </summary>
        private void SetUpButtonPrefixToScriptMapping()
        {
            var buttonTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(UIButton)) && t != typeof(UIButton));

            foreach (var type in buttonTypes)
            {
                string scriptName = type.Name;
                if (scriptName.StartsWith("On") && scriptName.EndsWith("Button"))
                {
                    string prefix = scriptName.Substring(2, scriptName.Length - 2 - 6) + "_";
                    if (!_buttonPrefixToScriptMapping.ContainsKey(prefix))
                    {
                        _buttonPrefixToScriptMapping.Add(prefix, type);
                        Debug.Log($"���� �߰� : {prefix} -> {type.Name}");
                    }
                    else
                        Debug.Log($"���λ� {prefix}�� �̹� ���εǾ� ����");
                }
                else
                    Debug.Log($"��ũ��Ʈ ��� ��Ģ�� ������ �ʾ��� -> {scriptName}");
            }

            foreach (var mapping in _buttonPrefixToScriptMapping)
                Debug.Log($"���� : {mapping.Key} -> {mapping.Value.Name}");
        }

        /// <summary>
        /// ��ư�� �˾� ����
        /// </summary>
        private void SetUpButtonPopupMapping()
        {
            // Button�� ã�Ƽ� �˾��� ����
            UIButton[] uIButtons = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtons)
            {
                string popupName = button.gameObject.name.Replace("Button", "PopUp");
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
        private void SetUpButtonActionMapping()
        {
            string targetNamespace = "SOF.Scripts.View";
            MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>()
                .Where(m => m.GetType().Namespace == targetNamespace)
                .ToArray();

            foreach (var monoBehaviour in monoBehaviours)
            {
                Type type = monoBehaviour.GetType();
                MethodInfo[] methods =type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var method in methods)
                {
                    if (method.Name.StartsWith("On") && method.Name.EndsWith("Click") && method.GetParameters().Length == 0)
                    {
                        string buttonName = method.Name.Substring(2, method.Name.Length - 2 - 5);

                        if (!_buttonActionMapping.ContainsKey(buttonName))
                        {
                            Action action = () => method.Invoke(monoBehaviour, null);
                            _buttonActionMapping.Add(buttonName, action);
                            Debug.Log($"�޼��� {method.Name}�� ��ư {buttonName}�� �����");
                        }
                        else
                            Debug.Log($"��ư {buttonName}�� �̹� �����.");
                    }
                }
            }

            // Button�� �ش� ������ ����
            UIButton[] uIButtonActions = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtonActions)
            {
                string buttonName = button.gameObject.name;
                if (_buttonActionMapping.ContainsKey(buttonName))
                {
                    button.SetButtonAction(_buttonActionMapping[buttonName]);
                    Debug.Log($"{buttonName} ��ư�� ������ ��ϵǾ����ϴ�.");
                }
                else
                    Debug.Log($"{buttonName}�� ���� ������ ��ϵ��� �ʾҽ��ϴ�.");
            }
        }

        /// <summary>
        /// UIScene�� �� ����
        /// </summary>
        /// <param name="sceneNumber"></param>
        private void SetUpUIScene()
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
            if (_popupUIDictionary.TryGetValue(popUpName, out UIPopUp popUp))
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
                UIPopUp popUp = _currentPopupUI.Pop();
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

            var popUpStackCopy = new Stack<UIPopUp>(_currentPopupUI);

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

        /// <summary>
        /// Text�� ���� ����
        /// </summary>
        /// <param name="fieldName"> Text �̸� </param>
        /// <param name="value"> �� </param>
        public void SetTextValue(string fieldName, string value)
        {
            if (_textDictionary.TryGetValue(fieldName, out TextMeshProUGUI text))
                text.text = value;
            else
                Debug.Log($"{fieldName} Text�� ã�� �� ����");
        }

        /// <summary>
        /// �� �ε�� �� UI ȣ��
        /// </summary>
        /// <param name="sceneNumber"> �� ��ȣ </param>
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
using SOF.Scripts.Etc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// 매핑 UI 명명 규칙 요약
/// 버튼	    {Prefix}_{Action}Button 
/// 팝업	    {Prefix}_{Action}Popup 
/// 스크립트	On{Prefix}Button 
/// 메서드	On{ButtonName}Click 
namespace SOF.Scripts.View
{
    /// <summary>
    /// UI 전체를 관리하는 매니저
    /// 1. 팝업과 버튼을 매핑, 스택을 사용하여 팝업창 관리
    /// 2. InputField 관리
    /// 3. Button 관리
    /// </summary>
    public class UIManager : SingletonLazy<UIManager>
    {
        [HideInInspector]
        public Transform canvasTransform;

        private Dictionary<string, UIPopUp> _popupUIDictionary = new();               // 팝업 이름과 UIPopUp 객체를 매핑하는 Dictionary
        private Dictionary<int, UIScene> _sceneUIDictionary = new();                  // 각 씬 번호에 따라 UIScene을 저장하는 Dictionary
        private Dictionary<string, UIButton> _buttonPopupMapping = new();             // 버튼과 팝업 이름을 매핑하는 Dictionary
        private Dictionary<string, Action> _buttonActionMapping = new();              // 버튼과 해당 동작을 매핑하는 Dictionary
        private Dictionary<string, InputField> _inputFieldDictionary = new();         // InputField를 저장할 Dictionary
        private Dictionary<string, TextMeshProUGUI> _textDictionary = new();          // Text를 저장할 Dictionary
        private Stack<UIPopUp> _currentPopupUI = new();                               // 현재 활성화된 팝업을 관리하는 스택
        private UIScene _currentSceneUI;                                              // 현재 활성화된 UIScene
        private Dictionary<string, Type> _buttonPrefixToScriptMapping = new();        // 접두사와 스크립트 타입을 매핑(버튼)
        private Dictionary<string, Type> _popupPrefixToScriptMapping = new();         // 접두사와 스크립트 타입을 매핑(팝업)

        /// <summary>
        /// 캔버스 할당, 팝업 찾기 및 등록, 버튼과 팝업 매핑, UI와 씬 연결
        /// </summary>
        private void Awake()
        {
            Debug.Log("UIManager 실행");

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
        /// ESC 키 입력 처리
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
        /// Canvas 할당
        /// </summary>
        private void SetUpCanvas()
        {
            canvasTransform = FindObjectOfType<Canvas>().transform;

            if (canvasTransform == null)
                Debug.Log("Canvas 없음");
            else
                Debug.Log("성공적으로 할당");
        }

        /// <summary>
        /// 팝업 UI 설정
        /// </summary>
        private void SetUpUIPopUp()
        {
            // "PopUps" 부모 찾기
            Transform popupsParent = canvasTransform.Find("PopUps");
            if (popupsParent == null)
            {
                Debug.Log("부모 PopUps GameObject를 찾을 수 없음");
                return;
            }

            // "PopUps" 부모 아래 모든 자식 게임 오브젝트 검색
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
                        Debug.Log($"UIPopUp 스크립트가 {popupName}에 자동으로 추가됨");
                    }

                    _popupUIDictionary.Add(popupName, popupComponent);
                    Debug.Log($"{popupName} 팝업이 Dictionary에 추가됨");
                }
                        
            }
        }

        /// <summary>
        /// InputField 설정
        /// </summary>
        private void SetUpInputField()
        {
            // InoutField를 찾아서 Dictionary에 등록 ** UIScene 연동 후 되는지 확인해보기
            InputField[] uIInputFields = canvasTransform.GetComponentsInChildren<InputField>();

            foreach (var field in uIInputFields)
            {
                Debug.Log($"UIInputField 발견 : {field.name}");
                string fieldName = field.gameObject.name;
                if (!_inputFieldDictionary.ContainsKey(fieldName))
                {
                    _inputFieldDictionary.Add(fieldName, field);
                    Debug.Log($"InputField 등록됨 : {fieldName}");
                }
            }
        }

        /// <summary>
        /// Text 설정
        /// </summary>
        private void SetUpText()
        {
            TextMeshProUGUI[] texts = canvasTransform.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var text in texts)
            {
                Debug.Log($"UIText 발견 : {text.name}");
                string fieldName = text.gameObject.name;
                if (!_textDictionary.ContainsKey(fieldName))
                {
                    _textDictionary.Add(fieldName, text);
                    Debug.Log($"Text 등록됨 : {fieldName}");
                }
            }
        }

        /// <summary>
        /// PopUp에 Custom 스크립트 추가
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
                            Debug.Log($"{scriptType.Name}가 {popupName}에 할당됨");
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
                        Debug.Log($"UIPopUp이 {popupName}에 할당됨");
                    }
                }
            }
        }

        /// <summary>
        /// Button에 Custom 스크립트 추가
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
                            Debug.Log($"{scriptType.Name}가 {buttonName}에 할당됨");
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
                        Debug.Log($"UIButton이 {buttonName}에 할당됨");
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
                        Debug.Log($"매핑 추가 : {prefix} -> {type.Name}");
                    }
                    else
                        Debug.Log($"접두사 {prefix}는 이미 매핑되어 있음");
                }
                else
                    Debug.Log($"스크립트 명명 규칙을 따르지 않았음 -> {scriptName}");
            }

            foreach (var mapping in _popupPrefixToScriptMapping)
                Debug.Log($"매핑 : {mapping.Key} -> {mapping.Value.Name}");
        }

        /// <summary>
        /// 스크립트 자동 매핑(버튼)
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
                        Debug.Log($"매핑 추가 : {prefix} -> {type.Name}");
                    }
                    else
                        Debug.Log($"접두사 {prefix}는 이미 매핑되어 있음");
                }
                else
                    Debug.Log($"스크립트 명명 규칙을 따르지 않았음 -> {scriptName}");
            }

            foreach (var mapping in _buttonPrefixToScriptMapping)
                Debug.Log($"매핑 : {mapping.Key} -> {mapping.Value.Name}");
        }

        /// <summary>
        /// 버튼과 팝업 매핑
        /// </summary>
        private void SetUpButtonPopupMapping()
        {
            // Button을 찾아서 팝업과 매핑
            UIButton[] uIButtons = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtons)
            {
                string popupName = button.gameObject.name.Replace("Button", "PopUp");
                Debug.Log($"버튼 : {button.gameObject.name} -> 매핑된 팝업 : {popupName}");
                if (_popupUIDictionary.ContainsKey(popupName))
                {
                    _buttonPopupMapping.Add(popupName, button);
                    button.SetPopupName(popupName);
                    Debug.Log($"{popupName} 팝업을 {button.gameObject.name} 버튼에 연결");
                }
                else
                    Debug.Log($"{popupName} <- 해당 팝업을 찾을 수 없음");
            }
        }

        /// <summary>
        /// 버튼과 동작 매핑
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
                            Debug.Log($"메서드 {method.Name}가 버튼 {buttonName}에 연결됨");
                        }
                        else
                            Debug.Log($"버튼 {buttonName}이 이미 연결됨.");
                    }
                }
            }

            // Button과 해당 동작을 매핑
            UIButton[] uIButtonActions = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtonActions)
            {
                string buttonName = button.gameObject.name;
                if (_buttonActionMapping.ContainsKey(buttonName))
                {
                    button.SetButtonAction(_buttonActionMapping[buttonName]);
                    Debug.Log($"{buttonName} 버튼에 동작이 등록되었습니다.");
                }
                else
                    Debug.Log($"{buttonName}에 대한 동작이 등록되지 않았습니다.");
            }
        }

        /// <summary>
        /// UIScene과 씬 연결
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
        /// 버튼 클릭시 팝업 호출
        /// </summary>
        /// <param name="popUpName"> 팝업 이름 </param>
        public void ShowPopUp(string popUpName)
        {
            if (_popupUIDictionary.TryGetValue(popUpName, out UIPopUp popUp))
            {
                if (!popUp.IsActive())
                {
                    Debug.Log($"ShowPopUp 호출 : {popUpName}");
                    popUp.ShowPanel();
                    _currentPopupUI.Push(popUp);
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
            if (_currentPopupUI.Count > 0)
            {
                UIPopUp popUp = _currentPopupUI.Pop();
                popUp.HidePanel();
            }
        }

        /// <summary>
        /// 스택에 있는 모든 팝업을 닫음
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
        /// InputField의 텍스트 값 가져오기
        /// </summary>
        /// <param name="fieldName"> InputField 이름 </param>
        /// <returns> inputfield.text </returns>
        public string GetInputFieldValue(string fieldName)
        {
            if (_inputFieldDictionary.TryGetValue(fieldName, out InputField inputfield))
            {
                return inputfield.text;
            }
            Debug.Log($"{fieldName} 필드를 찾을 수 없음");
            return string.Empty;
        }

        /// <summary>
        /// InputField의 값을 설정
        /// </summary>
        /// <param name="fieldName"> InputField 이름 </param>
        /// <param name="value"> 값 </param>
        public void SetInputFieldValue(string fieldName, string value)
        {
            if (_inputFieldDictionary.TryGetValue(fieldName, out InputField inputfield))
                inputfield.text = value;
            else
                Debug.Log($"{fieldName} 필드를 찾을 수 없음");
        }

        /// <summary>
        /// Text의 값을 설정
        /// </summary>
        /// <param name="fieldName"> Text 이름 </param>
        /// <param name="value"> 값 </param>
        public void SetTextValue(string fieldName, string value)
        {
            if (_textDictionary.TryGetValue(fieldName, out TextMeshProUGUI text))
                text.text = value;
            else
                Debug.Log($"{fieldName} Text를 찾을 수 없음");
        }

        /// <summary>
        /// 씬 로드될 때 UI 호출
        /// </summary>
        /// <param name="sceneNumber"> 씬 번호 </param>
        public void LoadSceneUI(int sceneNumber)
        {
            if (_currentSceneUI != null)
                _currentSceneUI.HideUI();

            if (_sceneUIDictionary.TryGetValue(sceneNumber, out UIScene newSceneUI))
            {
                newSceneUI.ShowUI();
                _currentSceneUI = newSceneUI;
                Debug.Log($"씬 {sceneNumber}에 맞는 UI를 로드함");
            }
            else
                Debug.Log($"씬 {sceneNumber}에 맞는 UIScene을 찾을 수 없음");
        }
    }
}
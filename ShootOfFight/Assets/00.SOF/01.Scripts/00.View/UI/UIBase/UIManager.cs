using SOF.Scripts.Etc;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private Dictionary<string, UIPopup> _popupUIDictionary = new();               // 팝업 이름과 UIPopUp 객체를 매핑하는 Dictionary
        private Dictionary<int, UIScene> _sceneUIDictionary = new();                  // 각 씬 번호에 따라 UIScene을 저장하는 Dictionary
        private Dictionary<string, UIButton> _buttonPopupMapping = new();             // 버튼과 팝업 이름을 매핑하는 Dictionary
        private Dictionary<string, Action> _buttonActionMapping = new();              // 버튼과 해당 동작을 매핑하는 Dictionary
        private Dictionary<string, InputField> _inputFieldDictionary = new();         // InputField를 저장할 Dictionary
        private Dictionary<string, TextMeshProUGUI> _textDictionary = new();          // Text를 저장할 Dictionary
        private Stack<UIPopup> _currentPopupUI = new Stack<UIPopup>();                // 현재 활성화된 팝업을 관리하는 스택
        private UIScene _currentSceneUI;                                              // 현재 활성화된 UIScene

        /// <summary>
        /// 캔버스 할당, 팝업 찾기 및 등록, 버튼과 팝업 매핑, UI와 씬 연결
        /// </summary>
        private void Awake()
        {
            Debug.Log("UIManager 실행");

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
        private void SetupCanvas()
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
        private void SetupUIPopup()
        {
            // UIPopup을 찾아서 Dictionary에 등록
            UIPopup[] uIPopUps = canvasTransform.GetComponentsInChildren<UIPopup>(true);

            foreach (var popUp in uIPopUps)
            {
                Debug.Log($"UIPopUp 발견 : {popUp.name}");
                if (!_popupUIDictionary.ContainsKey(popUp.name))
                    _popupUIDictionary.Add(popUp.name, popUp);
                else
                    Debug.LogWarning($"{popUp.name} <- 이미 존재하는 PopUp UI입니다.");
            }
        }

        /// <summary>
        /// InputField 설정
        /// </summary>
        private void SetupInputField()
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

        private void SetupText()
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
        /// 버튼과 팝업 매핑
        /// </summary>
        private void SetupButtonPopupMapping()
        {
            // Button을 찾아서 팝업과 매핑
            UIButton[] uIButtons = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtons)
            {
                string popupName = button.gameObject.name.Replace("Button", "Popup");
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
        private void SetupButtonActionMapping()
        {
            // Button과 해당 동작을 매핑
            UIButton[] uIButtonActions = canvasTransform.GetComponentsInChildren<UIButton>();

            foreach (var button in uIButtonActions)
            {
                string actionName = button.gameObject.name;
                if (_buttonActionMapping.ContainsKey(actionName))
                {
                    button.SetButtonAction(_buttonActionMapping[actionName]);
                    Debug.Log($"{actionName} 버튼에 동작이 등록되었습니다.");
                }
                else
                    Debug.Log($"{actionName}에 대한 동작이 등록되지 않았습니다.");
            }
        }

        /// <summary>
        /// UIScene과 씬 연결
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
        /// 버튼 클릭시 팝업 호출
        /// </summary>
        /// <param name="popUpName"> 팝업 이름 </param>
        public void ShowPopUp(string popUpName)
        {
            if (_popupUIDictionary.TryGetValue(popUpName, out UIPopup popUp))
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
                UIPopup popUp = _currentPopupUI.Pop();
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

            var popUpStackCopy = new Stack<UIPopup>(_currentPopupUI);

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

        public void SetTextValue(string fieldName, string value)
        {
            if (_textDictionary.TryGetValue(fieldName, out TextMeshProUGUI text))
                text.text = value;
            else
                Debug.Log($"{fieldName} Text를 찾을 수 없음");
        }

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

        public void SetButtonActions()
        {
            // 버튼 동작할 것 여기다가 넣으면 됨
        }
    }
}
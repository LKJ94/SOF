using SOF.Scripts.Etc;
using System;
using System.Collections.Generic;
using System.Reflection;
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

        private Dictionary<string, UIPopUp> _popupUIDictionary = new();               // 팝업 이름과 UIPopUp 객체를 매핑하는 Dictionary
        private Stack<UIPopUp> _currentPopupUI = new();                               // 현재 활성화된 팝업을 관리하는 스택

        private Dictionary<string, UIButton> _buttonPopupMapping = new();             // 버튼과 팝업 이름을 매핑하는 Dictionary
        private Dictionary<string, TMP_InputField> _inputFieldDictionary = new();     // InputField를 저장할 Dictionary
        private Dictionary<string, TextMeshProUGUI> _textDictionary = new();          // Text를 저장할 Dictionary

        /// <summary>
        /// UI 호출
        /// </summary>
        private void Awake()
        {
            Debug.Log("UIManager 실행");

            SetUpCanvas();
            SetUpUIPopUp();
            SetUpText();
            SetUpInputField();
            SetupButtonMapping();
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

        #region Canvas
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
        #endregion

        #region Mapping
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

                if (!obj.name.EndsWith("PopUp"))
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
        /// 팝업과 스크립트를 버튼에 매핑
        /// </summary>
        private void SetupButtonMapping()
        {
            Button[] buttons = canvasTransform.GetComponentsInChildren<Button>(true);
            Debug.Log($"{buttons.Length} buttons");
            
            foreach (var button in buttons)
            {
                Debug.Log("스크립트 매핑 활성화");
                GameObject buttonObject = button.gameObject;
                string buttonName = buttonObject.name;

                UIButton uIButton = buttonObject.GetComponent<UIButton>();
                if (uIButton == null)
                {
                    string scriptName = $"{buttonName}Script";
                    string fullScriptName = $"SOF.Scripts.View.{scriptName}";
                    Type scriptType = null;

                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        scriptType = assembly.GetType(fullScriptName);
                        if (scriptType != null)
                            break;
                    }

                    if (scriptType != null && scriptType.IsSubclassOf(typeof(UIButton)))
                    {
                        uIButton = (UIButton)buttonObject.AddComponent(scriptType);
                        Debug.Log($"{scriptName} 컴포넌트가 {buttonName}에 추가됨");
                    }
                    else
                    {
                        uIButton = buttonObject.AddComponent<UIButton>();
                        Debug.Log($"UIButton 컴포넌트가 {buttonName}에 추가됨");
                    }
                }

                string popupName = buttonName.Replace("Button", "PopUp");
                if (_popupUIDictionary.ContainsKey(popupName))
                {
                    uIButton.SetPopupName(popupName);
                    Debug.Log($"{buttonName}에 {popupName}이 매핑됨");
                }
                else
                    Debug.Log($"{popupName}을 찾을 수 없음");
            }
        }
        #endregion

        #region InputField
        /// <summary>
        /// InputField 설정
        /// </summary>
        private void SetUpInputField()
        {
            // InoutField를 찾아서 Dictionary에 등록 ** UIScene 연동 후 되는지 확인해보기
            TMP_InputField[] uIInputFields = canvasTransform.GetComponentsInChildren<TMP_InputField>(true);

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
            Debug.Log($"인풋필드 : {uIInputFields.Length}");
        }

        /// <summary>
        /// InputField의 텍스트 값 가져오기
        /// </summary>
        /// <param name="fieldName"> InputField 이름 </param>
        /// <returns> inputfield.text </returns>
        public string GetInputFieldValue(string fieldName)
        {
            if (_inputFieldDictionary.TryGetValue(fieldName, out TMP_InputField inputfield))
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
            if (_inputFieldDictionary.TryGetValue(fieldName, out TMP_InputField inputfield))
                inputfield.text = value;
            else
                Debug.Log($"{fieldName} 필드를 찾을 수 없음");
        }
        #endregion

        #region Text
        /// <summary>
        /// Text 설정
        /// </summary>
        private void SetUpText()
        {
            Transform textsParent = canvasTransform.Find("Texts");

            if (textsParent == null)
            {
                Debug.Log("Texts 그룹을 찾을 수 없음");
                return;
            }

            TextMeshProUGUI[] texts = textsParent.GetComponentsInChildren<TextMeshProUGUI>();

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
        #endregion
    }
}
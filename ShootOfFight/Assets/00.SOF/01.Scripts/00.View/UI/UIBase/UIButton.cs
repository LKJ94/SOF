using System;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    /// <summary>
    /// 버튼 UI의 구조를 정함
    /// UIManager로부터 전달된 동작 또는 팝업 관련 동작을 실행
    /// </summary>
    public class UIButton : MonoBehaviour
    {
        private Button _button;             // Unity의 버튼 컴포넌트를 참조하여 onClick 이벤트를 처리
        private Action _buttonAction;       // 버튼이 클릭될 때 실행될 동작을 저장함
        private string _popupName;          // 버튼이 클릭될 때 실행될 팝업을 저장함

        /// <summary>
        /// 버튼 컴포넌트 초기화 및 onClick 이벤트 리스너 등록
        /// </summary>
        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(OnButtonClick);
        }
        
        /// <summary>
        /// 버튼이 클릭될 때 실행할 동작을 설정
        /// </summary>
        /// <param name="action"> 버튼이 클릭될 때 실행될 동작 </param>
        public virtual void SetButtonAction(Action action)
        {
            _buttonAction = action;
        }

        /// <summary>
        /// 버튼이 클릭될 때 실행될 팝업을 설정
        /// </summary>
        /// <param name="popupName"> 버튼이 클릭될 때 실행될 팝업 </param>
        public virtual void SetPopupName(string popupName)
        {
            _popupName = popupName;
        }

        /// <summary>
        /// 팝업 관련 버튼이면 팝업이, 동작 관련 버튼이면 동작이 실행됨
        /// 둘 다 해당되면 팝업이 켜지고 동작이 실행됨
        /// </summary>
        protected virtual void OnButtonClick()
        {
            if (!string.IsNullOrEmpty(_popupName))
                UIManager.Instance.ShowPopUp(_popupName);

            _buttonAction?.Invoke();
        }
    }
}

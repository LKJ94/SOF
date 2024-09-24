using System;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    public class UIButton : MonoBehaviour
    {
        private Button _button;
        private Action _buttonAction;
        private string _popupName;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(OnButtonClick);
        }

        public virtual void SetButtonAction(Action action)
        {
            _buttonAction = action;
        }

        public virtual void SetPopupName(string popupName)
        {
            _popupName = popupName;
        }

        protected virtual void OnButtonClick()
        {
            if (_buttonAction != null)
                _buttonAction.Invoke();
            else if (!string.IsNullOrEmpty(_popupName))
                UIManager.Instance.ShowPopUp(_popupName);
            else
                Debug.Log("버튼이 할당되지 않았거나 동작하지 않습니다.");
        }
    }
}

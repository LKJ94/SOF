using System;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    /// <summary>
    /// ��ư UI�� ������ ����
    /// UIManager�κ��� ���޵� ���� �Ǵ� �˾� ���� ������ ����
    /// </summary>
    public class UIButton : MonoBehaviour
    {
        private Button _button;             // Unity�� ��ư ������Ʈ�� �����Ͽ� onClick �̺�Ʈ�� ó��
        private Action _buttonAction;       // ��ư�� Ŭ���� �� ����� ������ ������
        private string _popupName;          // ��ư�� Ŭ���� �� ����� �˾��� ������

        /// <summary>
        /// ��ư ������Ʈ �ʱ�ȭ �� onClick �̺�Ʈ ������ ���
        /// </summary>
        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
                _button.onClick.AddListener(OnButtonClick);
        }
        
        /// <summary>
        /// ��ư�� Ŭ���� �� ������ ������ ����
        /// </summary>
        /// <param name="action"> ��ư�� Ŭ���� �� ����� ���� </param>
        public virtual void SetButtonAction(Action action)
        {
            _buttonAction = action;
        }

        /// <summary>
        /// ��ư�� Ŭ���� �� ����� �˾��� ����
        /// </summary>
        /// <param name="popupName"> ��ư�� Ŭ���� �� ����� �˾� </param>
        public virtual void SetPopupName(string popupName)
        {
            _popupName = popupName;
        }

        /// <summary>
        /// �˾� ���� ��ư�̸� �˾���, ���� ���� ��ư�̸� ������ �����
        /// �� �� �ش�Ǹ� �˾��� ������ ������ �����
        /// </summary>
        protected virtual void OnButtonClick()
        {
            if (!string.IsNullOrEmpty(_popupName))
                UIManager.Instance.ShowPopUp(_popupName);

            _buttonAction?.Invoke();
        }
    }
}

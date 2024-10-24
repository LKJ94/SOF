using System;
using UnityEngine;
using UnityEngine.UI;

namespace SOF.Scripts.View
{
    /// <summary>
    /// ��ư UI�� ������ ����
    /// </summary>
    public class UIButton : MonoBehaviour
    {
        private Button _button;             // Unity�� ��ư ������Ʈ�� �����Ͽ� onClick �̺�Ʈ�� ó��
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
        /// ��ư�� Ŭ���� �� ����� �˾��� ����
        /// </summary>
        /// <param name="popupName"> ��ư�� Ŭ���� �� ����� �˾� </param>
        public virtual void SetPopupName(string popupName)
        {
            _popupName = popupName;
        }

        /// <summary>
        /// �˾� ���� ��ư�̸� �˾��� �����
        /// �� �� �ش�Ǹ� �˾��� ������ ������ �����
        /// </summary>
        protected virtual void OnButtonClick()
        {
            if (!string.IsNullOrEmpty(_popupName))
                UIManager.Instance.ShowPopUp(_popupName);
        }
    }
}

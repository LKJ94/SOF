using UnityEngine;

namespace SOF.Scripts.View
{
    public class UIPopUp : MonoBehaviour
    {
        /* �˾�â�� ���� �Ҹ�, �ִϸ��̼�, Ȱ��ȭ/��Ȱ��ȭ ó�� �� */

        private bool _isActive;

        public virtual void ShowPanel()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _isActive =true;
        }

        public virtual void HidePanel()
        {
            gameObject.SetActive(false);
            _isActive = false;
        }

        public bool IsActive()
        {
            return _isActive;
        }
    }
}
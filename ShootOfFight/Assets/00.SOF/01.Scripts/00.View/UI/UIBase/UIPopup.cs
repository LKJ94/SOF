using UnityEngine;

namespace SOF.Scripts.View
{
    /// <summary>
    /// �˾� UI â�� ������ ����
    /// </summary>
    public class UIPopup : MonoBehaviour
    {
        private bool _isActive;     // Ȱ��ȭ ����

        /// <summary>
        /// �˾� â�� ���̰� ��
        /// </summary>
        public virtual void ShowPanel()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _isActive =true;
        }

        /// <summary>
        /// �˾� â�� �Ⱥ��̰� ��
        /// </summary>
        public virtual void HidePanel()
        {
            gameObject.SetActive(false);
            _isActive = false;
        }

        /// <summary>
        /// �ܺ� Ŭ������ �ڵ忡�� �� �˾��� ���� Ȱ��ȭ �Ǿ� �ִ��� Ȯ���� �� �ְ� ����
        /// </summary>
        /// <returns> _isActive </returns>
        public bool IsActive()
        {
            return _isActive;
        }
    }
}
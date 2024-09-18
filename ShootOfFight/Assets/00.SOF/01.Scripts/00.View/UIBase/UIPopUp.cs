using UnityEngine;

namespace SOF.Scripts.View
{
    public class UIPopUp : MonoBehaviour
    {
        /* 팝업창에 대한 소리, 애니메이션, 활성화/비활성화 처리 등 */

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
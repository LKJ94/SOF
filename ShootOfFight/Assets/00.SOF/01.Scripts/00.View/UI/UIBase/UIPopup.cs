using UnityEngine;

namespace SOF.Scripts.View
{
    /// <summary>
    /// 팝업 UI 창의 구조를 정함
    /// </summary>
    public class UIPopup : MonoBehaviour
    {
        private bool _isActive;     // 활성화 여부

        /// <summary>
        /// 팝업 창을 보이게 함
        /// </summary>
        public virtual void ShowPanel()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _isActive =true;
        }

        /// <summary>
        /// 팝업 창을 안보이게 함
        /// </summary>
        public virtual void HidePanel()
        {
            gameObject.SetActive(false);
            _isActive = false;
        }

        /// <summary>
        /// 외부 클래스나 코드에서 이 팝업이 현재 활성화 되어 있는지 확인할 수 있게 해줌
        /// </summary>
        /// <returns> _isActive </returns>
        public bool IsActive()
        {
            return _isActive;
        }
    }
}
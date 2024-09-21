using UnityEngine;

namespace SOF.Scripts.View
{
    /// <summary>
    /// 씬에서 기본적으로 존재하는 UI 창의 구조를 정함
    /// </summary>
    public class UIScene : MonoBehaviour
    {
        /// <summary>
        /// UI 활성화
        /// </summary>
        public virtual void ShowUI()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UI 비활성화
        /// </summary>
        public virtual void HideUI()
        {
            gameObject.SetActive(false);
        }
    }
}

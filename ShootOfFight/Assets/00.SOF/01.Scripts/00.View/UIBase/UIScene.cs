using UnityEngine;

namespace SOF.Scripts.View
{
    public class UIScene : MonoBehaviour
    {
        /* Scene UI에 대한 소리, 애니메이션, 활성화/비활성화 처리 등 */
        public virtual void ShowUI()
        {
            gameObject.SetActive(true);
        }

        public virtual void HideUI()
        {
            gameObject.SetActive(false);
        }
    }
}

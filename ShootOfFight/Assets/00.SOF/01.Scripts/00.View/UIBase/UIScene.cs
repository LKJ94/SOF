using UnityEngine;

namespace SOF.Scripts.View
{
    /// <summary>
    /// ������ �⺻������ �����ϴ� UI â�� ������ ����
    /// </summary>
    public class UIScene : MonoBehaviour
    {
        /// <summary>
        /// UI Ȱ��ȭ
        /// </summary>
        public virtual void ShowUI()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// UI ��Ȱ��ȭ
        /// </summary>
        public virtual void HideUI()
        {
            gameObject.SetActive(false);
        }
    }
}

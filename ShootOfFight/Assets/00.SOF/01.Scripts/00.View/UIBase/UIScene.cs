using UnityEngine;

namespace SOF.Scripts.View
{
    public class UIScene : MonoBehaviour
    {
        /* Scene UI�� ���� �Ҹ�, �ִϸ��̼�, Ȱ��ȭ/��Ȱ��ȭ ó�� �� */
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

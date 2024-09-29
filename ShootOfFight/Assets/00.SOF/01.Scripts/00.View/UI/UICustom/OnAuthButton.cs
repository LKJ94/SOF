using SOF.Scripts.Presenter.Auth;
using UnityEngine;

namespace SOF.Scripts.View
{
    public class OnAuthButton : MonoBehaviour
    {
        public void OnRegisterButtonClick()
        {
            string email = UIManager.Instance.GetInputFieldValue("EmailInputField");
            string username = UIManager.Instance.GetInputFieldValue("Username");
            string password = UIManager.Instance.GetInputFieldValue("Password");
            string passwordCheck = UIManager.Instance.GetInputFieldValue("PasswordCheck");

            #region ���� ó��
            /* ���� ó�� */
            if (string.IsNullOrEmpty(email))                                                    // �̸��� ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "�̸����� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(username))                                                 // �г��� ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "�г����� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))          // ��й�ȣ ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||                // ȸ�� ���� ��ĭ
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))
            {
                UIManager.Instance.SetTextValue("ErrorText", "�� ĭ�� �ִ��� Ȯ�����ּ���.");
            }

            if (!IsValidEmail(email))                                                           // �̸��� ��ȿ�� �˻�
            {
                UIManager.Instance.SetTextValue("ErrorText", "��ȿ�� �̸��� ������ �Է����ּ���.");
                return;
            }

            if (password != passwordCheck)                                                      // ��й�ȣ�� ��й�ȣ Ȯ���� �� �� ��ġ�ϴ���
            {
                UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� ��ġ���� �ʽ��ϴ�. �ٽ� �Է����ּ���.");
                return;
            }
            #endregion

            Authentication.Instance.RegisterUser(email, password, username);
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
    }
}

using SOF.Scripts.View;
using System.Net.NetworkInformation;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace SOF.Scripts.Presenter.Auth
{
    public class Authentication : MonoBehaviour
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

            RegisterUser(email, password, username);
        }

        public async void RegisterUser(string email, string password, string username)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, password);
                Debug.Log("ȸ������ ���� ! ");

                await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
                Debug.Log("���� �г��� ���� ���� !");

                UIManager.Instance.SetTextValue("SuccessText", "ȸ�������� �Ϸ�Ǿ����ϴ�.");
            }
            catch (AuthenticationException e)
            {
                if (e.Message.Contains("email-already-in-use"))
                    UIManager.Instance.SetTextValue("ErrorText", "�̹� ��� ���� �̸����Դϴ�.");
                else
                    UIManager.Instance.SetTextValue("ErrorText", $"ȸ������ ���� : {e.Message}");
            }
            catch (RequestFailedException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"������ �߻��߽��ϴ� : {e.Message}");
            }
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
    }
}

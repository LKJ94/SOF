using SOF.Scripts.Etc;
using SOF.Scripts.View;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace SOF.Scripts.Presenter.Auth
{
    public class Authentication : SingletonLazy<Authentication>
    {
        /// <summary>
        /// ȸ������
        /// </summary>
        /// <param name="email"> �̸��� </param>
        /// <param name="password"> ��й�ȣ </param>
        /// <param name="username"> �г��� </param>
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

        public async void LoginUser(string email, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, password);
                Debug.Log($"�α��� ���� : {email}, {password}");
            }
            catch (AuthenticationException e)
            {
                if (e.Message.Contains("invalid-email"))
                    UIManager.Instance.SetTextValue("ErrorText", "��ȿ���� ���� �̸���");
                else if (e.Message.Contains("wrong-password"))
                    UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� Ʋ��");
            }
        }
    }
}

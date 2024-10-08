using SOF.Scripts.Etc;
using SOF.Scripts.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace SOF.Scripts.Presenter.Auth
{
    public class Authentication : SingletonLazy<Authentication>
    {
        /* ���� �޽��� ��ųʸ��� ���� */
        private static readonly Dictionary<int, string> ErrorMessages = new()
        {
            {CommonErrorCodes.InvalidRequest, "�Է��� ������ ��ȿ���� �ʽ��ϴ�. �ٽ� Ȯ�����ּ���."},
            {CommonErrorCodes.NotFound, "��ϵ��� ���� �̸����Դϴ�. ȸ�������� �������ּ���." },
            {CommonErrorCodes.Conflict, "�̹� ��� ���� �̸����Դϴ�. �α������ּ���." },
            {CommonErrorCodes.TooManyRequests, "��� �Ŀ� �ٽ� �α������ּ���." },
            {CommonErrorCodes.InvalidToken, "�ٽ� �α������ּ���."},
            {CommonErrorCodes.TokenExpired, "�ٽ� �α������ּ���." },
        };

        /// <summary>
        /// ȸ������
        /// </summary>
        /// <param name="email"> �̸��� </param>
        /// <param name="password"> ��й�ȣ </param>
        /// <param name="username"> �г��� </param>
        public async Task RegisterUser(string email, string password, string username)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, password);
                Debug.Log("ȸ������ ���� ! ");

                await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
                Debug.Log("���� �г��� ���� ���� !");

                UIManager.Instance.SetTextValue("SuccessText", "ȸ�������� �Ϸ�Ǿ����ϴ�.");
            }
            catch (RequestFailedException e)
            {
                if (ErrorMessages.TryGetValue(e.ErrorCode, out string errorMessage))
                    UIManager.Instance.SetTextValue("ErrorText", errorMessage);
                else
                    UIManager.Instance.SetTextValue("ErrorText", $"������ �߻��߽��ϴ� : {e.Message}");
            }
        }

        /// <summary>
        /// �α���
        /// </summary>
        /// <param name="email"> �̸��� </param>
        /// <param name="password"> ��й�ȣ </param>
        public async Task LoginUser(string email, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, password);
                Debug.Log($"�α��� ���� : {email}, {password}");
            }
            catch (RequestFailedException e)
            {
                if (ErrorMessages.TryGetValue(e.ErrorCode, out string errorMessage))
                    UIManager.Instance.SetTextValue("ErrorText", errorMessage);
                else
                    UIManager.Instance.SetTextValue("ErrorText", $"������ �߻� : {e.Message}");
            }
        }

        /// <summary>
        /// �α׾ƿ�
        /// </summary>
        /// <returns> true -> �α׾ƿ� ���� / false -> �α׾ƿ� ���� </returns>
        public void LogoutUser()
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log($"�α׾ƿ� ���� ! ");
        }
    }
}

using SOF.Scripts.Etc;
using SOF.Scripts.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

namespace SOF.Scripts.Presenter.UGS
{
    public class Authentication : SingletonLazy<Authentication>
    {
        private static readonly Dictionary<int, string> ErrorMessages = new()
        {
            {CommonErrorCodes.InvalidRequest, "�Է��� ��Ȯ���� �ʽ��ϴ�. �ٽ� Ȯ�����ּ���."},
            {CommonErrorCodes.NotFound, "�ش� ���̵� �������� �ʽ��ϴ�. ȸ�������� �������ּ���." },
            {CommonErrorCodes.Conflict, "�̹� �����ϴ� ���̵��Դϴ�." },
            {CommonErrorCodes.TooManyRequests, "��� �� �ٽ� �α������ּ���." },
            {CommonErrorCodes.InvalidToken, "�ٽ� �α������ּ���."},
            {CommonErrorCodes.TokenExpired, "������ ����Ǿ����ϴ�." },
        };

        private string GetErrorMessage(int errorCode)
        {
            if (ErrorMessages.TryGetValue(errorCode, out string meesage))
                return meesage;
            else
                return "�� �� ���� ������ �߻��߽��ϴ�. �ٽ� �õ��� �ּ���.";
        }

        public async Task RegisterUser(string id, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(id, password);
                Debug.Log("ȸ������ ���� [Auth] !");
                UIManager.Instance.SetTextValue("SuccessText", "ȸ�����Կ� �����߽��ϴ�."); // �̰� UIâ�� ���� �ٲܿ���
            }

            catch (AuthenticationException e)
            {
                string errorMessage = GetErrorMessage(e.ErrorCode);
                Debug.Log($"ȸ������ ���� ���� : {e.Message} (�ڵ� : {e.ErrorCode})");
                UIManager.Instance.SetTextValue("ErrorText", $"{errorMessage}");
            }
        }

        public async Task LoginUser(string id, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(id, password);
                Debug.Log("�α��� ����");
            }
            catch (AuthenticationException e)
            {
                string errorMessage = GetErrorMessage(e.ErrorCode);
                Debug.Log($"�α��� ���� ���� : {e.Message} (�ڵ� : {e.ErrorCode})");
                UIManager.Instance.SetTextValue("ErrorText", $"{errorMessage}");
            }
        }

        public void LogoutUser()
        {
            try
            {
                AuthenticationService.Instance.SignOut();
            }
            catch (AuthenticationException e)
            {
                Debug.Log($"�α׾ƿ� ���� : {e.Message} (�ڵ� : {e.ErrorCode})");
            }
        }
    }
}

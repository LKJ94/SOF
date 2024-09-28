using SOF.Scripts.View;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace SOF.Scripts.Presenter.Auth
{
    public class Authentication : MonoBehaviour
    {
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
    }
}

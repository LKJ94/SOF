using SOF.Scripts.Etc;
using SOF.Scripts.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using UnityEngine;

namespace SOF.Scripts.Presenter.UGS
{
    public class Authentication : SingletonLazy<Authentication>
    {
        public async Task RegisterUser(string email, string password, string username)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "email", email },
                    { "password", password },
                    { "username", username }

                };

                var result = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>("registerUser", parameters);

                if (result.success)
                {
                    Debug.Log("ȸ������ ����");
                    UIManager.Instance.SetTextValue("SuccessText", result.message);
                }
                else
                    UIManager.Instance.SetTextValue("ErrorText", result.message);
            }
            catch (CloudCodeException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"���� �߻� : {e.Message}");
            }   
        }

        public async Task LoginUser(string email, string password)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "email", email },
                    { "password", password }
                };

                var result = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>("loginUser", parameters);

                if (result.success)
                {
                    Debug.Log("�α��� ����");
                    // �α��� ���� �� �߰� �۾� ����
                }
                else
                    UIManager.Instance.SetTextValue("ErrorText", result.message);
            }
            catch (CloudCodeException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"���� �߻� : {e.Message}");
            }
        }

        public async Task LogoutUser()
        {
            try
            {
                var result = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>("logoutUser", null);

                if (result.success)
                {
                    Debug.Log("�α׾ƿ� ����");
                    // �α׾ƿ� ���� �� �߰� �۾� ����
                }
                else
                    UIManager.Instance.SetTextValue("ErrorText", result.message);
            }
            catch (CloudCodeException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"���� �߻� : {e.Message}");
            }
        }

        private class CloudCodeResult
        {
            public bool success;
            public string message;
        }
    }
}

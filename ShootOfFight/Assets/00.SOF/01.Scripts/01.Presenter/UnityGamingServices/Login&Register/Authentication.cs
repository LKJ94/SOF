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
                    Debug.Log("회원가입 성공");
                    UIManager.Instance.SetTextValue("SuccessText", result.message);
                }
                else
                    UIManager.Instance.SetTextValue("ErrorText", result.message);
            }
            catch (CloudCodeException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"오류 발생 : {e.Message}");
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
                    Debug.Log("로그인 성공");
                    // 로그인 성공 시 추가 작업 수행
                }
                else
                    UIManager.Instance.SetTextValue("ErrorText", result.message);
            }
            catch (CloudCodeException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"오류 발생 : {e.Message}");
            }
        }

        public async Task LogoutUser()
        {
            try
            {
                var result = await CloudCodeService.Instance.CallEndpointAsync<CloudCodeResult>("logoutUser", null);

                if (result.success)
                {
                    Debug.Log("로그아웃 성공");
                    // 로그아웃 성공 시 추가 작업 수행
                }
                else
                    UIManager.Instance.SetTextValue("ErrorText", result.message);
            }
            catch (CloudCodeException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"오류 발생 : {e.Message}");
            }
        }

        private class CloudCodeResult
        {
            public bool success;
            public string message;
        }
    }
}

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
        /// 회원가입
        /// </summary>
        /// <param name="email"> 이메일 </param>
        /// <param name="password"> 비밀번호 </param>
        /// <param name="username"> 닉네임 </param>
        public async void RegisterUser(string email, string password, string username)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, password);
                Debug.Log("회원가입 성공 ! ");

                await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
                Debug.Log("유저 닉네임 설정 성공 !");

                UIManager.Instance.SetTextValue("SuccessText", "회원가입이 완료되었습니다.");
            }
            catch (AuthenticationException e)
            {
                if (e.Message.Contains("email-already-in-use"))
                    UIManager.Instance.SetTextValue("ErrorText", "이미 사용 중인 이메일입니다.");
                else
                    UIManager.Instance.SetTextValue("ErrorText", $"회원가입 실패 : {e.Message}");
            }
            catch (RequestFailedException e)
            {
                UIManager.Instance.SetTextValue("ErrorText", $"오류가 발생했습니다 : {e.Message}");
            }
        }

        public async void LoginUser(string email, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, password);
                Debug.Log($"로그인 성공 : {email}, {password}");
            }
            catch (AuthenticationException e)
            {
                if (e.Message.Contains("invalid-email"))
                    UIManager.Instance.SetTextValue("ErrorText", "유효하지 않은 이메일");
                else if (e.Message.Contains("wrong-password"))
                    UIManager.Instance.SetTextValue("ErrorText", "비밀번호가 틀림");
            }
        }
    }
}

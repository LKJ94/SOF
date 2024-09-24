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

            #region 예외 처리
            /* 예외 처리 */
            if (string.IsNullOrEmpty(email))                                                    // 이메일 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "이메일을 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(username))                                                 // 닉네임 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "닉네임을 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))          // 비밀번호 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호를 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||                // 회원 정보 빈칸
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))
            {
                UIManager.Instance.SetTextValue("ErrorText", "빈 칸이 있는지 확인해주세요.");
            }

            if (!IsValidEmail(email))                                                           // 이메일 유효성 검사
            {
                UIManager.Instance.SetTextValue("ErrorText", "유효한 이메일 형식을 입력해주세요.");
                return;
            }

            if (password != passwordCheck)                                                      // 비밀번호와 비밀번호 확인이 둘 다 일치하는지
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호가 일치하지 않습니다. 다시 입력해주세요.");
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

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
    }
}

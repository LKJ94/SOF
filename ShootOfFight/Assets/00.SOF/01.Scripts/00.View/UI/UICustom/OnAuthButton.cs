using SOF.Scripts.Presenter.Auth;
using UnityEngine;

namespace SOF.Scripts.View
{
    public class OnAuthButton : UIButton
    {
        public async void OnRegisterButtonClick()
        {
            string email = UIManager.Instance.GetInputFieldValue("EmailInputField");
            string username = UIManager.Instance.GetInputFieldValue("Username");
            string password = UIManager.Instance.GetInputFieldValue("Password");
            string passwordCheck = UIManager.Instance.GetInputFieldValue("PasswordCheck");

            #region 입력값 예외 처리
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

            await Authentication.Instance.RegisterUser(email, password, username);
        }

        public async void OnLoginButtonClick()
        {
            string email = UIManager.Instance.GetInputFieldValue("EmailInputField");
            string password = UIManager.Instance.GetInputFieldValue("Password");

            #region 입력값 예외 처리
            if (string.IsNullOrEmpty(email))                                                    // 이메일 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "이메일을 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(password))                                                 // 비밀번호 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호를 입력해주세요.");
                return;
            }

            if (!IsValidEmail(email))                                                           // 이메일 유효성 검사
            {
                UIManager.Instance.SetTextValue("ErrorText", "유효한 이메일 형식을 입력해주세요.");
                return;
            }
            #endregion

            await Authentication.Instance.LoginUser(email, password);
        }

        public void OnLogoutButtonClick()
        {

        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
    }
}

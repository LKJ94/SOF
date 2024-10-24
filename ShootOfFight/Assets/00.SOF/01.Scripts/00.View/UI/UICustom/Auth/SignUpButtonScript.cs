using SOF.Scripts.Presenter.UGS;
using UnityEngine;

namespace SOF.Scripts.View
{
    public class SignUpButtonScript : UIButton
    {
        protected override void OnButtonClick()
        {
            base.OnButtonClick();

            OnSignUpButtonClick();
        }

        public async void OnSignUpButtonClick()
        {
            string user = UIManager.Instance.GetInputFieldValue("User_InputField");
            Debug.Log($"User : {user}");
            string password = UIManager.Instance.GetInputFieldValue("Password_InputField");
            Debug.Log($"PW : {password}");
            string passwordCheck = UIManager.Instance.GetInputFieldValue("PasswordCheck_InputField");

            #region 입력값 예외 처리
            if (string.IsNullOrEmpty(user))                                                    // 아이디 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "이메일을 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))          // 비밀번호 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호를 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))    // 회원 정보 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "빈 칸이 있는지 확인해주세요.");
                return;
            }

            if (password != passwordCheck)                                                      // 비밀번호와 비밀번호 확인이 둘 다 일치하는지
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호가 일치하지 않습니다. 다시 입력해주세요.");
                return;
            }
            #endregion

            await Authentication.Instance.SignUpUser(user, password);

            Debug.Log($"회원가입 완료 : ID {user}, PW {password}");
            
        }
        /* 비밀번호 조건
         * 최소 8자 이상, 최대 30자 이하
         * 최소 1개의 대문자 (A-Z)
         * 최소 1개의 소문자 (a-z)
         * 최소 1개의 숫자 (0-9)
         * 최소 1개의 특수 문자 (예: !, @, #, $, %, ^, &, *, 등) */
    }
}
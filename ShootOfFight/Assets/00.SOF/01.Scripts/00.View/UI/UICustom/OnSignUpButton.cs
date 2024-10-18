using SOF.Scripts.Presenter.UGS;

namespace SOF.Scripts.View
{
    public class OnSignUpButton : UIButton
    {
        public async void OnSignUpButtonClick()
        {
            string id = UIManager.Instance.GetInputFieldValue("IDInputField");
            string password = UIManager.Instance.GetInputFieldValue("Password");
            string passwordCheck = UIManager.Instance.GetInputFieldValue("PasswordCheck");

            #region 입력값 예외 처리
            if (string.IsNullOrEmpty(id))                                                    // 아이디 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "이메일을 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))          // 비밀번호 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호를 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))    // 회원 정보 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "빈 칸이 있는지 확인해주세요.");
            }

            if (password != passwordCheck)                                                      // 비밀번호와 비밀번호 확인이 둘 다 일치하는지
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호가 일치하지 않습니다. 다시 입력해주세요.");
                return;
            }
            #endregion

            await Authentication.Instance.SignUpUser(id, password);
        }
    }
}

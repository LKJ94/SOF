using SOF.Scripts.Presenter.UGS;

namespace SOF.Scripts.View
{
    public class OnSignInButton : UIButton
    {
        public async void OnSignInButtonClick()
        {
            string id = UIManager.Instance.GetInputFieldValue("ID_InputField");
            string password = UIManager.Instance.GetInputFieldValue("Password_InputField");

            #region 입력값 예외 처리
            if (string.IsNullOrEmpty(id))                                                    // 아이디 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "이메일을 입력해주세요.");
                return;
            }

            if (string.IsNullOrEmpty(password))                                                 // 비밀번호 빈칸
            {
                UIManager.Instance.SetTextValue("ErrorText", "비밀번호를 입력해주세요.");
                return;
            }
            #endregion

            await Authentication.Instance.SignInUser(id, password);
        }
    }
}

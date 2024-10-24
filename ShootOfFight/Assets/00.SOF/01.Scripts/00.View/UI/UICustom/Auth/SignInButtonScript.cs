using SOF.Scripts.Presenter.UGS;
using UnityEngine;

namespace SOF.Scripts.View
{
    public class SignInButtonScript : UIButton
    {
        protected override void OnButtonClick()
        {
            base.OnButtonClick();

            OnSignInButtonClick();
        }

        public async void OnSignInButtonClick()
        {
            string user = UIManager.Instance.GetInputFieldValue("ID_InputField");
            Debug.Log($"{user}");
            string password = UIManager.Instance.GetInputFieldValue("Password_InputField");
            Debug.Log($"{password}");

            #region 입력값 예외 처리
            if (string.IsNullOrEmpty(user))                                                    // 아이디 빈칸
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

            await Authentication.Instance.SignInUser(user, password);

            Debug.Log($"로그인 완료 : {user}");
        }
    }
}
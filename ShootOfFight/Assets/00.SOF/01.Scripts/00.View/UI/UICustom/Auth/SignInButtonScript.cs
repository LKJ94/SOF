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

            #region �Է°� ���� ó��
            if (string.IsNullOrEmpty(user))                                                    // ���̵� ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "�̸����� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(password))                                                 // ��й�ȣ ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� �Է����ּ���.");
                return;
            }
            #endregion

            await Authentication.Instance.SignInUser(user, password);

            Debug.Log($"�α��� �Ϸ� : {user}");
        }
    }
}
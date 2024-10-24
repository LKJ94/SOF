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

            #region �Է°� ���� ó��
            if (string.IsNullOrEmpty(user))                                                    // ���̵� ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "�̸����� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))          // ��й�ȣ ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))    // ȸ�� ���� ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "�� ĭ�� �ִ��� Ȯ�����ּ���.");
                return;
            }

            if (password != passwordCheck)                                                      // ��й�ȣ�� ��й�ȣ Ȯ���� �� �� ��ġ�ϴ���
            {
                UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� ��ġ���� �ʽ��ϴ�. �ٽ� �Է����ּ���.");
                return;
            }
            #endregion

            await Authentication.Instance.SignUpUser(user, password);

            Debug.Log($"ȸ������ �Ϸ� : ID {user}, PW {password}");
            
        }
        /* ��й�ȣ ����
         * �ּ� 8�� �̻�, �ִ� 30�� ����
         * �ּ� 1���� �빮�� (A-Z)
         * �ּ� 1���� �ҹ��� (a-z)
         * �ּ� 1���� ���� (0-9)
         * �ּ� 1���� Ư�� ���� (��: !, @, #, $, %, ^, &, *, ��) */
    }
}
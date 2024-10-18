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

            #region �Է°� ���� ó��
            if (string.IsNullOrEmpty(id))                                                    // ���̵� ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "�̸����� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))          // ��й�ȣ ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� �Է����ּ���.");
                return;
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))    // ȸ�� ���� ��ĭ
            {
                UIManager.Instance.SetTextValue("ErrorText", "�� ĭ�� �ִ��� Ȯ�����ּ���.");
            }

            if (password != passwordCheck)                                                      // ��й�ȣ�� ��й�ȣ Ȯ���� �� �� ��ġ�ϴ���
            {
                UIManager.Instance.SetTextValue("ErrorText", "��й�ȣ�� ��ġ���� �ʽ��ϴ�. �ٽ� �Է����ּ���.");
                return;
            }
            #endregion

            await Authentication.Instance.SignUpUser(id, password);
        }
    }
}

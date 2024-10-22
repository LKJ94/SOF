using SOF.Scripts.Presenter.UGS;

namespace SOF.Scripts.View
{
    public class OnSignInButton : UIButton
    {
        public async void OnSignInButtonClick()
        {
            string id = UIManager.Instance.GetInputFieldValue("ID_InputField");
            string password = UIManager.Instance.GetInputFieldValue("Password_InputField");

            #region �Է°� ���� ó��
            if (string.IsNullOrEmpty(id))                                                    // ���̵� ��ĭ
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

            await Authentication.Instance.SignInUser(id, password);
        }
    }
}

using SOF.Scripts.Presenter.UGS;

namespace SOF.Scripts.View
{
    public class OnSignOutButton : UIButton
    {
        public void OnSignOutButtonClick()
        {
            Authentication.Instance.SignOutUser();
            // �α׾ƿ� ó���ϸ鼭 �� �̵� �� ���� ����
        }
    }
}

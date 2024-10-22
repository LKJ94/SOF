using SOF.Scripts.Presenter.UGS;

namespace SOF.Scripts.View
{
    public class OnSignOutButton : UIButton
    {
        public void OnSignOutButtonClick()
        {
            Authentication.Instance.SignOutUser();
            // 로그아웃 처리하면서 씬 이동 등 구현 예정
        }
    }
}

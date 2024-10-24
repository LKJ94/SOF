using SOF.Scripts.Presenter.UGS;
using UnityEngine;

namespace SOF.Scripts.View
{
    public class SignOutButtonScript : UIButton
    {
        protected override void OnButtonClick()
        {
            base.OnButtonClick();

            OnSignOutButtonClick();
        }

        public void OnSignOutButtonClick()
        {
            Authentication.Instance.SignOutUser();

            Debug.Log($"로그아웃 완료");
            // 로그아웃 처리하면서 씬 이동 등 구현 예정
        }
    }
}
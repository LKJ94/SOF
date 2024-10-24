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

            Debug.Log($"�α׾ƿ� �Ϸ�");
            // �α׾ƿ� ó���ϸ鼭 �� �̵� �� ���� ����
        }
    }
}
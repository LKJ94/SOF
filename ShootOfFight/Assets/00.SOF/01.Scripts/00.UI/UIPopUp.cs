using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOF.Scripts.UI
{
    public class UIPopUp : UIBase
    {
        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;

            UIManager.Instance.SetCanvas(gameObject, null);
            return true;
        }

        public virtual void ClosePopUpUI()
        {
            UIManager.Instance.ClosePopUp(this);
        }
    }
}

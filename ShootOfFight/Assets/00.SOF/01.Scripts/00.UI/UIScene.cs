using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOF.Scripts.UI
{
    public class UIScene : UIBase
    {
        public override bool Initialize()
        {
            if (!base.Initialize()) 
                return false;

            UIManager.Instance.SetCanvas(gameObject);
            return true;
        }
    }
}

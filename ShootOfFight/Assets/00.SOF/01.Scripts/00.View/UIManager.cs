using SOF.Scripts.Etc;
using System.Collections.Generic;
using UnityEngine;

namespace SOF.Scripts.View
{
    public class UIManager : SingletonLazy<UIManager>
    {
        private Stack<UIPopUp> _popupStack = new Stack<UIPopUp>();

        public void ShowPopUp(UIPopUp popup)
        {
            popup.ShowPanel();
            _popupStack.Push(popup);
        }

        public void ClosePopUp()
        {
            if (_popupStack.Count > 0)
            {
                UIPopUp topPopup = _popupStack.Pop();
                topPopup.HidePanel();
            }
        }

        public void OnButtonClicked(string popUpName)
        {

        }
    }
}
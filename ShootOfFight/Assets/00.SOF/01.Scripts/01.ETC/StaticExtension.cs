using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SOF.Scripts.UI;

namespace SOF.Scripts.ETC
{
    public static class StaticExtension
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return Utilities.GetOrAddComponent<T>(obj);
        }

        public static void BindEvnet(this GameObject obj, Action action = null, Action<BaseEventData> dragAction = null, Define.UIEvent type = Define.UIEvent.Click)
        {
           UIBase.BindEventUI(obj, action, dragAction, type);
        }

        public static bool Isvalid(this GameObject obj)
        {
            return obj != null && obj.activeSelf;
        }
    }
}

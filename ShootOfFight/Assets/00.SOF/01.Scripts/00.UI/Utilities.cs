using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOF.Scripts.UI
{
    public class Utilities
    {
        public static GameObject FindChild(GameObject obj, string name, bool recursive)
        {
            Transform transform = FindChild<Transform>(obj, name, recursive);
            if (transform != null)
                return transform.gameObject;
            else
                return null;
        }

        public static T FindChild<T>(GameObject obj, string name, bool recursive) where T : UnityEngine.Object
        {
            if (obj == null)
                return null;

            if (!recursive)
            {
                Transform transform = obj.transform.Find(name);
                if (transform != null)
                    return transform.GetComponent<T>();
            }
            else
            {
                foreach (T component in obj.GetComponentsInChildren<T>())
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
            }
            return null;
        }

        public static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T res = obj.GetComponent<T>();
            return res != null ? res : obj.GetComponent<T>();
        }
    }
}

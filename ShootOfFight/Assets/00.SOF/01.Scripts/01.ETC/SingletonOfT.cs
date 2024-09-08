using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SOF.Scripts.ETC
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(T)).AddComponent<T>();
                    instance.Initialize();
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                instance.Initialize();
            }
        }

        protected virtual void Start()
        {
            if (instance != this)
                Destroy(gameObject);
        }

        protected virtual void Initialize() { }
    }
}

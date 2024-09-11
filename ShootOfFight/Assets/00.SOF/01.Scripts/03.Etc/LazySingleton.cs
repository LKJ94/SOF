using System;
using UnityEngine;

namespace SOF.Scripts.Etc
{
    /*  MonoBehaviour�� ��ӹް� ���׸��� ����Ͽ� �̱��� �ν��Ͻ� ���� 
     *  �ݵ�� T�� class�̿��� �� */
    public class SingletonLazy<T> : MonoBehaviour where T : class
    {
        /* Lazy<T>�� ����Ͽ� �̱��� �ν��Ͻ��� ����
         * Lazy�� ������ ���� �ʿ��� �� ���� ������ ������Ű�� ��ü */
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            T instance = FindObjectOfType(typeof(T)) as T;          // ���� ������ T Ÿ���� ��ü�� ã��

            if (instance == null)                                   // �ν��Ͻ��� null�̸� ���ο� GameObject�� �����ϰ� T ������Ʈ �߰��� 
            {
                GameObject obj = new GameObject("SingletonLazy");
                instance = obj.AddComponent(typeof(T)) as T;

                DontDestroyOnLoad(obj);                             // �� ��ȯ�� GameObject�� �ı����� �ʵ��� ����
            }
            else
                Destroy(instance as GameObject);                    // �̹� �ν��Ͻ��� ���� ��� ���� ������ GameObject �ı�

            return instance;                                        // ������ �ν��Ͻ� ��ȯ
        });

        public static T Instance                                    // �̱��� �ν��Ͻ��� �����ϱ� ���� ���� �Ӽ�
        {
            get
            {
                /* Lazy �ν��Ͻ��� Value �Ӽ��� ���� �̱��� �ν��Ͻ� ��ȯ
                 * �����ϴ� ���� Lazy ��ü�� ���� ���� ���� */
                return _instance.Value;
            }
        }
    }
}

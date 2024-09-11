using System;
using UnityEngine;

namespace SOF.Scripts.Etc
{
    /*  MonoBehaviour를 상속받고 제네릭을 사용하여 싱글톤 인스턴스 생성 
     *  반드시 T가 class이여야 함 */
    public class SingletonLazy<T> : MonoBehaviour where T : class
    {
        /* Lazy<T>를 사용하여 싱글톤 인스턴스를 저장
         * Lazy는 실제로 값이 필요할 때 까지 생성을 지연시키는 객체 */
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            T instance = FindObjectOfType(typeof(T)) as T;          // 현재 씬에서 T 타입의 객체를 찾음

            if (instance == null)                                   // 인스턴스가 null이면 새로운 GameObject를 생성하고 T 컴포넌트 추가함 
            {
                GameObject obj = new GameObject("SingletonLazy");
                instance = obj.AddComponent(typeof(T)) as T;

                DontDestroyOnLoad(obj);                             // 씬 전환시 GameObject가 파괴되지 않도록 설정
            }
            else
                Destroy(instance as GameObject);                    // 이미 인스턴스가 있을 경우 새로 생성된 GameObject 파괴

            return instance;                                        // 생성된 인스턴스 반환
        });

        public static T Instance                                    // 싱글톤 인스턴스에 접근하기 위한 정적 속성
        {
            get
            {
                /* Lazy 인스턴스의 Value 속성을 통해 싱글톤 인스턴스 반환
                 * 접근하는 순간 Lazy 객체가 실제 값을 생성 */
                return _instance.Value;
            }
        }
    }
}

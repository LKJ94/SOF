using SOF.Scripts.Etc;
using SOF.Scripts.View;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;


namespace SOF.Scripts.Presenter
{
    public class UnityGamingServices : SingletonLazy<UnityGamingServices>
    {
        private int _retryCount = 5;
        private float _retryDelay = 2f;

        void Awake()
        {
            InitializeUGS();
            Debug.Log($"UGS 초기화 성공 ! ");
        }

        async void InitializeUGS()
        {
            int attemts = 0;
            bool success = false;

            while (attemts < _retryCount && !success)
            {
                try
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log($"UGS가 성공적으로 초기화 됨");
                    success = true;
                }
                catch (System.Exception e)
                {
                    if (attemts < _retryCount)
                    {
                        attemts++;
                        Debug.Log($"UGS 초기화에 실패 (시도 : {attemts} / {_retryCount}) : {e.Message}");
                        await Task.Delay((int)(_retryDelay * 1000));
                    }
                    else
                    {
                        Debug.Log($"UGS 초기화 시도 실패");
                        UIManager.Instance.ShowPopUp("UGSErrorPopUp");  // << 관련 팝업창 생성
                    }
                }
            }
        }
    }
}
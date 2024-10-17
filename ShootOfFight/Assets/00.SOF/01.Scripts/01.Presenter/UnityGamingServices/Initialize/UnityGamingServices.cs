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
            Debug.Log($"UGS �ʱ�ȭ ���� ! ");
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
                    Debug.Log($"UGS�� ���������� �ʱ�ȭ ��");
                    success = true;
                }
                catch (System.Exception e)
                {
                    if (attemts < _retryCount)
                    {
                        attemts++;
                        Debug.Log($"UGS �ʱ�ȭ�� ���� (�õ� : {attemts} / {_retryCount}) : {e.Message}");
                        await Task.Delay((int)(_retryDelay * 1000));
                    }
                    else
                    {
                        Debug.Log($"UGS �ʱ�ȭ �õ� ����");
                        UIManager.Instance.ShowPopUp("UGSErrorPopUp");  // << ���� �˾�â ����
                    }
                }
            }
        }
    }
}
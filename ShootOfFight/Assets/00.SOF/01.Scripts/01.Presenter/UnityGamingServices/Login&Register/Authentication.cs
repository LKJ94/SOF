using SOF.Scripts.Etc;
using SOF.Scripts.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

namespace SOF.Scripts.Presenter.UGS
{
    public class Authentication : SingletonLazy<Authentication>
    {
        private static readonly Dictionary<int, string> ErrorMessages = new()
        {
            {CommonErrorCodes.InvalidRequest, "입력이 정확하지 않습니다. 다시 확인해주세요."},
            {CommonErrorCodes.NotFound, "해당 아이디가 존재하지 않습니다. 회원가입을 진행해주세요." },
            {CommonErrorCodes.Conflict, "이미 존재하는 아이디입니다." },
            {CommonErrorCodes.TooManyRequests, "잠시 후 다시 로그인해주세요." },
            {CommonErrorCodes.InvalidToken, "다시 로그인해주세요."},
            {CommonErrorCodes.TokenExpired, "세션이 만료되었습니다." },
        };

        private string GetErrorMessage(int errorCode)
        {
            if (ErrorMessages.TryGetValue(errorCode, out string meesage))
                return meesage;
            else
                return "알 수 없는 오류가 발생했습니다. 다시 시도해 주세요.";
        }

        public async Task RegisterUser(string id, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(id, password);
                Debug.Log("회원가입 성공 [Auth] !");
                UIManager.Instance.SetTextValue("SuccessText", "회원가입에 성공했습니다."); // 이건 UI창을 열든 바꿀예정
            }

            catch (AuthenticationException e)
            {
                string errorMessage = GetErrorMessage(e.ErrorCode);
                Debug.Log($"회원가입 인증 오류 : {e.Message} (코드 : {e.ErrorCode})");
                UIManager.Instance.SetTextValue("ErrorText", $"{errorMessage}");
            }
        }

        public async Task LoginUser(string id, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(id, password);
                Debug.Log("로그인 성공");
            }
            catch (AuthenticationException e)
            {
                string errorMessage = GetErrorMessage(e.ErrorCode);
                Debug.Log($"로그인 인증 오류 : {e.Message} (코드 : {e.ErrorCode})");
                UIManager.Instance.SetTextValue("ErrorText", $"{errorMessage}");
            }
        }

        public void LogoutUser()
        {
            try
            {
                AuthenticationService.Instance.SignOut();
            }
            catch (AuthenticationException e)
            {
                Debug.Log($"로그아웃 오류 : {e.Message} (코드 : {e.ErrorCode})");
            }
        }
    }
}

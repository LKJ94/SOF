using SOF.Scripts.Etc;
using SOF.Scripts.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace SOF.Scripts.Presenter.Auth
{
    public class Authentication : SingletonLazy<Authentication>
    {
        /* 에러 메시지 딕셔너리로 저장 */
        private static readonly Dictionary<int, string> ErrorMessages = new()
        {
            {CommonErrorCodes.InvalidRequest, "입력한 정보가 유효하지 않습니다. 다시 확인해주세요."},
            {CommonErrorCodes.NotFound, "등록되지 않은 이메일입니다. 회원가입을 진행해주세요." },
            {CommonErrorCodes.Conflict, "이미 사용 중인 이메일입니다. 로그인해주세요." },
            {CommonErrorCodes.TooManyRequests, "잠시 후에 다시 로그인해주세요." },
            {CommonErrorCodes.InvalidToken, "다시 로그인해주세요."},
            {CommonErrorCodes.TokenExpired, "다시 로그인해주세요." },
        };

        /// <summary>
        /// 회원가입
        /// </summary>
        /// <param name="email"> 이메일 </param>
        /// <param name="password"> 비밀번호 </param>
        /// <param name="username"> 닉네임 </param>
        public async Task RegisterUser(string email, string password, string username)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, password);
                Debug.Log("회원가입 성공 ! ");

                await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
                Debug.Log("유저 닉네임 설정 성공 !");

                UIManager.Instance.SetTextValue("SuccessText", "회원가입이 완료되었습니다.");
            }
            catch (RequestFailedException e)
            {
                if (ErrorMessages.TryGetValue(e.ErrorCode, out string errorMessage))
                    UIManager.Instance.SetTextValue("ErrorText", errorMessage);
                else
                    UIManager.Instance.SetTextValue("ErrorText", $"오류가 발생했습니다 : {e.Message}");
            }
        }

        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="email"> 이메일 </param>
        /// <param name="password"> 비밀번호 </param>
        public async Task LoginUser(string email, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, password);
                Debug.Log($"로그인 성공 : {email}, {password}");
            }
            catch (RequestFailedException e)
            {
                if (ErrorMessages.TryGetValue(e.ErrorCode, out string errorMessage))
                    UIManager.Instance.SetTextValue("ErrorText", errorMessage);
                else
                    UIManager.Instance.SetTextValue("ErrorText", $"오류가 발생 : {e.Message}");
            }
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        /// <returns> true -> 로그아웃 성공 / false -> 로그아웃 실패 </returns>
        public void LogoutUser()
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log($"로그아웃 성공 ! ");
        }
    }
}

using System;
using GhysX.Framework.Domains;
using Loxodon.Framework.Asynchronous;

namespace GhysX.Framework.Services
{
    public class AuthEventArgs : EventArgs
    {
        public AuthEventArgs(bool succeed, AuthorizationInfo authorizationInfo)
        {
            this.IsSucceed = succeed;
            this.AuthorizationInfo = authorizationInfo;
        }

        public bool IsSucceed { get; private set; }

        public AuthorizationInfo AuthorizationInfo { get; private set; }
    }
    
    public interface IAuthService
    {
        event EventHandler<AuthEventArgs> AuthFinished;
        
        IAsyncResult<AuthorizationInfo> Auth();
        
        IAsyncResult<AuthorizationInfo> Auth(AuthorizationInfo authorizationInfo);
    }
}
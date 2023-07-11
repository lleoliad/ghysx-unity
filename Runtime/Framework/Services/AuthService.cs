using System;
using GhysX.Framework.Domains;
using GhysX.Framework.Repositories;
using GhysX.Framework.Utilities;
using Loxodon.Framework.Asynchronous;
using UnityEngine;

namespace GhysX.Framework.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;

        public AuthService(IAuthRepository repository)
        {
            this._repository = repository;
        }

        public event EventHandler<AuthEventArgs> AuthFinished;

        public virtual IAsyncResult<AuthorizationInfo> Auth()
        {
            string privateKey = "";
            string publicKey = "";
            SecurityUtil.RSAGenerateKey(ref privateKey, ref publicKey);

            AuthorizationInfo authorizationInfo = new AuthorizationInfo();
            authorizationInfo.PrivateKey = privateKey;
            authorizationInfo.PublicKey = publicKey;
            return Auth(authorizationInfo);
        }

        public virtual IAsyncResult<AuthorizationInfo> Auth(AuthorizationInfo authorizationInfo)
        {
            AsyncResult<AuthorizationInfo> result = new AsyncResult<AuthorizationInfo>();
            DoAuth(result, authorizationInfo);
            return result;
        }

        private async void DoAuth(IPromise<AuthorizationInfo> promise, AuthorizationInfo authorizationInfo)
        {
            promise.SetResult(authorizationInfo);
            await _repository.Save(authorizationInfo);
        }

        protected virtual void RaiseAuthFinished(bool succeed, AuthorizationInfo authorizationInfo)
        {
            try
            {
                if (this.AuthFinished != null)
                    this.AuthFinished(this, new AuthEventArgs(succeed, authorizationInfo));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
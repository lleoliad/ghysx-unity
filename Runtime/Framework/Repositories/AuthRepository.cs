using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using GhysX.Framework.Domains;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace GhysX.Framework.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Dictionary<string, AuthorizationInfo> _cache = new Dictionary<string, AuthorizationInfo>();

        private AuthorizationInfo _current;

        public AuthRepository()
        {
            // AuthorizationInfo authorizationInfo = new AuthorizationInfo() { };
            // _cache.Add(authorizationInfo.PrivateKey, authorizationInfo);
        }

        public AuthorizationInfo Current()
        {
            return _current;
        }

        public virtual IAsyncResult<AuthorizationInfo> Get(string privateKey)
        {
            return Executors.RunOnCoroutine<AuthorizationInfo>(promise => DoGet(promise, privateKey));
        }
        
        protected virtual IEnumerator DoGet(IPromise<AuthorizationInfo> promise, string privateKey)
        {
            var httpRequest = new HTTPRequest(new Uri(Environment.Settings.authServerAddress), OnRequestFinishedDelegate);
            yield return httpRequest.Send();
            this._cache.TryGetValue(privateKey, out var authorizationInfo);
            yield return null;
            _current = authorizationInfo;
            promise.SetResult(authorizationInfo);
        }

        protected virtual void OnRequestFinishedDelegate(HTTPRequest originalRequest, HTTPResponse response)
        {
            
        }

        public virtual IAsyncResult<AuthorizationInfo> Save(AuthorizationInfo authorizationInfo)
        {
            return Executors.RunOnCoroutine<AuthorizationInfo>(promise => DoSave(promise, authorizationInfo));
        }
        
        protected virtual IEnumerator DoSave(IPromise<AuthorizationInfo> promise, AuthorizationInfo authorizationInfo)
        {
            if (_cache.ContainsKey(authorizationInfo.PrivateKey))
            {
                promise.SetException(new Exception("The authorization information already exists."));
                yield break;
            }

            _cache.Add(authorizationInfo.PrivateKey, authorizationInfo);
            promise.SetResult(authorizationInfo);
        }

        public virtual IAsyncResult<AuthorizationInfo> Update(AuthorizationInfo authorizationInfo)
        {
            throw new System.NotImplementedException();
        }

        public virtual IAsyncResult<bool> Delete(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
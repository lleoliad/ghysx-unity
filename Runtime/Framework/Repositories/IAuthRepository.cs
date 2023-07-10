using GhysX.Framework.Domains;
using Loxodon.Framework.Asynchronous;

namespace GhysX.Framework.Repositories
{
    public interface IAuthRepository
    {
        IAsyncResult<AuthorizationInfo> Get(string privateKey);

        IAsyncResult<AuthorizationInfo> Save(AuthorizationInfo authorizationInfo);

        IAsyncResult<AuthorizationInfo> Update(AuthorizationInfo authorizationInfo);

        IAsyncResult<bool> Delete(string privateKey);
    }
}
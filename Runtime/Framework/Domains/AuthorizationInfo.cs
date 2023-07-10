using System;
using Loxodon.Framework.Observables;

namespace GhysX.Framework.Domains
{
    public partial class AuthorizationInfo : ObservableObject
    {
        private string _privateKey;
        private string _publicKey;

        private DateTime _created;

        public string PrivateKey
        {
            get { return this._privateKey; }
            set { this.Set<string>(ref this._privateKey, value); }
        }

        public string PublicKey
        {
            get { return this._publicKey; }
            set { this.Set<string>(ref this._publicKey, value); }
        }

        public DateTime Created
        {
            get { return this._created; }
            set { this.Set<DateTime>(ref this._created, value); }
        }
    }
}
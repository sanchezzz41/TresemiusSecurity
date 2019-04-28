using System.Security.Cryptography;
using Security2.Rsa;

namespace TresemiusSecurity.Server
{
    public class RsaServerKeys : RsaKeys
    {
        /// <inheritdoc />
        public RsaServerKeys(RSAParameters publicKey, RSAParameters privateKey) : base(publicKey, privateKey)
        {
        }
    }
}

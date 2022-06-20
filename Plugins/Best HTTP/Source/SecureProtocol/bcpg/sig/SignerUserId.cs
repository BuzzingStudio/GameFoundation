#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Bcpg.Sig
{
    /**
    * packet giving the User ID of the signer.
    */
    public class SignerUserId
        : SignatureSubpacket
    {
        private static byte[] UserIdToBytes(
            string id)
        {
            var idData = new byte[id.Length];

            for (var i = 0; i != id.Length; i++) idData[i] = (byte)id[i];

            return idData;
        }

        public SignerUserId(
            bool critical,
            bool isLongLength,
            byte[] data)
            : base(SignatureSubpacketTag.SignerUserId, critical, isLongLength, data)
        {
        }

        public SignerUserId(
            bool critical,
            string userId)
            : base(SignatureSubpacketTag.SignerUserId, critical, false, UserIdToBytes(userId))
        {
        }

        public string GetId()
        {
            var chars = new char[this.data.Length];

            for (var i = 0; i != chars.Length; i++) chars[i] = (char)(this.data[i] & 0xff);

            return new string(chars);
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1
{
    using System.IO;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.IO;

    public class DerOutputStream
        : FilterStream
    {
        public DerOutputStream(Stream os)
            : base(os)
        {
        }

        public virtual void WriteObject(Asn1Encodable encodable) { Asn1OutputStream.Create(this.s, Asn1Encodable.Der).WriteObject(encodable); }

        public virtual void WriteObject(Asn1Object primitive) { Asn1OutputStream.Create(this.s, Asn1Encodable.Der).WriteObject(primitive); }
    }

    internal class DerOutputStreamNew
        : Asn1OutputStream
    {
        internal DerOutputStreamNew(Stream os)
            : base(os)
        {
        }

        internal override bool IsBer => false;

        internal override void WritePrimitive(Asn1Object primitive, bool withID)
        {
            var asn1Set = primitive as Asn1Set;
            if (null != asn1Set)
                /*
                     * NOTE: Even a DerSet isn't necessarily already in sorted order (particularly from DerSetParser),
                     * so all sets have to be converted here.
                     */
                primitive = new DerSet(asn1Set.elements);

            primitive.Encode(this, withID);
        }
    }
}
#pragma warning restore
#endif

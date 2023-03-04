using System.IO;

namespace Aequus.Common.Net.Attributes
{
    public class NetBoolAttribute : NetTypeAttribute
    {
        public override void Send(BinaryWriter writer, object obj)
        {
            writer.Write((bool)obj);
        }
        public override object Read(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }
    }
}
using System.IO;

namespace Aequus.Common
{
    public interface INetBinary
    {
        void Send(BinaryWriter writer);
        void Receive(BinaryReader reader);
    }
}
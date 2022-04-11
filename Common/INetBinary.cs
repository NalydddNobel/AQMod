using System.IO;

namespace Aequus.Common
{
    public interface INetBinary
    {
        void Send(BinaryWriter writer);
        void Recieve(BinaryReader reader);
    }
}
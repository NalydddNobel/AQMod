using System.IO;

namespace Aequus.Common.Networking
{
    public interface INetworker
    {
        void Send(BinaryWriter writer);
        void Receive(BinaryReader reader);
    }
}
using System.IO;

namespace Aequus.Common.Networking
{
    public interface IBinaryNetworker
    {
        void Send(BinaryWriter writer);
        void Receive(BinaryReader reader);
    }
}
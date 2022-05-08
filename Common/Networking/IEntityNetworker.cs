using System.IO;

namespace Aequus.Common.Networking
{
    public interface IEntityNetworker
    {
        void Send(int whoAmI, BinaryWriter writer);
        void Receive(int whoAmI, BinaryReader reader);
    }
}
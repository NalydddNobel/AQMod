using System.IO;

namespace Aequus.Common.Tiles.Components;
public interface INetTileInteraction {
    public void Send(System.Int32 i, System.Int32 j, BinaryWriter binaryWriter) {
    }

    public void Receive(System.Int32 i, System.Int32 j, BinaryReader binaryReader, System.Int32 sender);
}
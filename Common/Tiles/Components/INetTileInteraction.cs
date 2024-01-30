using System.IO;

namespace Aequus.Common.Tiles.Components;
public interface INetTileInteraction {
    public void Send(int i, int j, BinaryWriter binaryWriter) {
    }

    public void Receive(int i, int j, BinaryReader binaryReader, int sender);
}
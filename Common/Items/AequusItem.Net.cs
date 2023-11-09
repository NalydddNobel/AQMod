using System.IO;
using Terraria;

namespace Aequus;

public partial class AequusItem {
    public override void NetSend(Item item, BinaryWriter writer) {
        var bb = new BitsByte(itemGravityCheck > 0);
        writer.Write(bb);
        if (bb[0]) {
            writer.Write(itemGravityCheck);
            writer.Write(itemGravityMultiplier);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader) {
        itemGravityCheck = 0;

        var bb = (BitsByte)reader.ReadByte();
        if (bb[0]) {
            itemGravityCheck = reader.ReadByte();
            itemGravityMultiplier = reader.ReadSingle();
        }
    }
}
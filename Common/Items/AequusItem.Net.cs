using System.IO;
using Terraria;

namespace Aequus;

public partial class AequusItem {
    public override void NetSend(Item item, BinaryWriter writer) {
        var bb = new BitsByte(itemGravityCheck > 0, HasNameTag);
        writer.Write(bb);
        if (bb[0]) {
            writer.Write(itemGravityCheck);
            writer.Write(itemGravityMultiplier);
        }
        if (bb[1]) {
            writer.Write(NameTag);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader) {
        itemGravityCheck = 0;
        NameTag = null;

        var bb = (BitsByte)reader.ReadByte();
        if (bb[0]) {
            itemGravityCheck = reader.ReadByte();
            itemGravityMultiplier = reader.ReadSingle();
        }
        if (bb[1]) {
            NameTag = reader.ReadString();
        }
    }
}
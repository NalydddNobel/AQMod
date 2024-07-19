using System.IO;

namespace Aequus.Items;
public partial class AequusItem {
    public override void NetSend(Item item, BinaryWriter writer) {
        var bb = new BitsByte(naturallyDropped, reversedGravity, itemGravityCheck > 0, luckyDrop > 0);
        writer.Write(bb);
        if (bb[2]) {
            writer.Write(itemGravityCheck);
            writer.Write(itemGravityMultiplier);
        }
        if (bb[3])
            writer.Write(luckyDrop);
    }

    public override void NetReceive(Item item, BinaryReader reader) {
        itemGravityCheck = 0;

        var bb = (BitsByte)reader.ReadByte();
        naturallyDropped = bb[0];
        reversedGravity = bb[1];
        if (bb[2]) {
            itemGravityCheck = reader.ReadByte();
            itemGravityMultiplier = reader.ReadSingle();
        }
        if (bb[3]) {
            luckyDrop = reader.ReadUInt16();
        }
    }
}
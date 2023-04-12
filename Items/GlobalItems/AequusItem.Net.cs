using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public override void NetSend(Item item, BinaryWriter writer)
        {
            var bb = new BitsByte(naturallyDropped, reversedGravity, itemGravityCheck > 0, luckyDrop > 0, HasNameTag, RenameCount > 0);
            writer.Write(bb);
            if (bb[2]) {
                writer.Write(itemGravityCheck);
                writer.Write(itemGravityMultiplier);
            }
            if (bb[3])
                writer.Write(luckyDrop);
            if (bb[4])
                writer.Write(NameTag);
            if (bb[5])
                writer.Write((ushort)RenameCount);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            itemGravityCheck = 0;
            NameTag = null;
            RenameCount = 0;

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
            if (bb[4])
                NameTag = reader.ReadString();
            if (bb[5])
                RenameCount = reader.ReadUInt16();
        }
    }
}
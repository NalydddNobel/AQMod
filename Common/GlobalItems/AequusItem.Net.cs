using Aequus.Common.Utilities;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public override void NetSend(Item item, BinaryWriter writer)
        {
            var bb = new BitsByte(naturallyDropped, reversedGravity, noGravityTime > 0, luckyDrop, HasNameTag, RenameCount > 0);
            writer.Write(bb);
            if (bb[2])
                writer.Write(noGravityTime);
            if (bb[4])
                writer.Write(NameTag);
            if (bb[5])
                writer.Write((ushort)RenameCount);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            noGravityTime = 0;
            NameTag = null;
            RenameCount = 0;

            var bb = (BitsByte)reader.ReadByte();
            naturallyDropped = bb[0];
            reversedGravity = bb[1];
            if (bb[2])
                noGravityTime = reader.ReadByte();
            luckyDrop = bb[3];
            if (bb[4])
                NameTag = reader.ReadString();
            if (bb[5])
                RenameCount = reader.ReadUInt16();
        }
    }
}
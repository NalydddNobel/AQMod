using Aequus.NPCs.Critters;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.Event.BloodMoon;
using Aequus.NPCs.Monsters.Event.Glimmer.UltraStarite;
using Aequus.NPCs.Town.SkyMerchantNPC;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.CrossMod.SplitSupport.Photography.Prints {
    public class PrintsTile : ModTile {
        public const int BreadOfCthulhu = 0;
        public const int BloodMimic = 1;
        public const int UltraStarite = 2;
        public const int Heckto = 3;
        public const int Oblivision = 4;
        public const int SkyMerchant = 5;

        public record struct PrintInfo(int npcID, int posterItemID);

        public PrintInfo[] printInfo;

        public override void SetStaticDefaults() {
            printInfo = new PrintInfo[]
            {
                new(ModContent.NPCType<BreadOfCthulhu>(), ModContent.ItemType<PosterBreadOfCthulhu>()),
                new(ModContent.NPCType<BloodMimic>(), ModContent.ItemType<PosterBloodMimic>()),
                new(ModContent.NPCType<UltraStarite>(), ModContent.ItemType<PosterUltraStarite>()),
                new(ModContent.NPCType<Heckto>(), ModContent.ItemType<PosterHeckto>()),
                new(ModContent.NPCType<Oblivision>(), ModContent.ItemType<PosterOblivision>()),
                new(ModContent.NPCType<SkyMerchant>(), ModContent.ItemType<PosterSkyMerchant>()),
            };

            Main.tileFrameImportant[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Origin = new Point16(3, 3);
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16, 16, 16, 16, 16, 16
            };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 6;
            TileObjectData.addTile(Type);
            DustType = -1;
            AddMapEntry(Helper.ColorFurniture);
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = 0;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            int id = frameX / 108;
            var print = printInfo[id];
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 96, 96, print.posterItemID);
        }
    }
}
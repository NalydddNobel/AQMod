using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public class Trophies : ModTile
    {
        public const int OmegaStarite = 0;
        public const int Crabson = 1;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            dustType = 7;
            disableSmartCursor = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Trophy");
            AddMapEntry(new Color(120, 85, 60), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            switch (frameX / 54)
            {
                case 0:
                Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.BossItems.Starite.OmegaStariteTrophy>());
                break;

                case 1:
                Item.NewItem(i * 16, j * 16, 48, 48, ModContent.ItemType<Items.BossItems.Crabson.CrabsonTrophy>());
                break;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Furniture.Jeweled {
    public class JeweledChalice : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<JeweledChaliceTile>());
            Item.width = 16;
            Item.height = 24;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = Item.CommonMaxStack;
        }
    }

    public class JeweledChaliceTile : ModTile {
        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileShine[Type] = 5000;
            Main.tileShine2[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            AddMapEntry(Color.Gold * 1.25f, TextHelper.GetText("ItemName.JeweledChalice"));
            HitSound = SoundID.Dig;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) {
            num = 0;
        }
    }
}
using Aequus.Common.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Radon;

[LegacyName("RadonMossBrickCraftedTile")]
public class RadonMossBrick : ModTile {
    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this).WithRecipe(item => {
            item.CreateRecipe(10)
                .AddIngredient(RadonMoss.Item)
                .AddIngredient(ItemID.ClayBlock, 10)
                .AddTile(TileID.Furnaces)
                .Register();
        }));
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(Color.SaddleBrown);
        HitSound = SoundID.Tink;
        MineResist = 1f;
        DustType = DustID.Ambient_DarkBrown;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}
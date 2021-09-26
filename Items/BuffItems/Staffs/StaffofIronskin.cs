using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems.Staffs
{
    public class StaffofIronskin : BuffStaff
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.StaffofIronskin, new Color(128, 128, 128, 0)), item.type);
        }

        protected override int BuffType => BuffID.Ironskin;

        protected override int DustType => 64;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.IronskinPotion, 4);
            r.AddIngredient(ItemID.Amethyst, 8);
            r.AddIngredient(ItemID.Wood, 4);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
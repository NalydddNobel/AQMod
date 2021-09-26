using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems.Staffs
{
    public class StaffofRegeneration : BuffStaff
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.StaffofRegeneration, new Color(128, 128, 128, 0)), item.type);
        }

        protected override int BuffType => BuffID.Regeneration;

        protected override int DustType => 60;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.RegenerationPotion, 4);
            r.AddIngredient(ItemID.Amethyst, 8);
            r.AddIngredient(ItemID.Wood, 4);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
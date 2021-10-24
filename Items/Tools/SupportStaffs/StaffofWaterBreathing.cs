using AQMod.Assets.Textures;
using AQMod.Common.ItemOverlays;
using AQMod.Items.Weapons.Magic.Support;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.SupportStaffs
{
    public class StaffofWaterBreathing : LegacyBuffStaff
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmask(GlowID.StaffofWaterBreathing, new Color(128, 128, 128, 0)), item.type);
        }

        protected override int BuffType => BuffID.Gills;

        protected override int DustType => 59;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.GillsPotion, 4);
            r.AddIngredient(ItemID.Amethyst, 8);
            r.AddIngredient(ItemID.Wood, 4);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
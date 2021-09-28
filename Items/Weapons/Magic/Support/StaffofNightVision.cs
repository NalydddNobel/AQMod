using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Items.BuffItems.Staffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic.Support
{
    public class StaffofNightVision : BuffStaff
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.StaffofNightVision, new Color(128, 128, 128, 0)), item.type);
        }

        protected override int BuffType => BuffID.NightOwl;

        protected override int DustType => 61;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.NightOwlPotion, 4);
            r.AddIngredient(ItemID.Amethyst, 8);
            r.AddIngredient(ItemID.Wood, 4);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
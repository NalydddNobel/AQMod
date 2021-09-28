using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Items.Weapons.Magic.Support;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems.Staffs
{
    public class BatonofNightVision : BuffBaton
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.BatonofNightVision, new Color(128, 128, 128, 0)), item.type);
        }

        protected override int BuffType => BuffID.NightOwl;

        protected override int DustType => 61;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<StaffofNightVision>());
            r.AddIngredient(ItemID.HallowedBar, 4);
            r.AddIngredient(ItemID.SoulofSight);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
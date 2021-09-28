using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Items.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems.Staffs
{
    public class BatonofRage : BuffBaton
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.BatonofRage, new Color(128, 128, 128, 0)), item.type);
        }

        protected override int BuffType => BuffID.Rage;

        protected override int DustType => 60;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.RagePotion, 8);
            r.AddIngredient(ItemID.CrimtaneBar, 20);
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
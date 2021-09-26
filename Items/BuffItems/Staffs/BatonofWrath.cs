using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.BuffItems.Staffs
{
    public class BatonofWrath : BuffBaton
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.BatonofWrath, new Color(128, 128, 128, 0)), item.type);
        }

        protected override int BuffType => BuffID.Wrath;

        protected override int DustType => 62;

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.WrathPotion, 8);
            r.AddIngredient(ItemID.DemoniteBar, 20);
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
using AQMod.Assets.Textures;
using AQMod.Common.ItemOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic.Support
{
    public class BatonofWrath : LegacyBuffBaton
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmask(GlowID.BatonofWrath, new Color(128, 128, 128, 0)), item.type);
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
using Aequus.Common.Players;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public sealed class Mendshroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.CrabCreviceValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var stat = player.GetModPlayer<MendshroomPlayer>();
            stat.Add(circumference: 240f, regen: 60);
            if (stat.EffectActive)
            {
                var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                var d = Dust.NewDustPerfect(player.Center + v * Main.rand.NextFloat(stat.circumference / 2f), ModContent.DustType<MonoDust>(), -v, 0, new Color(10, 100, 20, 25));
            }
        }
    }
}
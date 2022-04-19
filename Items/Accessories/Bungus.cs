using Aequus.Common.Players.StatData;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public sealed class Bungus : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarities.CrabCrevice;
            Item.value = ItemPrices.CrabCreviceValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var stat = player.GetModPlayer<AequusPlayer>().GetStat<BungusStat>();
            stat.Add(new BungusStat(240, 60));
            if (stat.EffectActive)
            {
                var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                var d = Dust.NewDustPerfect(player.Center + v * Main.rand.NextFloat(stat.circumference / 2f), ModContent.DustType<MonoDust>(), -v, 0, new Color(10, 100, 20, 25));
            }
        }
    }
}
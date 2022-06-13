using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Summon.Candles
{
    public class CrimsonCandle : SoulCandle
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            DefaultToCandle(24, 6, NPCID.BloodCrawler);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, Color.MediumVioletRed.ToVector3() * Main.rand.NextFloat(0.5f, 0.8f));
        }
    }
}
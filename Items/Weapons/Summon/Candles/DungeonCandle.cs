using Aequus.Projectiles.Summon.CandleSpawners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Weapons.Summon.Candles
{
    public class DungeonCandle : SoulCandleBase
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            DefaultToCandle<WretchednessProj>(40);
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.DungeonValue;
            Item.flame = true;
            Item.UseSound = SoundID.Item83;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X += -4f * player.direction;
            player.itemLocation.Y += 8f;

            Lighting.AddLight(player.itemLocation, Color.Blue.ToVector3() * Main.rand.NextFloat(0.5f, 0.8f));
        }
    }
}
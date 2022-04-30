using Aequus.Items.Accessories.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Common.Players
{
    public class PlayerFishing : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (Player.ZoneBeach && attempt.uncommon && Main.rand.NextBool(3))
            {
                itemDrop = ModContent.ItemType<SentrySquid>();
            }
        }
    }
}
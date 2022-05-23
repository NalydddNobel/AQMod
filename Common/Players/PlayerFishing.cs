using Aequus.Items.Accessories.Summon;
using Aequus.Items.Consumables.Bait;
using Aequus.Items.Consumables.Foods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Common.Players
{
    public class PlayerFishing : ModPlayer
    {
        public Item baitUsed;

        public override void Load()
        {
            On.Terraria.Projectile.FishingCheck_RollItemDrop += Projectile_FishingCheck_RollItemDrop;
        }

        private void Projectile_FishingCheck_RollItemDrop(On.Terraria.Projectile.orig_FishingCheck_RollItemDrop orig, Projectile self, ref FishingAttempt fisher)
        {
            if (fisher.playerFishingConditions.Bait?.ModItem is IModifyFishAttempt modBait)
            {
                if (!modBait.OnItemRoll(self, ref fisher))
                {
                    return;
                }
            }
            orig(self, ref fisher);
        }

        public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel)
        {
            if (bait.ModItem is IModifyFishingPower modBait)
            {
                modBait.ModifyFishingPower(Player, this, fishingRod, ref fishingLevel);
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            baitUsed = attempt.playerFishingConditions.Bait;
            if (baitUsed?.ModItem is IModifyCatchFish modBait && modBait.ModifyCatchFish(attempt, ref itemDrop, ref npcSpawn, ref sonar, ref sonarPosition))
            {
                return;
            }

            if (npcSpawn > 0)
            {
                goto PostProbeFish;
            }

            if (Player.ZoneBeach && attempt.uncommon && Main.rand.NextBool(3))
            {
                itemDrop = ModContent.ItemType<SentrySquid>();
            }
            if (Main.bloodMoon && attempt.heightLevel < 2)
            {
                if (attempt.uncommon && Main.rand.NextBool())
                {
                    itemDrop = ModContent.ItemType<PalePufferfish>();
                }
                else if (attempt.rare && Main.rand.NextBool())
                {
                    itemDrop = ModContent.ItemType<VampireSquid>();
                }
            }

        PostProbeFish:
            if (Main.myPlayer == Player.whoAmI && baitUsed.type == ModContent.ItemType<Omnibait>())
            {
                Player.UpdateBiomes(); // Kind of cheeky
            }
        }

        public override void ModifyCaughtFish(Item fish)
        {
            if (baitUsed?.ModItem is IModifyFishItem modBait)
            {
                modBait.ModifyFishItem(fish);
            }
        }
    }
}
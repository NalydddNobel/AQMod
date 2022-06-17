﻿using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Items.Consumables.Bait;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc;
using Aequus.NPCs.Monsters;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public class PlayerFishing : ModPlayer
    {
        public Item baitUsed;

        public const int HeightLevel_Sky = 0;
        public const int HeightLevel_Surface = 1;
        public const int HeightLevel_Underground = 2;
        public const int HeightLevel_Caverns = 3;
        public const int HeightLevel_Underworld = 4;

        public override void Load()
        {
            On.Terraria.Projectile.FishingCheck_RollItemDrop += Projectile_FishingCheck_RollItemDrop;

            MonoModHooks.RequestNativeAccess();

            new Hook(
                typeof(PlayerLoader).GetMethod(nameof(PlayerLoader.CatchFish), BindingFlags.Public | BindingFlags.Static),
                typeof(PlayerFishing).GetMethod(nameof(PostCatchFish), BindingFlags.NonPublic | BindingFlags.Static)
                ).Apply();
        }

        #region Hooks
        private delegate void PlayerLoader_CatchFish(Player player, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition);

        private static void PostCatchFish(PlayerLoader_CatchFish orig, Player player, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            orig(player, attempt, ref itemDrop, ref npcSpawn, ref sonar, ref sonarPosition);

            if (npcSpawn > 0)
            {
                return;
            }

            if (itemDrop > 0 && ContentSamples.ItemsByType[itemDrop].rare == -1) // trash drop tables
            {
                int breadMonsterChance = 30;
                if (!Main.dayTime)
                {
                    breadMonsterChance /= 2; // 1/15

                    if (Main.bloodMoon)
                    {
                        breadMonsterChance /= 4; // 1/3
                    }
                }
                if (Main.rand.NextBool(breadMonsterChance))
                {
                    itemDrop = 0;
                    npcSpawn = ModContent.NPCType<BreadOfCthulhu>();
                }
                else if (Main.rand.NextBool())
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            {
                                itemDrop = ModContent.ItemType<PlasticBottle>();
                            }
                            break;

                        case 1:
                            {
                                itemDrop = ModContent.ItemType<Driftwood>();
                            }
                            break;
                    }
                }
            }
        }

        private static void Projectile_FishingCheck_RollItemDrop(On.Terraria.Projectile.orig_FishingCheck_RollItemDrop orig, Projectile self, ref FishingAttempt fisher)
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
        #endregion

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

            var aequus = Player.Aequus();
            if (attempt.inLava)
            {

                if (attempt.fishingLevel <= 0.75f && Main.rand.NextBool(4))
                {
                    itemDrop = ModContent.ItemType<TatteredDemonHorn>();
                }
                else if (aequus.nearGoreNest && Main.rand.NextBool(16))
                {
                    itemDrop = ModContent.ItemType<GoreFish>();
                }
                goto PostProbeFish;
            }

            if (attempt.inHoney)
            {
                goto PostProbeFish;
            }

            if (Player.ZoneBeach && attempt.veryrare && Main.rand.NextBool(3))
            {
                itemDrop = ModContent.ItemType<SentrySquid>();
            }
            if (Player.ZoneSnow && attempt.heightLevel > HeightLevel_Surface && (itemDrop == ItemID.Bass || itemDrop == ItemID.SpecularFish || (attempt.common && Main.rand.NextBool(3))))
            {
                itemDrop = ModContent.ItemType<IcebergFish>();
            }
            if (Main.bloodMoon && attempt.heightLevel < HeightLevel_Underground)
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
                Player.UpdateBiomes(); // Kind of cheaty
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
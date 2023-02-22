using Aequus.Items;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Items.Consumables;
using Aequus.Items.Consumables.Bait;
using Aequus.Items.Consumables.LootBags;
using Aequus.Items.Misc;
using Aequus.Items.Misc.Fish;
using Aequus.Items.Misc.Fish.Legendary;
using Aequus.Items.Misc.Fish.Quest;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools.FishingRods;
using Aequus.NPCs.Monsters;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus
{
    public partial class AequusPlayer : ModPlayer
    {
        public Item baitUsed;

        public const int HeightLevel_Sky = 0;
        public const int HeightLevel_Surface = 1;
        public const int HeightLevel_Underground = 2;
        public const int HeightLevel_Caverns = 3;
        public const int HeightLevel_Underworld = 4;

        public static List<int> TrashItemIDs { get; private set; }
        public static object Hook_PlayerLoader_CatchFish { get; private set; }

        public void Load_FishingEffects()
        {
            On.Terraria.Projectile.FishingCheck_RollItemDrop += Projectile_FishingCheck_RollItemDrop;

            TrashItemIDs = new List<int>()
            {
                ItemID.FishingSeaweed,
                ItemID.TinCan,
                ItemID.OldShoe,
            };

            Hook_PlayerLoader_CatchFish = Aequus.Hook(typeof(PlayerLoader).GetMethod(nameof(PlayerLoader.CatchFish), BindingFlags.Public | BindingFlags.Static),
                typeof(AequusPlayer).GetMethod(nameof(PostCatchFish), BindingFlags.NonPublic | BindingFlags.Static));
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

            if (itemDrop > 0) // trash drop tables
            {
                var item = ContentSamples.ItemsByType[itemDrop];
                if (npcSpawn <= 0 && item.rare != -1)
                {
                    if (player.Aequus().accDevilsTongue && ((item.rare < ItemRarityID.Blue && item.value < Item.buyPrice(gold: 1)) || (!attempt.legendary && !attempt.veryrare && !attempt.rare)))
                    {
                        itemDrop = Main.rand.Next(TrashItemIDs);
                        item = ContentSamples.ItemsByType[itemDrop];
                    }
                }

                if (item.rare == -1 || player.Aequus().ZoneCrabCrevice)
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
                    else if (Main.rand.NextBool(7))
                    {
                        itemDrop = ModContent.ItemType<BreadRoachPainting>();
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

        public void Unload_FishingEffects()
        {
            Hook_PlayerLoader_CatchFish = null;
        }

        public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel)
        {
            if (fishingRod.ModItem is ItemHooks.IModifyFishingPower modFishingRod)
            {
                modFishingRod.ModifyFishingPower(Player, this, fishingRod, ref fishingLevel);
            }
            if (bait.ModItem is ItemHooks.IModifyFishingPower modBait)
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

            var aequus = Player.Aequus();
            if (npcSpawn > 0)
            {
                goto PostProbeFish;
            }

            if (!Main.anglerQuestFinished && Main.rand.NextBool(10))
            {
                if (CanCatchQuestFish(ModContent.ItemType<BrickFish>(), attempt) && BrickFish.CheckVillagerBuildings(attempt, Player))
                {
                    itemDrop = ModContent.ItemType<BrickFish>();
                }
            }

            if (attempt.inLava)
            {
                if (attempt.fishingLevel <= 0.75f && Main.rand.NextBool(4))
                {
                    itemDrop = ModContent.ItemType<TatteredDemonHorn>();
                }
                else if (aequus.ZoneGoreNest && (attempt.rare || attempt.veryrare) && Main.rand.NextBool(3))
                {
                    itemDrop = ModContent.ItemType<GoreFish>();
                }
                goto PostProbeFish;
            }

            if (attempt.inHoney)
            {
                goto PostProbeFish;
            }

            if ((IsBasicFish(itemDrop) || (attempt.common && Main.rand.NextBool(5))) && Main.rand.NextBool())
            {
                var chooseableFish = new List<int>();
                if (attempt.heightLevel < HeightLevel_Underworld)
                {
                    if (Player.ZoneCrimson)
                        chooseableFish.Add(ModContent.ItemType<Leecheel>());
                    if (Player.ZoneCorrupt)
                        chooseableFish.Add(ModContent.ItemType<Depthscale>());
                }
                if (attempt.heightLevel > HeightLevel_Surface && Player.ZoneSnow)
                {
                    chooseableFish.Add(ModContent.ItemType<IcebergFish>());
                }
                if (chooseableFish.Count != 0)
                {
                    itemDrop = Main.rand.Next(chooseableFish);
                }
            }

            if (Player.ZoneBeach && attempt.veryrare && Main.rand.NextBool())
            {
                itemDrop = ModContent.ItemType<SentrySquid>();
            }

            if (attempt.heightLevel >= HeightLevel_Underground && Main.rand.NextBool())
            {
                if (attempt.veryrare)
                {
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            itemDrop = ModContent.ItemType<ArgonFish>();
                            break;
                        case 1:
                            itemDrop = ModContent.ItemType<KryptonFish>();
                            break;
                        case 2:
                            itemDrop = ModContent.ItemType<XenonFish>();
                            break;
                        case 3:
                            itemDrop = ModContent.ItemType<RadonFish>();
                            break;
                    }
                }
            }
            else if (Main.bloodMoon)
            {
                if ((attempt.uncommon || (attempt.common && Player.GetModPlayer<AequusPlayer>().IsVampire)) && Main.rand.NextBool())
                {
                    itemDrop = ModContent.ItemType<PalePufferfish>();
                }
                else if (attempt.rare && Main.rand.NextBool(4))
                {
                    itemDrop = ModContent.ItemType<VampireSquid>();
                }
            }

            if (Player.Aequus().ZoneCrabCrevice)
            {
                if (attempt.crate && Main.rand.NextBool())
                {
                    if (Main.hardMode)
                    {
                        itemDrop = ModContent.ItemType<CrabCreviceCrateHard>();
                    }
                    else
                    {
                        itemDrop = ModContent.ItemType<CrabCreviceCrate>();
                    }
                }
                else
                {
                    if (attempt.veryrare && Main.rand.NextBool(4))
                    {
                        itemDrop = ModContent.ItemType<CrabDaughter>();
                    }
                }
            }

            if (baitUsed?.ModItem is CrateBait crateBait)
            {
                CrateBait.ConvertCrate(Player, attempt, ref itemDrop);
            }

        PostProbeFish:
            if (Main.myPlayer == Player.whoAmI && baitUsed?.type == ModContent.ItemType<Omnibait>())
            {
                aequus.omnibait = false;
                Player.UpdateBiomes(); // Kind of cheaty
            }
        }

        private bool CanCatchQuestFish(int questFish, FishingAttempt attempt)
        {
            return attempt.questFish == questFish && !Player.HasItem(questFish);
        }

        public static bool IsBasicFish(int itemDrop)
        {
            return itemDrop == ItemID.Bass || itemDrop == ItemID.SpecularFish;
        }

        public override void ModifyCaughtFish(Item fish)
        {
            if (baitUsed?.ModItem is IModifyFishItem modBait)
            {
                modBait.ModifyFishItem(Player, fish);
            }
            if (Player.HeldItem.ModItem is CrabRod)
            {
                AequusHelpers.DropMoney(Player.GetSource_ItemUse(Player.HeldItem), Player.getRect(), fish.value * fish.stack / 5 / 4, quiet: false);
            }
        }
    }
}
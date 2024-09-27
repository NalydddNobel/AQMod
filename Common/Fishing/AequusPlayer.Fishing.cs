using Aequus.Common.DataSets;
using Aequus.Common.Fishing;
using Aequus.Common.Items;
using Aequus.Content.Biomes.Oblivion;
using Aequus.Content.Fishing.Junk;
using Aequus.Content.Vampirism.Items;
using Aequus.Items.Materials.Fish;
using Aequus.Items.Misc;
using Aequus.Items.Misc.FishCatches.LegendaryFish;
using Aequus.Items.Misc.FishCatches.QuestFish;
using Aequus.Items.Misc.FishingBait;
using Aequus.Items.Tools.FishingPoles;
using Aequus.NPCs.Monsters;
using System.Reflection;
using Terraria.DataStructures;

namespace Aequus;
public partial class AequusPlayer : ModPlayer {
    public Item baitUsed;

    private static object Hook_PlayerLoader_CatchFish;

    public void Load_Fishing() {
        Load_FishingHooks();
    }

    #region Hooks
    private void Load_FishingHooks() {
        On_Projectile.FishingCheck_RollItemDrop += On_Projectile_FishingCheck_RollItemDrop;
        On_Main.DrawProj_FishingLine += On_Main_DrawProj_FishingLine;
        Hook_PlayerLoader_CatchFish = Aequus.Detour(typeof(PlayerLoader).GetMethod(nameof(PlayerLoader.CatchFish), BindingFlags.Public | BindingFlags.Static),
            typeof(AequusPlayer).GetMethod(nameof(PlayerLoader_CatchFish), BindingFlags.NonPublic | BindingFlags.Static));
    }

    private static void On_Main_DrawProj_FishingLine(On_Main.orig_DrawProj_FishingLine orig, Projectile proj, ref float polePosX, ref float polePosY, Vector2 mountedCenter) {
        if (Main.player[proj.owner].HeldItem.ModItem is FishingPoleItem fishingPole && !fishingPole.PreDrawFishingLine(proj)) {
            return;
        }
        orig(proj, ref polePosX, ref polePosY, mountedCenter);
    }

    private static void On_Projectile_FishingCheck_RollItemDrop(On_Projectile.orig_FishingCheck_RollItemDrop orig, Projectile self, ref FishingAttempt fisher) {
        if (fisher.playerFishingConditions.Bait?.ModItem is IModifyFishAttempt modBait) {
            if (!modBait.OnItemRoll(self, ref fisher)) {
                return;
            }
        }
        orig(self, ref fisher);
    }

    private delegate void PlayerLoader_CatchFish_orig(Player player, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition);
    private static void PlayerLoader_CatchFish(PlayerLoader_CatchFish_orig orig, Player player, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
        orig(player, attempt, ref itemDrop, ref npcSpawn, ref sonar, ref sonarPosition);

        if (npcSpawn > 0) {
            return;
        }

        bool zenithSeed = Aequus.ZenithSeed;
        // trash drop tables 
        if (itemDrop > 0) {
            var item = ContentSamples.ItemsByType[itemDrop];
            if (npcSpawn <= 0 && item.rare != -1) {
                if (player.Aequus().accDevilsTongue && ((item.rare < ItemRarityID.Blue && item.value < Item.buyPrice(gold: 1)) || (!attempt.legendary && !attempt.veryrare && !attempt.rare))) {
                    itemDrop = Main.rand.Next(ItemSets.FishingTrashForDevilsTounge);
                    item = ContentSamples.ItemsByType[itemDrop];
                }
            }

            bool canCatchBreadOfCthulhu = item.rare == -1 || zenithSeed;
#if !CRAB_CREVICE_DISABLE
            if (player.InModBiome<global::Aequus.Content.Biomes.CrabCrevice.CrabCreviceBiome>()) {
                canCatchBreadOfCthulhu = true;
            }
#endif
            if (canCatchBreadOfCthulhu) {
                int breadMonsterChance = 30;
                if (!Main.dayTime) {
                    breadMonsterChance /= 2; // 1/15

                    if (Main.bloodMoon && !zenithSeed) {
                        breadMonsterChance /= 4; // 1/3
                    }
                }

                if (zenithSeed) {
                    breadMonsterChance *= 2;
                }

                if (Main.rand.NextBool(breadMonsterChance)) {
                    itemDrop = 0;
                    npcSpawn = ModContent.NPCType<BreadOfCthulhu>();
                }
                //else if (Main.rand.NextBool(7)) {
                //    itemDrop = ModContent.ItemType<BreadRoachPainting>();
                //}
                else if (Main.rand.NextBool()) {
                    switch (Main.rand.Next(2)) {
                        case 0: {
                                itemDrop = ModContent.ItemType<PlasticBottle>();
                            }
                            break;

                        case 1: {
                                itemDrop = ModContent.ItemType<Driftwood>();
                            }
                            break;
                    }
                }
            }
        }
    }
    #endregion

    public void Unload_FishingEffects() {
        Hook_PlayerLoader_CatchFish = null;
    }

    #region Catch Fish
    private bool CanCatchQuestFish(int questFish, FishingAttempt attempt) {
        return attempt.questFish == questFish && !Player.HasItem(questFish);
    }

    private void CatchLavaFish(FishingAttempt attempt, ref int itemDrop) {
        if (attempt.fishingLevel <= 0.75f && Main.rand.NextBool(4)) {
            itemDrop = ModContent.ItemType<TatteredDemonHorn>();
        }
        else if (Player.InModBiome<OblivionAltarBiome>() && (attempt.rare || attempt.veryrare) && (Main.rand.NextBool(3) || Aequus.ZenithSeed)) {
            itemDrop = ModContent.ItemType<GoreFish>();
        }
    }

    private void CatchHoneyFish(FishingAttempt attempt, ref int itemDrop) {

    }

    private void CatchWaterFish(FishingAttempt attempt, ref int itemDrop) {
        if (!Main.anglerQuestFinished && Main.rand.NextBool(10)) {
            if (CanCatchQuestFish(ModContent.ItemType<BrickFish>(), attempt) && BrickFish.CheckVillagerBuildings(attempt, Player)) {
                itemDrop = ModContent.ItemType<BrickFish>();
            }
        }

        if (attempt.uncommon) {
            if (Player.ZoneCrimson && Main.rand.NextBool(3)) {
                itemDrop = ModContent.ItemType<Leecheel>();
            }
            if (Player.ZoneCorrupt && Main.rand.NextBool(3)) {
                itemDrop = ModContent.ItemType<Depthscale>();
            }
#if !POLLUTED_OCEAN
            if (Player.ZoneSnow && Main.rand.NextBool(3)) {
                itemDrop = ModContent.ItemType<IcebergFish>();
            }
#endif
            if (Player.ZoneDesert && Main.rand.NextBool(3)) {
                itemDrop = ModContent.ItemType<HeatFish>();
            }
        }

        if (attempt.heightLevel >= HeightLevel.Underground && Main.rand.NextBool(3)) {
            if (attempt.legendary || (Aequus.ZenithSeed && attempt.veryrare)) {
                switch (Main.rand.Next(4)) {
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
        else if (Main.bloodMoon) {
            if ((attempt.uncommon || (attempt.common && Player.GetModPlayer<AequusPlayer>().IsVampire)) && Main.rand.NextBool()) {
                itemDrop = ModContent.ItemType<PalePufferfish>();
            }
            else if (attempt.rare && Main.rand.NextBool(4)) {
                itemDrop = ModContent.ItemType<VampireSquid>();
            }
        }

#if !CRAB_CREVICE_DISABLE
        if (Player.InModBiome<global::Aequus.Content.Biomes.CrabCrevice.CrabCreviceBiome>()) {
            if (attempt.crate && Main.rand.NextBool()) {
                if (Main.hardMode) {
                    itemDrop = ModContent.ItemType<global::Aequus.Items.Misc.GrabBags.Crates.CrabCreviceCrateHard>();
                }
                else {
                    itemDrop = ModContent.ItemType<global::Aequus.Items.Misc.GrabBags.Crates.CrabCreviceCrate>();
                }
            }
            else {
                if (attempt.veryrare && Main.rand.NextBool(4)) {
                    itemDrop = ModContent.ItemType<global::Aequus.Items.Misc.FishCatches.LegendaryFish.CrabDaughter>();
                }
            }
        }
#endif

#if POLLUTED_OCEAN
        if (Player.InModBiome<Content.Biomes.PollutedOcean.PollutedOceanSurface>() || Player.InModBiome<Content.Biomes.PollutedOcean.PollutedOceanUnderground>()) {
            if (Main.rand.NextBool(3)) {
                itemDrop = ModContent.ItemType<Content.Fishing.Fish.Killifish>();
            }
            if (Main.rand.NextBool(3)) {
                itemDrop = ModContent.ItemType<Content.Fishing.Fish.Piraiba>();
            }

            if (attempt.crate) {
                if (Main.hardMode) {
                    itemDrop = Instance<Content.Fishing.Crates.PollutedOceanCrate>().HardmodeVariant.Type;
                }
                else {
                    itemDrop = Instance<Content.Fishing.Crates.PollutedOceanCrate>().PreHardmodeVariant.Type;
                }
            }
        }
#endif
    }

    public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
        baitUsed = attempt.playerFishingConditions.Bait;
        if (baitUsed?.ModItem is IModifyCatchFish modBait && modBait.ModifyCatchFish(attempt, ref itemDrop, ref npcSpawn, ref sonar, ref sonarPosition)) {
            return;
        }

        if (npcSpawn > 0) {
            goto PostProbeFish;
        }

        if (attempt.inLava) {
            CatchLavaFish(attempt, ref itemDrop);
        }
        else if (attempt.inHoney) {
            CatchHoneyFish(attempt, ref itemDrop);
        }
        else {
            CatchWaterFish(attempt, ref itemDrop);
        }

    PostProbeFish:
        if (baitUsed?.ModItem is CrateBait crateBait) {
            CrateBait.ConvertCrate(Player, attempt, ref itemDrop);
        }

        if (Main.myPlayer == Player.whoAmI && baitUsed?.type == ModContent.ItemType<Omnibait>()) {
            omnibait = false;
            Player.UpdateBiomes(); // Kind of cheaty
        }
    }
    #endregion

    public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel) {
        if (fishingRod.ModItem is ItemHooks.IModifyFishingPower modFishingRod) {
            modFishingRod.ModifyFishingPower(Player, this, fishingRod, ref fishingLevel);
        }
        if (bait.ModItem is ItemHooks.IModifyFishingPower modBait) {
            modBait.ModifyFishingPower(Player, this, fishingRod, ref fishingLevel);
        }
    }

    public override void ModifyCaughtFish(Item fish) {
        if (baitUsed?.ModItem is IModifyFishItem modBait) {
            modBait.ModifyFishItem(Player, fish);
        }
        if (Player.HeldItem.ModItem is CrabRod) {
            Helper.DropMoney(Player.GetSource_ItemUse(Player.HeldItem), Player.getRect(), fish.value * fish.stack / 5 / 4, quiet: false);
        }
    }
}
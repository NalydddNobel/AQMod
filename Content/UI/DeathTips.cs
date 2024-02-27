using Aequus.Content.Configuration;
using Aequus.Core;
using Aequus.Core.IO;
using Aequus.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.Utilities;

namespace Aequus.Content.UI;

public class DeathTips : UILayer {
    public bool resetGameTips;

    private static bool CanShow() {
        return ClientConfig.Instance.ShowDeathTips && Main.LocalPlayer.dead && !Main.LocalPlayer.ghost;
    }

    public override void OnPreUpdatePlayers() {
        if (!Active && CanShow()) {
            this.Activate();
        }
    }

    public override bool OnUIUpdate(GameTime gameTime) {
        return CanShow();
    }

    protected override bool DrawSelf() {
        if (Main.gameMenu) {
            return true;
        }

        if (!CanShow()) {
            resetGameTips = true;
            return true;
        }

        if (resetGameTips) {
            Main.gameTips.ClearTips();
            TryAddingBiasedGameTip();
            resetGameTips = false;
        }

        Main.gameTips.Update();
        Main.gameTips.Draw();
        return true;
    }

    public DeathTips() : base("Death Tips", InterfaceLayerNames.PlayerChat_35, InterfaceScaleType.UI) { }

    private string GetIdKey(int id) {
        int offset = 1;
        // Localization and ID set issue. All entries past 64 in the ID set are offset by 2.
        if (id >= 63) {
            offset = 2;
        }
        return $"LoadingTips_Default.{id + offset}";
    }

    #region Biased Tips
    private delegate bool GameTipCondition(Player player);

    private List<BiasedGameTip> _biasedGameTips;

    private readonly FieldInfo FIELD_CURRENT_TIPS = typeof(GameTipsDisplay).GetField("_currentTips", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    private readonly Type GAME_TIP_TYPE = typeof(GameTipsDisplay).GetNestedType("GameTip", BindingFlags.Public | BindingFlags.NonPublic);

    public const float WEIGHT_IMPORTANT = 1f;
    public const float WEIGHT_TOWN_NPCS = 0.01f;
    public const float WEIGHT_MISC = 0.005f;

    public override void OnLoad() {
        const int TORCHES_MINING_HELMET_THRESHOLD = 2;

        _biasedGameTips = new() {
            // Helpful
            FromId(WEIGHT_IMPORTANT, (p) => p.GetModPlayer<DeathTipsPlayerContext>().contextWasBurning,
                GameTipID.ObsidianSkullWithHellstoneOrMeteorite),
            FromId(WEIGHT_IMPORTANT, (p) => p.GetModPlayer<DeathTipsPlayerContext>().contextLavaImmunityCooldown > 0,
                GameTipID.LavaAndObsidianSkinPotion),
            FromId(WEIGHT_IMPORTANT, (p) => p.GetModPlayer<DeathTipsPlayerContext>().contextFallHeight > 0,
                GameTipID.LuckyHorseshoeFallDamage, GameTipID.WaterBreaksFall, GameTipID.CloudsPreventFallDamage),
            FromId(WEIGHT_MISC, (p) => p.ConsumedLifeCrystals == 0 && Main.rand.NextBool(),
                GameTipID.LifeCrystalsForLifeIncrease),
            FromId(WEIGHT_MISC, (p) => !Main.dayTime && p.ConsumedManaCrystals == 0 && Main.rand.NextBool(),
                GameTipID.ManaCrystalCrafting),
            FromId(WEIGHT_MISC, (p) => Main.rand.NextBool() && p.position.Y > Main.worldSurface * 16f && p.CountItem(ItemID.Torch, TORCHES_MINING_HELMET_THRESHOLD) < TORCHES_MINING_HELMET_THRESHOLD && NPC.AnyNPCs(NPCID.Merchant),
                GameTipID.MiningHelmet),
            FromId(WEIGHT_MISC, (p) => !Main.hardMode && Main.rand.NextBool(5) &&(NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && p.position.Y < Main.UnderworldLayer * 16f && ! p.HasItem(ItemID.GuideVoodooDoll) && ! NPC.AnyNPCs(NPCID.WallofFlesh),
                GameTipID.GuideVoodooDollToSummonWOF),
            FromId(WEIGHT_MISC, (p) => Main.hardMode && WorldGen.altarCount == 0,
                GameTipID.PwnHammerForAltars),
            FromId(WEIGHT_MISC, (p) => NPC.downedBoss1 && !WorldGen.shadowOrbSmashed,
                GameTipID.ShadowOrbsDestruction),
            FromId(WEIGHT_IMPORTANT, (p) => Main.invasionType == InvasionID.GoblinArmy && Main.invasionSize > 5 && Main.rand.NextBool(2),
                GameTipID.GoblinArmyCrowdControl),
            FromId(WEIGHT_MISC, (p) => NPC.downedGoblins && !NPC.savedGoblin && Main.rand.NextBool(3),
                GameTipID.GoblinTinkererLocation),
            FromId(WEIGHT_MISC, (p) => Main.hardMode && !p.GetModPlayer<DeathTipsPlayerContext>().contextHasWings && Main.rand.NextBool(5),
                GameTipID.WingsAllowFlight),
            FromId(WEIGHT_MISC, (p) => Main.hardMode && p.ConsumedLifeFruit == 0 && Main.rand.NextBool(5),
                GameTipID.LifeFruitLocation),
            FromId(WEIGHT_MISC, (p) => !p.GetModPlayer<DeathTipsPlayerContext>().contextHasMount && Main.rand.NextBool(5),
                GameTipID.MountMobilityAndUniqueness),
            FromId(WEIGHT_MISC, (p) => Main.hardMode && NPC.downedGolemBoss && !NPC.downedMartians && Main.invasionType == InvasionID.None && Main.rand.NextBool(5),
                GameTipID.MartianProbesSpawnInvasion),
            FromId(WEIGHT_MISC, (p) => Main.hardMode && NPC.downedMechBossAny && !NPC.downedAncientCultist && !Main.eclipse && Main.rand.NextBool(12),
                GameTipID.SolarEclipseCreepyMonsters),
            FromId(WEIGHT_MISC, (p) => Main.hardMode && NPC.downedPlantBoss && !Main.pumpkinMoon && !Main.snowMoon && !(NPC.downedHalloweenTree || NPC.downedHalloweenKing) && Main.rand.NextBool(5),
                GameTipID.PumpkingMoonStarting),
            FromId(WEIGHT_MISC, (p) => Main.hardMode && NPC.downedPlantBoss && !Main.pumpkinMoon && !Main.snowMoon && !(NPC.downedChristmasTree || NPC.downedChristmasSantank|| NPC.downedChristmasIceQueen) && Main.rand.NextBool(5),
                GameTipID.FrostMoonStarting),
            FromId(WEIGHT_MISC, (p) => !NPC.downedQueenBee && Main.rand.NextBool(12),
                GameTipID.WitchDoctorNeedsQueenBeeToMoveIn),
            FromId(WEIGHT_MISC, (p) => WorldState.HardmodeTier && !NPC.savedWizard && Main.rand.NextBool(12),
                GameTipID.WizardLocation),
            FromId(WEIGHT_MISC, (p) => WorldState.HardmodeTier && !NPC.savedTaxCollector && Main.rand.NextBool(12),
                GameTipID.TaxCollectorNeedsToBePurified),

            // Town NPCs
            FromId(WEIGHT_TOWN_NPCS, (p) => Main.rand.NextBool(4) && ! NPC.AnyNPCs(NPCID.Merchant),
                GameTipID.MerchantsNeedMoneyToMoveIn),
            FromId(WEIGHT_TOWN_NPCS, (p) => p.ConsumedLifeCrystals > 0 && Main.rand.NextBool() && !NPC.AnyNPCs(NPCID.Nurse),
                GameTipID.NurseNeedsLifeCrystalToMoveIn),
            FromId(WEIGHT_TOWN_NPCS, (p) => Main.rand.NextBool(4) && ! NPC.AnyNPCs(NPCID.Demolitionist),
                GameTipID.DemolitionistNeedsExplosivesToMoveIn),
            FromId(WEIGHT_TOWN_NPCS, (p) => NPC.downedBoss2 && ! NPC.downedBoss3 && Main.rand.NextBool(4),
                GameTipID.OldManWithCurseIsClothier),

            // Misc
            FromId(WEIGHT_MISC, (p) => Main.rand.NextBool(8) && p.HasItem(ItemID.EmptyBucket),
                GameTipID.CanWearBucketsOnHead),
            FromId(WEIGHT_MISC, (p) => WorldGen.spawnMeteor,
                GameTipID.WatchOutForMeteorites),
            FromId(WEIGHT_MISC, (p) => NPC.downedBoss1 && !Main.hardMode && !p.GetModPlayer<DeathTipsPlayerContext>().hasReachedUnderworld && Main.rand.NextBool(12),
                GameTipID.DigDeeperForUnderworld),
            FromId(WEIGHT_MISC, (p) => !NPC.downedBoss3 && !p.GetModPlayer<DeathTipsPlayerContext>().contextHasCobaltShield && Main.rand.NextBool(8),
                GameTipID.CobaltShieldKnockback),
            FromId(WEIGHT_MISC, (p) => !p.GetModPlayer<DeathTipsPlayerContext>().contextHasGrapplingHook && Main.rand.NextBool(8),
                GameTipID.GrapplingHooksForExploration),
            FromId(WEIGHT_MISC, (p) => !DD2Event.DownedInvasionAnyDifficulty && NPC.downedBoss2 && Main.rand.NextBool(6),
                GameTipID.OldOnesArmyInvasion, GameTipID.TavernkeepOrigins),
        };
    }

    private void TryAddingBiasedGameTip() {
        try {
            object tipList = FIELD_CURRENT_TIPS.GetValue(Main.gameTips);
            List<BiasedGameTip> gameTips = _biasedGameTips.Where(g => g.Condition(Main.LocalPlayer)).ToList();
            if (gameTips.Count <= 0) {
                return;
            }

            WeightedRandom<BiasedGameTip> random = new WeightedRandom<BiasedGameTip>();
            foreach (var t in gameTips) {
                //foreach (string key in t.GameTipKeys) {
                //    Main.NewText(Language.GetTextValue(key), Color.Yellow);
                //}

                random.Add(t, t.Weight);
            }

            BiasedGameTip chosenGameTip = random.Get();
            string chosenGameTipKey = chosenGameTip.GameTipKeys.Length == 1 ? chosenGameTip.GameTipKeys[0] : Main.rand.Next(chosenGameTip.GameTipKeys);
            object gameTipObject = Activator.CreateInstance(GAME_TIP_TYPE, new object[] { chosenGameTipKey, Main.gameTimeCache.TotalGameTime.TotalSeconds });
            MethodInfo addMethod = tipList.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
            addMethod.Invoke(tipList, new object[] { gameTipObject });
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }
    }

    private BiasedGameTip FromId(float weight, GameTipCondition Condition, params int[] ids) {
        return new BiasedGameTip(ids.Select(GetIdKey).ToArray(), Condition, weight);
    }
    private BiasedGameTip FromKeys(float weight, GameTipCondition Condition, params string[] keys) {
        return new BiasedGameTip(keys, Condition, weight);
    }

    private record class BiasedGameTip(string[] GameTipKeys, GameTipCondition Condition, float Weight);
    #endregion
}

public class DeathTipsPlayerContext : ModPlayer {
    public const int FALL_DAMAGE_BLOCK_THRESHOLD = 25;

    public bool contextHasMount;
    public bool contextHasWings;
    public int contextFallHeight;
    public int contextLavaImmunityCooldown;
    public bool contextWasBurning;
    [SaveData("ReachedUnderworld")]
    public bool hasReachedUnderworld;
    public bool contextHasCobaltShield;
    public bool contextHasGrapplingHook;

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
        contextHasWings = Player.equippedWings != null && !Player.equippedWings.IsAir;
        if (contextHasWings || Player.noFallDmg) {
            contextFallHeight = 0;
        }
        else {
            contextFallHeight = (int)(Player.position.Y / 16f) - Player.fallStart - Player.extraFall - FALL_DAMAGE_BLOCK_THRESHOLD;
        }
        contextLavaImmunityCooldown = Player.hurtCooldowns[ImmunityCooldownID.Lava];
        contextWasBurning = Player.HasBuff(BuffID.Burning);
        contextHasCobaltShield = Player.noKnockback;
        contextHasGrapplingHook = Player.miscEquips[Player.miscSlotHook] != null && !Player.miscEquips[Player.miscSlotHook].IsAir;
        contextHasMount = Player.miscEquips[Player.miscSlotMount] != null && !Player.miscEquips[Player.miscSlotMount].IsAir;
    }

    public override void PostUpdate() {
        if (!hasReachedUnderworld && Player.position.Y > Main.UnderworldLayer * 16f) {
            hasReachedUnderworld = true;
        }
    }

    public override void SaveData(TagCompound tag) {
        SaveDataAttribute.SaveData(tag, this);
    }

    public override void LoadData(TagCompound tag) {
        SaveDataAttribute.LoadData(tag, this);
    }
}
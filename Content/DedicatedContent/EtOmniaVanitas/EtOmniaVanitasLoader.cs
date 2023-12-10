using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

public class EtOmniaVanitasLoader : ILoadable {
    public readonly record struct ProgressionCheck(Func<bool> Check, GameProgression Progression);

    public static ModItem Tier1 { get; private set; }

    public static readonly List<ProgressionCheck> GameProgress = new();


    internal static readonly Dictionary<GameProgression, EtOmniaVanitas> ProgressionToItem = new();

    public void Load(Mod mod) {
        GameProgress.Add(new(() => NPC.downedBoss2, GameProgression.EvilBosses));
        GameProgress.Add(new(() => NPC.downedQueenBee, GameProgression.EvilBosses));
        GameProgress.Add(new(() => NPC.downedDeerclops, GameProgression.EvilBosses));
        GameProgress.Add(new(() => NPC.downedBoss3, GameProgression.Skeletron));
        GameProgress.Add(new(() => Main.hardMode, GameProgression.WallOfFleshEarlyHardmode));
        GameProgress.Add(new(() => NPC.downedQueenSlime, GameProgression.WallOfFleshEarlyHardmode));
        GameProgress.Add(new(() => NPC.downedMechBossAny, GameProgression.MechanicalBoss));
        GameProgress.Add(new(() => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, GameProgression.MechanicalBoss3));
        GameProgress.Add(new(() => NPC.downedPlantBoss, GameProgression.Plantera));
        GameProgress.Add(new(() => NPC.downedGolemBoss, GameProgression.Golem));
        GameProgress.Add(new(() => NPC.downedAncientCultist, GameProgression.LunaticCultistPillars));
        GameProgress.Add(new(() => NPC.downedMoonlord, GameProgression.MoonLord));

        Tier1 = AddVanilla(mod, GameProgression.Earlygame, new() {
            Damage = 10,
            AmmoConsumptionReduction = 0f,
            ShootSpeed = 14f,
            UseTime = 15,
            CooldownTime = 1800,
            ChargeShotDamageIncrease = 2f,
            ChargeShotDefenseReduction = 5,
            ChargeShotDefenseReductionDuration = 300
        });
        AddVanilla(mod, GameProgression.EvilBosses, new() {
            Damage = 11,
            AmmoConsumptionReduction = 0f,
            ShootSpeed = 15f,
            UseTime = 15,
            CooldownTime = 1800,
            ChargeShotDamageIncrease = 2f,
            ChargeShotDefenseReduction = 5,
            ChargeShotDefenseReductionDuration = 360
        });
        AddVanilla(mod, GameProgression.Skeletron, new() {
            Damage = 25,
            AmmoConsumptionReduction = 0f,
            ShootSpeed = 20f,
            UseTime = 14,
            CooldownTime = 1800,
            ChargeShotDamageIncrease = 2f,
            ChargeShotDefenseReduction = 5,
            ChargeShotDefenseReductionDuration = 420
        });
        AddVanilla(mod, GameProgression.WallOfFleshEarlyHardmode, new() {
            Damage = 26,
            AmmoConsumptionReduction = 0.2f,
            ShootSpeed = 34f,
            UseTime = 12,
            CooldownTime = 1500,
            ChargeShotDamageIncrease = 3f,
            ChargeShotDefenseReduction = 8,
            ChargeShotDefenseReductionDuration = 420,
            FrostburnDebuff = BuffID.Frostburn2
        });
        AddVanilla(mod, GameProgression.MechanicalBoss, new() {
            Damage = 37,
            AmmoConsumptionReduction = 0.25f,
            ShootSpeed = 35f,
            UseTime = 10,
            CooldownTime = 1500,
            ChargeShotDamageIncrease = 3f,
            ChargeShotDefenseReduction = 10,
            ChargeShotDefenseReductionDuration = 420,
            FrostburnDebuff = BuffID.Frostburn2
        });
        AddVanilla(mod, GameProgression.MechanicalBoss3, new() {
            Damage = 40,
            AmmoConsumptionReduction = 0.33f,
            ShootSpeed = 36f,
            UseTime = 10,
            CooldownTime = 1500,
            ChargeShotDamageIncrease = 4f,
            ChargeShotDefenseReduction = 10,
            ChargeShotDefenseReductionDuration = 480,
            FrostburnDebuff = BuffID.Frostburn2
        });
        AddVanilla(mod, GameProgression.Plantera, new() {
            Damage = 46,
            AmmoConsumptionReduction = 0.4f,
            ShootSpeed = 37f,
            UseTime = 8,
            CooldownTime = 1200,
            ChargeShotDamageIncrease = 4f,
            ChargeShotDefenseReduction = 10,
            ChargeShotDefenseReductionDuration = 480,
            FrostburnDebuff = BuffID.Frostburn2
        });
        AddVanilla(mod, GameProgression.Golem, new() {
            Damage = 60,
            AmmoConsumptionReduction = 0.5f,
            ShootSpeed = 38f,
            UseTime = 6,
            CooldownTime = 1200,
            ChargeShotDamageIncrease = 4f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 480,
            FrostburnDebuff = BuffID.Frostburn2
        });
        AddVanilla(mod, GameProgression.LunaticCultistPillars, new() {
            Damage = 75,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 5,
            CooldownTime = 1200,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2
        });
        AddVanilla(mod, GameProgression.MoonLord, new() {
            Damage = 222,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 4,
            CooldownTime = 600,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2
        });

        static EtOmniaVanitas AddVanilla(Mod mod, GameProgression progress, EtOmniaVanitas.ScaledStats stats) {
            var add = new EtOmniaVanitas(progress, stats with { Rarity = (int)progress + 1 });
            mod.AddContent(add);
            return add;
        }
    }

    public void Unload() {
        GameProgress.Clear();
        ProgressionToItem.Clear();
    }

    public static bool TryGet(GameProgression progression, out ModItem modItem) {
        bool value = ProgressionToItem.TryGetValue(progression, out var etOmniaVanitas);
        modItem = etOmniaVanitas;
        return value;
    }

    public static GameProgression GetGameProgress() {
        var value = GameProgression.Earlygame;
        foreach (var g in GameProgress) {
            if (g.Progression > value && g.Check.Invoke()) {
                value = g.Progression;
            }
        }
        return value;
    }
}
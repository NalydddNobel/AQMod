using Aequus.Common.Items.Dedications;
using Aequus.Content.CrossMod.CalamityModSupport;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Content.Dedicated.EtOmniaVanitas;

public class EtOmniaVanitasLoader : ILoad {
    public readonly record struct ProgressionCheck(Func<bool> Check, GameProgression Progression);

    public static ModItem Tier1 { get; private set; }

    public static readonly List<ProgressionCheck> GameProgress = new();


    internal static readonly Dictionary<GameProgression, EtOmniaVanitas> ProgressionToItem = new();

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

        Tier1 = Add(mod, GameProgression.Earlygame, new() {
            Damage = 10,
            AmmoConsumptionReduction = 0f,
            ShootSpeed = 14f,
            UseTime = 15,
            CooldownTime = 1800,
            ChargeShotDamageIncrease = 2f,
            ChargeShotDefenseReduction = 5,
            ChargeShotDefenseReductionDuration = 300
        });
        Add(mod, GameProgression.EvilBosses, new() {
            Damage = 11,
            AmmoConsumptionReduction = 0f,
            ShootSpeed = 15f,
            UseTime = 15,
            CooldownTime = 1800,
            ChargeShotDamageIncrease = 2f,
            ChargeShotDefenseReduction = 5,
            ChargeShotDefenseReductionDuration = 360
        });
        Add(mod, GameProgression.Skeletron, new() {
            Damage = 25,
            AmmoConsumptionReduction = 0f,
            ShootSpeed = 20f,
            UseTime = 14,
            CooldownTime = 1800,
            ChargeShotDamageIncrease = 2f,
            ChargeShotDefenseReduction = 5,
            ChargeShotDefenseReductionDuration = 420
        });
        Add(mod, GameProgression.WallOfFleshEarlyHardmode, new() {
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
        Add(mod, GameProgression.MechanicalBoss, new() {
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
        Add(mod, GameProgression.MechanicalBoss3, new() {
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
        Add(mod, GameProgression.Plantera, new() {
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
        Add(mod, GameProgression.Golem, new() {
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
        Add(mod, GameProgression.LunaticCultistPillars, new() {
            Damage = 75,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 5,
            CooldownTime = 1200,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2,
            Rarity = new Stats.IdRarity(ItemRarityID.Red)
        });
        Add(mod, GameProgression.MoonLord, GetMoonLordTierStats());

        AddCalamityTiers(mod);
    }

    private static Stats.Scale GetMoonLordTierStats() {
        int damage = 222;
        if (CalamityMod.IsEnabled()) {
            damage = 111;
        }

        return new() {
            Damage = damage,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 4,
            CooldownTime = 600,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2,
            Rarity = new Stats.IdRarity(ItemRarityID.Purple)
        };
    }

    private static void AddCalamityTiers(Mod mod) {
        if (!CalamityMod.TryGet(out Mod calamity)) {
            return;
        }

        GameProgress.Add(new(() => CalamityMod.Downed("providence"), GameProgression.Calamity_Providence));
        GameProgress.Add(new(() => CalamityMod.Downed("polterghast"), GameProgression.Calamity_Polterghast));
        GameProgress.Add(new(() => CalamityMod.Downed("devourerofgods"), GameProgression.Calamity_DoG));
        GameProgress.Add(new(() => CalamityMod.Downed("yharon"), GameProgression.Calamity_Yharon));
        GameProgress.Add(new(() => CalamityMod.Downed("exomechs") || CalamityMod.Downed("calamitas"), GameProgression.Calamity_SCaLExos));

        Add(mod, GameProgression.Calamity_Providence, new() {
            Damage = 130,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 4,
            CooldownTime = 600,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2,
            Rarity = new Stats.ModdedRarity(calamity, "Turquoise")
        });
        Add(mod, GameProgression.Calamity_Polterghast, new() {
            Damage = 160,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 4,
            CooldownTime = 600,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2,
            Rarity = new Stats.ModdedRarity(calamity, "PureGreen")
        });
        Add(mod, GameProgression.Calamity_DoG, new() {
            Damage = 200,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 4,
            CooldownTime = 600,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2,
            Rarity = new Stats.ModdedRarity(calamity, "DarkBlue")
        });
        Add(mod, GameProgression.Calamity_Yharon, new() {
            Damage = 235,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 4,
            CooldownTime = 600,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2,
            Rarity = new Stats.ModdedRarity(calamity, "Violet")
        });
        Add(mod, GameProgression.Calamity_SCaLExos, new() {
            Damage = 500,
            AmmoConsumptionReduction = 0.66f,
            ShootSpeed = 40f,
            UseTime = 4,
            CooldownTime = 600,
            ChargeShotDamageIncrease = 5f,
            ChargeShotDefenseReduction = 15,
            ChargeShotDefenseReductionDuration = 600,
            FrostburnDebuff = BuffID.Frostburn2,
            Rarity = new Stats.ModdedRarity(calamity, "HotPink")
        });
    }

    private static EtOmniaVanitas Add(Mod mod, GameProgression progress, Stats.Scale stats) {
        if (stats.Rarity == null) {
            stats = stats with { Rarity = new Stats.IdRarity((int)progress + 1) };
        }
        EtOmniaVanitas add = new EtOmniaVanitas(progress, stats);

        mod.AddContent(add);

        // Register this as a dedicated item
        if (Tier1 == null) {
            DedicationRegistry.Register(add, new Dedication.Default("Yuki-Miyako", new Color(60, 60, 120)));
        }
        else {
            DedicationRegistry.RegisterSubItem(Tier1, add);
        }

        return add;
    }

    public void Unload() {
        Tier1 = null;
        GameProgress.Clear();
        ProgressionToItem.Clear();
    }
}
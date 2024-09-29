using Aequus.Common;
using Aequus.Common.Entities.Players;
using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Entities.PotionAffixes.Empowered;

public class EmpoweredDatabase : LoadedType {
    public readonly Dictionary<int, EmpoweredAffixInfo> Info = [];

    public static readonly float WaterWalking_MoveSpeed = 0.5f;
    public static readonly float WaterWalking_SprintSpeed = 0.5f;
    public static readonly float WaterWalking_RunAccelMultiplier = 1.5f;
    public static readonly float ObsidianSkin_DefenseMultiplier = 2f;
    public static readonly float Flipper_MoveSpeed = 0.25f;
    public static readonly float Flipper_SprintSpeed = 0.25f;
    public static readonly float Flipper_RunAccelMultiplier = 1.25f;
    public static readonly float Swiftness_MoveSpeed = 0.25f;
    public static readonly float Swiftness_SprintSpeed = 0.25f;
    public static readonly int Invisibility_Aggro = -400;
    public static readonly int Featherfall_FlightIncrease = 60;
    public static readonly int Gills_Defense = 20;
    public static readonly int Regeneration_LifeRegen = 4;
    public static readonly int Ironskin_Defense = 8;
    public static readonly float MagicPower_MagicDamage = 0.1f;
    public static readonly float Shine_LightMultiplier = 2f;
    public static readonly float Thorns_Thorns = 2f;
    public static readonly float Archery_ArrowDamage = 0.1f;
    public static readonly float Mining_PickSpeed = 0.25f;
    public static readonly int Fishing_FishingSkill = 25;
    public static readonly float Builder_PlacementSpeed = 0.25f;
    public static readonly int Builder_BlockRange = 1;
    public static readonly int Summoning_MaxMinions = 1;
    public static readonly float Endurance_DamageReduction = 0.1f;
    public static readonly float Rage_CritChance = 10f;
    public static readonly float Wrath_Damage = 0.1f;
    public static readonly float Lucky_LuckRerolls = 1f;

    protected override void PostSetupContent() {
        Add(BuffID.WaterWalking, UpdateWaterWalking);

        void UpdateWaterWalking(Player player) {
            Tile standingTile = Framing.GetTileSafely(player.Bottom);
            if (!standingTile.IsSolid() && standingTile.LiquidAmount > 0) {
                player.moveSpeed += WaterWalking_MoveSpeed;
                player.GetModPlayer<SprintPlayer>().GroundedSprintSpeed += WaterWalking_SprintSpeed;
                player.runAcceleration *= WaterWalking_RunAccelMultiplier;
            }
        }

        Add(BuffID.Warmth, UpdateWarmth);

        void UpdateWarmth(Player player) {
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
        }

        Add(BuffID.ObsidianSkin, UpdateObsidianSkin);

        void UpdateObsidianSkin(Player player) {
            if (player.wet) {
                player.statDefense *= ObsidianSkin_DefenseMultiplier;
            }
        }

        Add(BuffID.Flipper, UpdateFlipper);

        void UpdateFlipper(Player player) {
            if (player.wet) {
                player.moveSpeed += Flipper_MoveSpeed;
                player.GetModPlayer<SprintPlayer>().GroundedSprintSpeed += Flipper_SprintSpeed;
                player.runAcceleration *= Flipper_RunAccelMultiplier;
            }
        }

        Add(BuffID.Gills, UpdateGills, tooltipArgs: [Gills_Defense]);

        void UpdateGills(Player player) {
            if (player.wet) {
                player.statDefense += Gills_Defense;
            }
        }

        Add(BuffID.Swiftness, UpdateSwiftness, tooltipArgs: [Gills_Defense]);

        void UpdateSwiftness(Player player) {
            player.moveSpeed += 0.25f;
            player.GetModPlayer<SprintPlayer>().GroundedSprintSpeed += Flipper_SprintSpeed;
        }

        Add(BuffID.Builder, UpdateBuilder, tooltipArgs: [Gills_Defense]);

        void UpdateBuilder(Player player) {
            player.tileSpeed += Builder_PlacementSpeed;
            player.wallSpeed += Builder_PlacementSpeed;
            player.blockRange += Builder_BlockRange;
        }

        Add(BuffID.Invisibility, p => p.aggro += Invisibility_Aggro);
        Add(BuffID.Featherfall, p => p.GetModPlayer<AequusPlayer>().flightStats.wingTime.Flat += Featherfall_FlightIncrease, tooltipArgs: [ALanguage.Seconds(Featherfall_FlightIncrease)]);
        Add(BuffID.ManaRegeneration, p => p.manaRegenCount += 60);
        Add(BuffID.Titan, p => p.GetKnockback<GenericDamageClass>() += 1f);
        Add(BuffID.Dangersense, p => p.InfoAccMechShowWires = true);

        Add(BuffID.Regeneration, p => p.lifeRegen += Regeneration_LifeRegen);
        Add(BuffID.Ironskin, p => p.statDefense += Ironskin_Defense);
        Add(BuffID.MagicPower, p => p.GetDamage<MagicDamageClass>() += MagicPower_MagicDamage, percent: 0.5f);
        Add(BuffID.Shine, p => Lighting.AddLight(p.Center, new Vector3(0.8f, 0.95f, 1f) * Shine_LightMultiplier));
        Add(BuffID.Thorns, p => p.thorns = Math.Max(p.thorns, Thorns_Thorns));
        Add(BuffID.Archery, p => p.arrowDamage += Archery_ArrowDamage);
        Add(BuffID.Mining, p => p.pickSpeed -= Mining_PickSpeed);
        Add(BuffID.Fishing, p => p.fishingSkill += Fishing_FishingSkill);
        Add(BuffID.Summoning, p => p.maxMinions += Summoning_MaxMinions);
        Add(BuffID.Lifeforce, p => p.statLifeMax2 += p.statLifeMax / 5 / 20 * 20);
        Add(BuffID.Endurance, p => p.SafelyAddDamageReduction(Endurance_DamageReduction));
        Add(BuffID.Rage, p => p.GetCritChance<GenericDamageClass>() += Rage_CritChance);
        Add(BuffID.Wrath, p => p.GetDamage<GenericDamageClass>() += Wrath_Damage);
        Add(BuffID.Lucky, p => p.GetModPlayer<AequusPlayer>().luckRerolls += Lucky_LuckRerolls);
    }

    void Add(int buffId, Action<Player> customAction, float percent = 1f, object[]? tooltipArgs = null) {
        string name = BuffID.Search.GetName(buffId);
        LocalizedText tooltip = Instance<EmpoweredPrefix>().GetLocalization($"Description.{name}");
        if (tooltipArgs != null) {
            tooltip = tooltip.WithFormatArgs(tooltipArgs);
        }
        Info.Add(buffId, new EmpoweredAffixInfo(tooltip, customAction, percent));
    }

    public void Add(ModBuff buff, Action<Player>? CustomAction = null, float Percentage = 1f, object[]? DescriptionArguments = null) {
        LocalizedText tooltip = buff.GetLocalization("EmpoweredDescription");
        if (DescriptionArguments != null) {
            tooltip = tooltip.WithFormatArgs(DescriptionArguments);
        }
        Add(buff, tooltip, CustomAction, Percentage);
    }
    public void Add(ModBuff buff, LocalizedText Tooltip, Action<Player>? CustomAction = null, float Percentage = 1f) {
        Info.Add(buff.Type, new EmpoweredAffixInfo(Tooltip, CustomAction, Percentage));
    }
}

public readonly record struct EmpoweredAffixInfo(LocalizedText Tooltip, Action<Player>? CustomAction, float Percent = 1f);
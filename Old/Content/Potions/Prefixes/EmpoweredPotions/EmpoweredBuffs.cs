using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Old.Content.Potions.Prefixes.EmpoweredPotions;

public class EmpoweredBuffs : MetadataSet {
    [JsonProperty]
    public static HashSet<Entry<BuffID>> Blacklist { get; private set; } = new();
    [JsonIgnore]
    public static Dictionary<Entry<BuffID>, EmpoweredOverride> Override { get; private set; } = new();

    public record struct EmpoweredOverride(float Percent = 1f, Action<Player> CustomAction = null, LocalizedText Tooltip = null);

    public override void Load() {
        Blacklist.Add(BuffID.Gravitation);
        Blacklist.Add(BuffID.Sonar);
        Blacklist.Add(BuffID.BiomeSight);
        Blacklist.Add(BuffID.Hunter);
        Blacklist.Add(BuffID.Honey);
        Blacklist.Add(BuffID.Inferno);

        Override.Add(BuffID.WaterWalking, new(CustomAction: (p) => {
            Tile standingTile = Framing.GetTileSafely(p.Bottom);
            if (!standingTile.IsSolid() && standingTile.LiquidAmount > 0) {
                p.moveSpeed += 0.5f;
                p.runAcceleration *= 1.5f;
            }
        }));
        Override.Add(BuffID.Warmth, new(CustomAction: (p) => {
            p.buffImmune[BuffID.Chilled] = true;
            p.buffImmune[BuffID.Frozen] = true;
        }));
        Override.Add(BuffID.Invisibility, new(CustomAction: (p) => p.aggro -= 400, Tooltip: Language.GetText("Mods.Aequus.Items.CommonTooltips.DecreaseAggro").WithFormatArgs(1)));
        Override.Add(BuffID.Featherfall, new(CustomAction: (p) => p.GetModPlayer<AequusPlayer>().wingTime += 60, Tooltip: Language.GetText("Mods.Aequus.Items.CommonTooltips.StatWingTime").WithFormatArgs(1)));
        Override.Add(BuffID.ObsidianSkin, new(CustomAction: (p) => {
            if (p.wet) {
                p.statDefense += 20;
            }
        }, Tooltip: Language.GetText("CommonItemTooltip.IncreasesDefenseBy").WithFormatArgs(20)));
        Override.Add(BuffID.Flipper, new(CustomAction: (p) => {
            if (p.wet) {
                p.moveSpeed += 0.25f;
            }
        }));
        Override.Add(BuffID.Gills, new(CustomAction: (p) => {
            if (p.wet) {
                p.statDefense += 20;
            }
        }, Tooltip: Language.GetText("CommonItemTooltip.IncreasesDefenseBy").WithFormatArgs(20)));
        Override.Add(BuffID.ManaRegeneration, new(CustomAction: (p) => p.statManaMax += 100, Tooltip: Language.GetText("CommonItemTooltip.IncreasesMaxManaBy").WithFormatArgs(100)));
        Override.Add(BuffID.Titan, new(CustomAction: (p) => p.GetKnockback<GenericDamageClass>() += 1f));
        Override.Add(BuffID.Dangersense, new(CustomAction: (p) => p.InfoAccMechShowWires = true, Tooltip: Language.GetText("ItemTooltip.MechanicalLens")));

        Override.Add(BuffID.Regeneration, new(CustomAction: (p) => p.lifeRegen += 4));
        Override.Add(BuffID.Swiftness, new(CustomAction: (p) => p.moveSpeed += 0.25f));
        Override.Add(BuffID.Ironskin, new(CustomAction: (p) => p.statDefense += 8));
        Override.Add(BuffID.MagicPower, new(Percent: 0.5f, CustomAction: (p) => p.GetDamage<MagicDamageClass>() += 0.1f));
        Override.Add(BuffID.Shine, new(CustomAction: (p) => Lighting.AddLight(p.Center, new Vector3(0.8f, 0.95f, 1f) * 2f)));
        Override.Add(BuffID.Thorns, new(CustomAction: (p) => p.thorns = Math.Max(p.thorns, 2f)));
        // Vanilla multiplies arrow damage by 1.1
        // To add another 10% damage, we simply add 10%
        Override.Add(BuffID.Archery, new(CustomAction: (p) => p.arrowDamage += 0.1f));
        Override.Add(BuffID.Mining, new(CustomAction: (p) => p.pickSpeed -= 0.25f));
        Override.Add(BuffID.Fishing, new(CustomAction: (p) => p.fishingSkill += 15));
        Override.Add(BuffID.Builder, new(CustomAction: (p) => {
            p.tileSpeed += 0.25f;
            p.wallSpeed += 0.25f;
            p.blockRange++;
        }));
        Override.Add(BuffID.Summoning, new(CustomAction: (p) => p.maxMinions++));
        Override.Add(BuffID.Lifeforce, new(CustomAction: (p) => p.statLifeMax2 += p.statLifeMax / 5 / 20 * 20));
        // Vanilla simply subtracts 10%, but we multiply it
        Override.Add(BuffID.Endurance, new(CustomAction: (p) => p.endurance *= 0.9f));
        Override.Add(BuffID.Rage, new(CustomAction: (p) => p.GetCritChance<GenericDamageClass>() += 10f));
        Override.Add(BuffID.Wrath, new(CustomAction: (p) => p.GetDamage<GenericDamageClass>() += 10f));
        // Add +1 to the luck potion stat (capped at 3)
        // Effectively makes the tiers 2-3-3 instead of 1-2-3
        Override.Add(BuffID.Lucky, new(CustomAction: (p) => p.luckPotion = (byte)Math.Min(p.luckPotion + 1, 3)));
    }
}

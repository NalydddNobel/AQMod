using Aequu2.Content.Configuration;
using Aequu2.Content.Dedicated;
using Aequu2.Content.Dedicated.SwagEye;
using Aequu2.Content.Items.Accessories.Balloons;
using Aequu2.Content.Items.Accessories.Informational.Monocle;
using Aequu2.Content.Items.Accessories.WeightedHorseshoe;
using Aequu2.Content.Items.PermaPowerups.Shimmer;
using Aequu2.Content.Items.Weapons.Magic.Furystar;
using Aequu2.Core.Components.Prefixes;
using Aequu2.Core.Entities.Items.Dedications;
using tModLoaderExtended.Terraria.GameContent;

namespace Aequu2.Core.Systems;

public class ShimmerSystem : ModSystem {
    public override void PostSetupContent() {
#if !DEBUG
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<Old.Content.Items.Accessories.GrandReward.GrandReward>()] = ModContent.ItemType<CosmicChest>();
#endif
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<RichMansMonocle>()] = ModContent.ItemType<ShimmerMonocle>();
        ExtendedShimmer.SetShimmerResultSafely(ItemID.TinkerersWorkshop, ModContent.ItemType<TinkerersGuidebook>());
        ExtendedShimmer.SetShimmerResultSafely(ItemID.SuspiciousLookingEye, ModContent.GetInstance<SwagEyePet>().PetItem.Type);

        ExtendedShimmer.CreateShimmerLoop(ModContent.ItemType<Furystar>(), ItemID.Starfury);
        ExtendedShimmer.CreateShimmerLoop(ModContent.ItemType<SlimyBlueBalloon>(), ItemID.ShinyRedBalloon);
        ExtendedShimmer.CreateShimmerLoop(ModContent.ItemType<WeightedHorseshoe>(), ItemID.LuckyHorseshoe);
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            ItemSets.ShimmerTransformToItem[ItemID.TreasureMagnet] = ItemID.CelestialMagnet;
            ItemSets.ShimmerTransformToItem[ItemID.CelestialMagnet] = ItemID.TreasureMagnet;
        }

#if !DEBUG
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<Old.Content.Items.Materials.SoulGem.SoulGemFilled>()] = ModContent.ItemType<Old.Content.Items.Materials.SoulGem.SoulGem>();
        ExtendedShimmer.CreateShimmerLoop(
            ModContent.ItemType<Old.Content.Items.Weapons.Ranged.CrusadersCrossbow.CrusadersCrossbow>(),
            ModContent.ItemType<Old.Content.Items.GrapplingHooks.HealingGrappleHook.LeechHook>(),
            ModContent.ItemType<Old.Content.Items.Accessories.HighSteaks.HighSteaks>(),
            ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.SaivoryKnife>()
        );
        ExtendedShimmer.CreateShimmerLoop(
            ModContent.ItemType<Old.Content.Items.Accessories.Pacemaker.Pacemaker>(),
            ModContent.ItemType<Old.Content.Items.Accessories.HoloLens.HoloLens>(),
            ModContent.ItemType<Old.Content.Items.Tools.MagicMirrors.PhaseMirror.PhaseMirror>()
        );
#endif

        ExtendedShimmer shimmer = ModContent.GetInstance<ExtendedShimmer>();
        foreach (ModItem item in Mod.GetContent<ModItem>()) {
            if (ItemSets.ShimmerTransformToItem[item.Type] == -1 && DedicationRegistry.Contains(item)) {
                shimmer.AddShimmerOverride(item.Type, new ShimmerFaelingsOverride());
            }
        }

        shimmer.AddGlobalShimmerOverride(new PrefixClearingOverride());
    }

    public struct ShimmerFaelingsOverride : IShimmerOverride {
        readonly bool IShimmerOverride.GetShimmered(Item item, int type) {
            DedicatedFaeling.SpawnFaelingsFromShimmer(item, item.ModItem);
            return true;
        }

        readonly bool? IShimmerOverride.CanShimmer(Item item, int type) {
            return true;
        }
    }

    public struct PrefixClearingOverride : IShimmerOverride {
        readonly bool IShimmerOverride.GetShimmered(Item item, int type) {
            if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is IRemovedByShimmerPrefix shimmerablePrefix && shimmerablePrefix.CanBeRemovedByShimmer) {
                int oldStack = item.stack;
                item.SetDefaults(item.netID);
                item.stack = oldStack;
                item.prefix = 0;
                item.shimmered = true;

                ExtendedShimmer.GetShimmered(item);
                return true;
            }
            return false;
        }

        readonly bool? IShimmerOverride.CanShimmer(Item item, int type) {
            return null;
        }
    }
}
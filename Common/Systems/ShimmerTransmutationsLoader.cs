using Aequus.Common.ItemPrefixes.Components;
using Aequus.Common.Items.Dedications;
using Aequus.Content.Configuration;
using Aequus.Content.Dedicated;
using Aequus.Content.Dedicated.SwagEye;
using Aequus.Content.Items.Accessories.Balloons;
using Aequus.Content.Items.Accessories.WeightedHorseshoe;
using Aequus.Content.Items.PermaPowerups.Shimmer;
using Aequus.Content.Items.Weapons.Magic.Furystar;
using tModLoaderExtended.Terraria.GameContent;
using tModLoaderExtended.Terraria.ModLoader;
using Aequus.Content.Items.Accessories.Informational.Monocle;

namespace Aequus.Common.Systems;

public class ShimmerTransmutationsLoader : IPostSetupContent {
    void IPostSetupContent.PostSetupContent(Mod mod) {
#if !DEBUG
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<Old.Content.Equipment.Accessories.GrandReward.GrandReward>()] = ModContent.ItemType<CosmicChest>();
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
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<Old.Content.Materials.SoulGem.SoulGemFilled>()] = ModContent.ItemType<Old.Content.Materials.SoulGem.SoulGem>();
        ExtendedShimmer.CreateShimmerLoop(
            ModContent.ItemType<Old.Content.Weapons.Ranged.Bows.CrusadersCrossbow.CrusadersCrossbow>(),
            ModContent.ItemType<Old.Content.Equipment.GrapplingHooks.HealingGrappleHook.LeechHook>(),
            ModContent.ItemType<Old.Content.Equipment.Accessories.HighSteaks.HighSteaks>(),
            ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.SaivoryKnife>()
        );
        ExtendedShimmer.CreateShimmerLoop(
            ModContent.ItemType<Old.Content.Equipment.Accessories.Pacemaker.Pacemaker>(),
            ModContent.ItemType<Old.Content.Equipment.Accessories.HoloLens.HoloLens>(),
            ModContent.ItemType<Old.Content.Tools.MagicMirrors.PhaseMirror.PhaseMirror>()
        );
#endif

        ExtendedShimmer shimmer = ModContent.GetInstance<ExtendedShimmer>();
        foreach (ModItem item in mod.GetContent<ModItem>()) {
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
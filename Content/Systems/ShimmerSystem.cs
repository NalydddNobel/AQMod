using AequusRemake.Content.Configuration;
using AequusRemake.Content.Dedicated;
using AequusRemake.Content.Dedicated.SwagEye;
using AequusRemake.Content.Items.Accessories.Balloons;
using AequusRemake.Content.Items.Accessories.Informational.Monocle;
using AequusRemake.Content.Items.Accessories.WeightedHorseshoe;
using AequusRemake.Content.Items.PermaPowerups.Shimmer;
using AequusRemake.Content.Items.Weapons.Magic.Furystar;
using AequusRemake.Core.Components.Prefixes;
using AequusRemake.Core.Entities.Items.Dedications;
using tModLoaderExtended.Terraria.GameContent;

namespace AequusRemake.Core.Systems;

public class ShimmerSystem : ModSystem {
    public override void PostSetupContent() {
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
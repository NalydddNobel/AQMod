using Aequus.Common.ItemPrefixes.Components;
using Aequus.Common.Items;
using Aequus.Common.Items.Components;
using Aequus.Content.Configuration;
using Aequus.Content.DedicatedContent;
using Aequus.Content.DedicatedContent.SwagEye;
using Aequus.Content.Equipment.Accessories.Balloons;
using Aequus.Content.Equipment.Accessories.GrandReward;
using Aequus.Content.Equipment.Accessories.Informational.Monocle;
using Aequus.Content.Equipment.Accessories.WeightedHorseshoe;
using Aequus.Content.Tools.MagicMirrors.PhaseMirror;
using Aequus.Content.Weapons.Magic.Furystar;
using System.Security.Cryptography;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;

namespace Aequus.Common.Systems;

public class ShimmerSystem : ModSystem {
    public override void Load() {
        On_Item.GetShimmered += On_Item_GetShimmered;
        On_ShimmerTransforms.IsItemTransformLocked += On_ShimmerTransforms_IsItemTransformLocked;
        On_Item.CanShimmer += On_Item_CanShimmer;
    }

    public static void RegisterShimmerTransmutations() {
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<GrandReward>()] = ModContent.ItemType<CosmicChest>();
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<RichMansMonocle>()] = ModContent.ItemType<ShimmerMonocle>();
        ItemSets.ShimmerTransformToItem[ItemID.SuspiciousLookingEye] = ModContent.GetInstance<SwagEyePet>().PetItem.Type;

        CreateShimmerLoop(ModContent.ItemType<Furystar>(), ItemID.Starfury);
        CreateShimmerLoop(ModContent.ItemType<SlimyBlueBalloon>(), ItemID.ShinyRedBalloon);
        CreateShimmerLoop(ModContent.ItemType<WeightedHorseshoe>(), ItemID.LuckyHorseshoe);
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            ItemSets.ShimmerTransformToItem[ItemID.TreasureMagnet] = ItemID.CelestialMagnet;
            ItemSets.ShimmerTransformToItem[ItemID.CelestialMagnet] = ItemID.TreasureMagnet;
        }

#if !DEBUG
        ItemSets.ShimmerTransformToItem[ModContent.ItemType<Old.Content.Materials.SoulGem.SoulGemFilled>()] = ModContent.ItemType<Old.Content.Materials.SoulGem.SoulGem>();
        CreateShimmerLoop(
            ModContent.ItemType<Old.Content.Weapons.Ranged.Bows.CrusadersCrossbow.CrusadersCrossbow>(),
            ModContent.ItemType<Old.Content.Equipment.GrapplingHooks.HealingGrappleHook.LeechHook>(),
            ModContent.ItemType<Old.Content.Equipment.Accessories.HighSteaks.HighSteaks>(),
            ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.SaivoryKnife>()
        );
        CreateShimmerLoop(
            ModContent.ItemType<Old.Content.Equipment.Accessories.Pacemaker.Pacemaker>(),
            ModContent.ItemType<Old.Content.Equipment.Accessories.HoloLens.HoloLens>(),
            ModContent.ItemType<PhaseMirror>()
        );
#endif
    }

    private static void CreateShimmerLoop(params int[] items) {
        for (int i = 0; i < items.Length; i++) {
            SetShimmerResultSafely(items[i], items[(i + 1) % items.Length]);
        }
    }

    /// <summary>Attempts to safely set the shimmer result of this item, by inserting it to the end of existing chains or loops.</summary>
    /// <param name="from"></param>
    /// <param name="into"></param>
    private static void SetShimmerResultSafely(int from, int into) {
        if (ItemSets.ShimmerCountsAsItem[from] > -1) {
            from = ItemSets.ShimmerCountsAsItem[from];
        }

        // Abort fancy logic if it already converts into the item, or is an invalid Id (0 or larger than ItemLoader.ItemCount)
        if (ItemSets.ShimmerTransformToItem[from] == into || ItemSets.ShimmerTransformToItem[from] <= 0 || ItemSets.ShimmerTransformToItem[from] >= ItemLoader.ItemCount) {
            ItemSets.ShimmerTransformToItem[from] = into;
            return;
        }

        // Get the endpoint of the shimmer tree we are inserting into.
        int fromEndpoint = GetShimmerTreeEndpoint(from, out _);

        // Now check if the item we are linking onto is part of a tree.
        if (ItemSets.ShimmerTransformToItem[into] > 0 && ItemSets.ShimmerTransformToItem[into] < ItemLoader.ItemCount) {
            // If so, get the endpoint of that item's shimmer tree.
            int intoEndpoint = GetShimmerTreeEndpoint(into, out _);

            // If the endpoint is also the same as the from item, don't set it
            // (ItemSets.ShimmerTransformToItem[from] = from;)
            if (intoEndpoint != from) {
                ItemSets.ShimmerTransformToItem[intoEndpoint] = from;
            }
        }

        // If the endpoint is also the same as the into item, don't set it
        // (ItemSets.ShimmerTransformToItem[into] = into;)
        if (fromEndpoint != into) {
            ItemSets.ShimmerTransformToItem[fromEndpoint] = into;
        }
    }

    private static int GetShimmerTreeEndpoint(int start, out ShimmerTransmutationTreeType type) {
        int lastType = start;
        for (int i = 0; i < ItemLoader.ItemCount; i++) {
            // Reached an endpoint where the item has no further conversions.
            if (ItemSets.ShimmerTransformToItem[lastType] <= 0) {
                type = ShimmerTransmutationTreeType.Line;
                return lastType;
            }

            // Reached an endpoint where the item returns to the starting item.
            if (ItemSets.ShimmerTransformToItem[lastType] == start) {
                type = ShimmerTransmutationTreeType.Loop;
                return lastType;
            }

            lastType = ItemSets.ShimmerTransformToItem[lastType];
            if (ItemSets.ShimmerCountsAsItem[lastType] > -1) {
                lastType = ItemSets.ShimmerCountsAsItem[lastType];
            }
        }

        // Reached a broken endpoint where it must loop, but never reach the starting item.
        type = ShimmerTransmutationTreeType.Broken;
        return lastType;
    }

    public override void SetStaticDefaults() {
        RegisterShimmerTransmutations();
    }

    #region Hooks
    private static bool On_ShimmerTransforms_IsItemTransformLocked(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type) {
        if (AequusRecipes.ShimmerTransformLocks.TryGetValue(type, out var condition)) {
            return !condition.IsMet();
        }

        return orig(type);
    }

    private static bool On_Item_CanShimmer(On_Item.orig_CanShimmer orig, Item item) {
        if (ItemLoader.GetItem(item.type) is IDedicatedItem) {
            return true;
        }

        return orig(item);
    }

    private static void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item item) {
        ModItem modItem = ItemLoader.GetItem(item.type);

        if (modItem is IOnShimmer onShimmer && !onShimmer.OnShimmer()) {
            return;
        }

        if (modItem is IDedicatedItem) {
            int maximumSpawnable = 50;
            int highestNPCSlotIndexWeWillPick = 200;
            int slotsAvailable = NPC.GetAvailableAmountOfNPCsToSpawnUpToSlot(item.stack, highestNPCSlotIndexWeWillPick);
            while (maximumSpawnable > 0 && slotsAvailable > 0 && item.stack > 0) {
                maximumSpawnable--;
                slotsAvailable--;
                item.stack--;
                int npc = NPC.NewNPC(item.GetSource_FromThis(), (int)item.Bottom.X, (int)item.Bottom.Y, ModContent.NPCType<DedicatedFaeling>());
                if (npc >= 0) {
                    Main.npc[npc].shimmerTransparency = 1f;
                    NetMessage.SendData(MessageID.ShimmerActions, -1, -1, null, 2, npc);
                }
            }
            item.shimmered = true;
            if (item.stack <= 0) {
                item.TurnToAir();
            }
            GetShimmeredEffects(item);
            return;
        }

        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is IRemovedByShimmerPrefix shimmerablePrefix && shimmerablePrefix.CanBeRemovedByShimmer) {
            int oldStack = item.stack;
            item.SetDefaults(item.netID);
            item.stack = oldStack;
            item.prefix = 0;
            item.shimmered = true;

            GetShimmeredEffects(item);
            return;
        }

        orig(item);
    }
    #endregion

    /// <summary>Emulates all vanilla shimmer on-transmutation effects. Including getting the achievement, the sound, particles, networking, ect.</summary>
    /// <param name="item"></param>
    public static void GetShimmeredEffects(Item item) {
        if (item.stack > 0) {
            item.shimmerTime = 1f;
        }
        else {
            item.shimmerTime = 0f;
        }

        item.shimmerWet = true;
        item.wet = true;
        item.velocity *= 0.1f;
        if (Main.netMode == NetmodeID.SinglePlayer) {
            Item.ShimmerEffect(item.Center);
        }
        else {
            NetMessage.SendData(146, -1, -1, null, 0, (int)item.Center.X, (int)item.Center.Y);
            NetMessage.SendData(145, -1, -1, null, item.whoAmI, 1f);
        }
        AchievementsHelper.NotifyProgressionEvent(27);
    }

    public enum ShimmerTransmutationTreeType {
        /// <summary>Items convert into each other, eventually ending with an item with no conversion.</summary>
        Line,
        /// <summary>Items convert into each other, and will cycle infinitely.</summary>
        Loop,
        /// <summary>Items will cycle, but will never reach the starting point.</summary>
        Broken
    }
}
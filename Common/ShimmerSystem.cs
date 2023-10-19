using Aequus.Common.Items;
using Aequus.Content.DedicatedContent;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common;

public class ShimmerSystem : ModSystem {
    public override void Load() {
        On_Item.GetShimmered += On_Item_GetShimmered;
        On_ShimmerTransforms.IsItemTransformLocked += On_ShimmerTransforms_IsItemTransformLocked;
        On_Item.CanShimmer += On_Item_CanShimmer;
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
        if (ItemLoader.GetItem(item.type) is IDedicatedItem) {
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
            TransmutateItem(item);
            return;
        }
        if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is UniqueItemPrefix prefix && prefix.CanBeShimmeredAway) {
            int oldStack = item.stack;
            item.SetDefaults(item.netID);
            item.stack = oldStack;
            item.prefix = 0;
            item.shimmered = true;

            TransmutateItem(item);
            return;
        }
        orig(item);
    }
    #endregion

    public static void TransmutateItem(Item item) {
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
}
using Aequus.CrossMod.Common;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public static class NPCHelper {
    public static bool TryRetargeting(this NPC npc, bool faceTarget = true) {
        npc.TargetClosest(faceTarget: faceTarget);
        return npc.HasValidTarget;
    }

    private static bool BuffImmuneCommon(int npcId, out NPCDebuffImmunityData buffImmunities) {
        if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(npcId, out buffImmunities) || buffImmunities == null || buffImmunities.ImmuneToAllBuffsThatAreNotWhips) {
            return true;
        }

        return false;
    }

    public static bool IsProbablyACritter(this NPC npc) {
        return NPCID.Sets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1);
    }

    #region Buffs
    public static void SetBuffImmune(int npcID, int buffID) {
        if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(npcID, out var data)) {
            NPCID.Sets.DebuffImmunitySets[npcID] = new() { SpecificallyImmuneTo = new[] { buffID } };
            return;
        }

        if (data.ImmuneToAllBuffsThatAreNotWhips || data.SpecificallyImmuneTo.Contains(buffID)) {
            return;
        }

        Array.Resize(ref data.SpecificallyImmuneTo, data.SpecificallyImmuneTo.Length + 1);
        data.SpecificallyImmuneTo[^1] = buffID;
    }

    public static bool IsBuffsImmune(int npcId, params int[] buffIds) {
        return !BuffImmuneCommon(npcId, out var buffImmunities) && buffImmunities.SpecificallyImmuneTo != null && buffImmunities.SpecificallyImmuneTo.ContainsAny(buffIds);
    }

    public static bool IsBuffImmune(int npcId, int buffId) {
        return !BuffImmuneCommon(npcId, out var buffImmunities) && buffImmunities.SpecificallyImmuneTo != null && buffImmunities.SpecificallyImmuneTo.ContainsAny(buffId);
    }
    #endregion

    #region Shops 
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, int customValue, params Condition[] conditions) {
        var item = new Item(itemType) {
            shopCustomPrice = customValue
        };
        return shop.Add(item, conditions);
    }
    public static NPCShop AddCustomValue<T>(this NPCShop shop, int customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue(ModContent.ItemType<T>(), customValue, conditions);
    }
    public static NPCShop AddCustomValue(this NPCShop shop, int itemType, double customValue, params Condition[] conditions) {
        return shop.AddCustomValue(itemType, (int)Math.Round(customValue), conditions);
    }
    public static NPCShop AddCustomValue<T>(this NPCShop shop, double customValue, params Condition[] conditions) where T : ModItem {
        return shop.AddCustomValue<T>((int)Math.Round(customValue), conditions);
    }

    public static int FindNextShopSlot(Item[] items) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i] == null) {
                return i;
            }
        }
        return -1;
    }

    internal static NPCShop AddCrossMod<T>(this NPCShop shop, string itemName, params Condition[] conditions) where T : ModSupport<T> {
        if (!ModSupport<T>.TryFind<ModItem>(itemName, out var modItem)) {
            return shop;
        }
        return shop.Add(modItem.Type, conditions);
    }
    #endregion
}
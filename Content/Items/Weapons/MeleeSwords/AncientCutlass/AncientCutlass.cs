using Aequus.Common.DataSets;
using Aequus.Common.Utilities.Helpers;
using Aequus.Common.Utilities.Sampling;
using System;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Items.Weapons.MeleeSwords.AncientCutlass;

public class AncientCutlass : ModItem {
    public static readonly int DropChance = 2;
    public static readonly int ValueDropChanceDecrease = Item.silver * 3;

    public override void SetStaticDefaults() {
        //Element.Water.AddItem(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.DyeTradersScimitar);
        Item.crit = 8;
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
        NPC realTarget = target;
        if (realTarget.realLife > -1) {
            realTarget = Main.npc[realTarget.realLife];
        }

        int dropChance = DropChance * Math.Max(target.rarity + 1, 1);

        // Check if the pickpocket should occur at all.
        if (NPCSets.PickpocketOverride.TryGetValue(target.type, out bool val)) {
            if (!val) {
                return;
            }
        }
        else if (target.IsABoss()) {
            return;
        }

        if (!Main.rand.NextBool(dropChance)) {
            return;
        }

        List<IItemDropRule> drops = Main.ItemDropsDB.GetRulesForNPCID(realTarget.netID, includeGlobalDrops: false);

        if (drops.Count == 0) {
            return;
        }

        NewItemCache.Instance.Begin();
        try {
            ItemDropAttemptResult result;
            do {
                int randomIndex = Main.rand.Next(drops.Count);
                IItemDropRule dropRule = Main.rand.Next(drops);
                DropAttemptInfo dropInfo = LootUtils.GetDropAttemptInfo(realTarget, player);

                result = LootUtils.ResolveRule(dropRule, in dropInfo);
                drops.RemoveAt(randomIndex);
            }
            while (drops.Count > 0 && result.State != ItemDropAttemptResultState.Success);
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }
        NewItemCache.Instance.End();

        Rectangle hitbox = target.Hitbox;
        foreach (Item item in NewItemCache.Instance.Items) {
            if (item.stack < 1) {
                continue;
            }

            item.stack = Math.Max(item.stack / 10, 1);
            int valueDropChance = item.value / ValueDropChanceDecrease * item.stack;
            if (valueDropChance > 0 && !Main.rand.NextBool(valueDropChance + 1)) {
                continue;
            }

            Item.NewItem(target.GetSource_Loot("AequusRemake: Steal"), hitbox, item);
        }
    }
}

using Aequus.Common.Items.DropRules;
using Aequus.Content.CrossMod.SplitSupport;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.CrossMod.SplitSupport.ItemContent.Envelopes;

public abstract class BaseEnvelope : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = Split.Instance != null ? 10 : 0;
    }

    public override void SetDefaults() {
        Item.width = 22;
        Item.height = 20;
        Item.rare = ItemRarityID.LightRed;
        Item.maxStack = Terraria.Item.CommonMaxStack;
    }

    public override bool CanRightClick() => true;

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 5, 5));
    }

    protected void AddPreHardmodeBasic(ItemLoot itemLoot) {
        itemLoot.Add(ItemDropRule.ByCondition(new NeedLifeCrystalCondition(), ItemID.LifeCrystal, 4));
        itemLoot.Add(ItemDropRule.ByCondition(new NeedManaCrystalCondition(), ItemID.ManaCrystal, 2));
    }
    protected void AddHardmodeBasic(ItemLoot itemLoot) {
        itemLoot.Add(ItemDropRule.ByCondition(new NeedLifeFruitCondition(), ItemID.LifeFruit, 2)
            .OnFailedRoll(ItemDropRule.Common(ItemID.LifeCrystal, 2)));
    }

    //public override void RightClick(Player player) {
    //    var itemSource = player.GetSource_GiftOrReward(Name);
    //    _rewardSelector.Clear();

    //    // some common stats
    //    var useHardmodeRewards = false;
    //    var secondaryRewardChance = 0.15f;
    //    var consolationGold = 5;
    //    var gold = 5;

    //    ModifyRewards(player, ref useHardmodeRewards, ref secondaryRewardChance, ref gold, ref consolationGold);

    //    // choose unique reward
    //    if (_rewardSelector.elements.Count > 0) {
    //        var type = _rewardSelector.Get();
    //        player.QuickSpawnItem(itemSource, type);

    //        // save the item in player's list so it doesn't get repeated next time
    //        player.Photography().ClaimedEnvelopeRewards.Add(ContentSamples.ItemsByType[type].Clone());

    //    }
    //    else {
    //        gold += consolationGold;
    //    }

    //    // give gold
    //    player.QuickSpawnItem(itemSource, ItemID.GoldCoin, gold);

    //    if (useHardmodeRewards) {
    //        if (Main.rand.NextBool(2) && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
    //            player.QuickSpawnItem(itemSource, ItemID.LifeFruit);
    //        else if (Main.rand.NextBool(3))
    //            player.QuickSpawnItem(itemSource, ItemID.LifeCrystal);
    //    }
    //    else {
    //        if (Main.rand.NextBool(4) && player.statLifeMax < 400)
    //            player.QuickSpawnItem(itemSource, ItemID.LifeCrystal);
    //        else if (Main.rand.NextBool(2) && player.statManaMax < 200)
    //            player.QuickSpawnItem(itemSource, ItemID.ManaCrystal);
    //    }
    //}
}
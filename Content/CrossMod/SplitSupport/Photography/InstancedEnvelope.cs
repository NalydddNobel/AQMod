using Aequus.Core.CrossMod;
using Aequus.Core.Entities.Items.DropRules;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.CrossMod.SplitSupport.Photography;

[Autoload(false)]
internal class InstancedEnvelope : CrossModItem {
    private readonly string _name;
    private readonly string _realName;
    public override string Name => _name;
    public override string Texture => $"{typeof(InstancedEnvelope).NamespaceFilePath()}/Envelopes/{Name}";

    private readonly bool _preHardmode;

    public readonly List<int> MainItemDrops = new();

    protected override bool CloneNewInstances => true;

    internal InstancedEnvelope(string name, bool preHardmode) {
        _realName = name;
        _name = name + "Envelope";

        _preHardmode = preHardmode;
    }

    public override void Load() {
        ModTypeLookup<ModItem>.RegisterLegacyNames(this, $"Envelope{_realName}");
    }

    public override void OnSetStaticDefaults() {
        Item.ResearchUnlockCount = 10;
    }

    public override void SetDefaults() {
        Item.width = 22;
        Item.height = 20;
        Item.rare = ItemRarityID.LightRed;
        Item.maxStack = Item.CommonMaxStack;
    }

    public override bool CanRightClick() {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 1, 5, 5));
        if (_preHardmode) {
            itemLoot.Add(ItemDropRule.ByCondition(new ConditionLifeCrystals(), ItemID.LifeCrystal, 4));
            itemLoot.Add(ItemDropRule.ByCondition(new ConditionManaCrystals(), ItemID.ManaCrystal, 2));
        }
        else {
            itemLoot.Add(ItemDropRule.ByCondition(new ConditionLifeFruit(), ItemID.LifeFruit, 2)
                .OnFailedRoll(ItemDropRule.Common(ItemID.LifeCrystal, 2)));
        }

        itemLoot.Add(ItemDropRule.OneFromOptions(1, MainItemDrops.ToArray()));
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
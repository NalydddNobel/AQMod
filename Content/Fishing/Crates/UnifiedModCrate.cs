using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace Aequus.Content.Fishing.Crates;

public abstract class UnifiedModCrate : ModTile {
    public ModItem PreHardmodeVariant;
    public ModItem HardmodeVariant;

    public readonly List<IItemDropRule> Primary = [];

    public UnifiedModCrate() {
        PreHardmodeVariant = new InstancedCrateItem(this, "", 0);
        HardmodeVariant = new InstancedCrateItem(this, "Hard", 1);
    }

    public override void Load() {
        Mod.AddContent(PreHardmodeVariant);
        Mod.AddContent(HardmodeVariant);
    }

    public virtual IEnumerable<IItemDropRule> ModifyCrateLoot(ModItem crate, ItemLoot loot) {
        yield return new OneFromRulesRule(1, [.. Primary]);

        // Drop gold coins.
        yield return ItemDropRule.NotScalingWithLuck(ItemID.GoldCoin, 4, 5, 12);

        IItemDropRule[] ores = [
            ItemDropRule.NotScalingWithLuck(ItemID.CopperOre, 1, 20, 35),
            ItemDropRule.NotScalingWithLuck(ItemID.TinOre, 1, 20, 35),
            ItemDropRule.NotScalingWithLuck(ItemID.IronOre, 1, 20, 35),
            ItemDropRule.NotScalingWithLuck(ItemID.LeadOre, 1, 20, 35),
            ItemDropRule.NotScalingWithLuck(ItemID.SilverOre, 1, 20, 35),
            ItemDropRule.NotScalingWithLuck(ItemID.TungstenOre, 1, 20, 35),
            ItemDropRule.NotScalingWithLuck(ItemID.GoldOre, 1, 20, 35),
            ItemDropRule.NotScalingWithLuck(ItemID.PlatinumOre, 1, 20, 35)
        ];
        IItemDropRule[] bars = [
            ItemDropRule.NotScalingWithLuck(ItemID.IronBar, 1, 6, 16),
            ItemDropRule.NotScalingWithLuck(ItemID.LeadBar, 1, 6, 16),
            ItemDropRule.NotScalingWithLuck(ItemID.SilverBar, 1, 6, 16),
            ItemDropRule.NotScalingWithLuck(ItemID.TungstenBar, 1, 6, 16),
            ItemDropRule.NotScalingWithLuck(ItemID.GoldBar, 1, 6, 16),
            ItemDropRule.NotScalingWithLuck(ItemID.PlatinumBar, 1, 6, 16)
        ];

        if (crate.Name.EndsWith("Hard")) {
            // Hardmode Ores
            yield return ItemDropRule.SequentialRulesNotScalingWithLuck(7,
                new OneFromRulesRule(2, [
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.AdamantiteOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.TitaniumOre, 1, 20, 35)
                ]),
                new OneFromRulesRule(1, ores)
            );

            // Hardmode Bars
            IItemDropRule hardmodeBiomeCrateBars = ItemDropRule.SequentialRulesNotScalingWithLuck(4,
                new OneFromRulesRule(3, 2, [
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.AdamantiteBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.TitaniumBar, 1, 5, 16)
                ]),
                new OneFromRulesRule(1, bars)
            );
        }
        else {
            // Ores
            yield return new OneFromRulesRule(7, ores);

            // Bars
            yield return new OneFromRulesRule(4, bars);
        }

        // Potions
        yield return new OneFromRulesRule(3, [
            ItemDropRule.NotScalingWithLuck(ItemID.ObsidianSkinPotion, 1, 2, 4),
            ItemDropRule.NotScalingWithLuck(ItemID.SpelunkerPotion, 1, 2, 4),
            ItemDropRule.NotScalingWithLuck(ItemID.HunterPotion, 1, 2, 4),
            ItemDropRule.NotScalingWithLuck(ItemID.GravitationPotion, 1, 2, 4),
            ItemDropRule.NotScalingWithLuck(ItemID.MiningPotion, 1, 2, 4),
            ItemDropRule.NotScalingWithLuck(ItemID.HeartreachPotion, 1, 2, 4)
        ]);
    }

    public sealed override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileTable[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        OnSetStaticDefaults();
        TileObjectData.addTile(Type);
    }

    protected virtual void OnSetStaticDefaults() { }

    public override bool CreateDust(int i, int j, ref int type) {
        return false;
    }
}

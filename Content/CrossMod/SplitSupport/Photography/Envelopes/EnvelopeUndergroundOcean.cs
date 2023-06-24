using Aequus.Content.Biomes.CrabCrevice;
using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod.SplitSupport.Photography.Envelopes;
public class EnvelopeUndergroundOcean : BaseEnvelope {
    public override void ModifyItemLoot(ItemLoot itemLoot) {
        base.ModifyItemLoot(itemLoot);
        AddPreHardmodeBasic(itemLoot);
        itemLoot.Add(ItemDropRule.OneFromOptions(1, Array.ConvertAll(CrabCreviceBiome.ChestPrimaryLoot, i => i.item)));
        itemLoot.Add(ItemDropRule.OneFromOptions(2, Array.ConvertAll(CrabCreviceBiome.ChestSecondaryLoot, i => i.item)));
        itemLoot.Add(ItemDropRule.OneFromOptions(4, Array.ConvertAll(CrabCreviceBiome.ChestTertiaryLoot, i => i.item)));
    }
}
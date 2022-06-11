using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class GrandReward : ModItem
    {
        public override void Load()
        {
            On.Terraria.NPC.NPCLoot_DropMoney += Hook_NoMoreMoney;
            On.Terraria.GameContent.ItemDropRules.ItemDropResolver.ResolveRule += Hook_RerollLoot;
        }
        private void Hook_NoMoreMoney(On.Terraria.NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
        {
            if (closestPlayer.GetModPlayer<GrandRewardPlayer>().noMoreMoney)
            {
                return;
            }
            orig(self, closestPlayer);
        }
        private static ItemDropAttemptResult Hook_RerollLoot(On.Terraria.GameContent.ItemDropRules.ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            var result = orig(self, rule, info);
            if (info.player != null && result.State == ItemDropAttemptResultState.FailedRandomRoll)
            {
                if (AequusHelpers.iterations == 0)
                {
                    for (float luckLeft = info.player.GetModPlayer<GrandRewardPlayer>().lootLuck; luckLeft > 0f; luckLeft--)
                    {
                        if (luckLeft < 1f)
                        {
                            if (Main.rand.NextFloat(1f) > luckLeft)
                            {
                                return result;
                            }
                        }
                        var result2 = orig(self, rule, info);
                        AequusHelpers.iterations++;
                        if (result2.State != ItemDropAttemptResultState.FailedRandomRoll)
                        {
                            AequusHelpers.iterations = 0;
                            return result2;
                        }
                    }
                    AequusHelpers.iterations = 0;
                }
                else
                {
                    AequusHelpers.iterations++;
                }
            }
            return result;
        }

        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GrandRewardPlayer>().lootLuck += 1f;
            player.GetModPlayer<GrandRewardPlayer>().noMoreMoney = true;
        }
    }

    public class GrandRewardPlayer : ModPlayer
    {
        /// <summary>
        /// Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.
        /// <para>Used by <see cref="GrandReward"/></para> 
        /// </summary>
        public float lootLuck;

        public bool noMoreMoney;

        public override void ResetEffects()
        {
            lootLuck = 0f;
            noMoreMoney = false;
        }
    }
}
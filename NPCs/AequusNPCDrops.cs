using Aequus.Items.Consumables.Foods;
using Aequus.Items.Weapons.Melee;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public sealed class AequusNPCDrops : GlobalNPC
    {
        public override void Load()
        {
            On.Terraria.GameContent.ItemDropRules.ItemDropResolver.ResolveRule += ItemDropResolver_ResolveRule;
        }

        private static ItemDropAttemptResult ItemDropResolver_ResolveRule(On.Terraria.GameContent.ItemDropRules.ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info)
        {
            var result = orig(self, rule, info);
            if (info.player != null && result.State == ItemDropAttemptResultState.FailedRandomRoll)
            {
                if (AequusHelpers.iterations == 0)
                {
                    for (float luckLeft = info.player.Aequus().lootLuck; luckLeft > 0f; luckLeft--)
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

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.UndeadViking || npc.type == NPCID.ArmoredViking)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalDagger>(), 12));
            }
            else if (npc.type == NPCID.DevourerHead || npc.type == NPCID.GiantWormHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.TombCrawlerHead
                || npc.type == NPCID.DiggerHead || npc.type == NPCID.DuneSplicerHead || npc.type == NPCID.SeekerHead || npc.type == NPCID.BloodEelHead)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 25));
            }
        }
    }
}
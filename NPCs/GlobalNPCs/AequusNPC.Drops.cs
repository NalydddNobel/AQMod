using Aequus.Common.ItemDrops;
using Aequus.Common.Preferences;
using Aequus.Items.Accessories.Misc;
using Aequus.Items.Accessories.Offense;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.Festive;
using Aequus.Items.Placeable.Furniture.CraftingStation;
using Aequus.Items.Vanity.Cursors;
using Aequus.Items.Weapons.Melee;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public partial class AequusNPC : GlobalNPC, IPostSetupContent, IAddRecipes
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(ItemDropRule.ByCondition(new VictorsReward.DropCondition(), ModContent.ItemType<VictorsReward>()));
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            ModifyLoot_Mimics(npc, npcLoot);
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    {
                        if (npcLoot.Find<ItemDropWithConditionRule>((i) => i.itemId == ItemID.DemoniteOre, out var itemDropRule))
                        {
                            itemDropRule.amountDroppedMinimum /= 2;
                            itemDropRule.amountDroppedMaximum /= 2;
                        }
                        if (npcLoot.Find((i) => i.itemId == ItemID.CrimtaneOre, out itemDropRule))
                        {
                            itemDropRule.amountDroppedMinimum /= 2;
                            itemDropRule.amountDroppedMaximum /= 2;
                        }
                    }
                    break;

                case NPCID.DD2Betsy:
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<IronLotus>()));
                    }
                    break;

                case NPCID.Everscream:
                case NPCID.SantaNK1:
                case NPCID.IceQueen:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<FrolicEnergy>()));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsChristmas(), ModContent.ItemType<FrolicEnergy>()));
                    break;

                case NPCID.SlimeRibbonWhite:
                case NPCID.SlimeRibbonYellow:
                case NPCID.SlimeRibbonGreen:
                case NPCID.SlimeRibbonRed:
                case NPCID.ZombieXmas:
                case NPCID.ZombieSweater:
                case NPCID.MisterStabby:
                case NPCID.SnowmanGangsta:
                case NPCID.SnowBalla:
                case NPCID.ZombieElf:
                case NPCID.ZombieElfBeard:
                case NPCID.ZombieElfGirl:
                case NPCID.ElfArcher:
                case NPCID.Nutcracker:
                case NPCID.NutcrackerSpinning:
                case NPCID.Yeti:
                case NPCID.ElfCopter:
                case NPCID.Krampus:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsChristmas(), ModContent.ItemType<FrolicEnergy>()));
                    break;

                case NPCID.Scarecrow1:
                case NPCID.Scarecrow2:
                case NPCID.Scarecrow3:
                case NPCID.Scarecrow4:
                case NPCID.Scarecrow5:
                case NPCID.Scarecrow6:
                case NPCID.Scarecrow7:
                case NPCID.Scarecrow8:
                case NPCID.Scarecrow9:
                case NPCID.Scarecrow10:
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HorrificEnergy>()));
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ItemID.Ale, 1), "Press B.", new NameTagCondition("birdy", "beardy")));
                    break;

                case NPCID.Bunny:
                case NPCID.ExplosiveBunny:
                case NPCID.BunnyXmas:
                case NPCID.BunnySlimed:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ModContent.ItemType<RabbitsFoot>(), 1), "You're a Monster.", new NameTagCondition("toast")));
                    break;

                case NPCID.Moth:
                case NPCID.Mothron:
                case NPCID.MothronSpawn:
                    npcLoot.Add(ItemDropRule.ByCondition(new NameTagCondition("cata", "cataclysmic", "armageddon", "cataclysmicarmageddon", "cataclysmic armageddon"), ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.Unicorn:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ModContent.ItemType<RabbitsFoot>(), 1), "Tattered Pegasus Wings", new NameTagCondition("pegasus")));
                    break;

                case NPCID.Crab:
                    npcLoot.Add(new NameTagDropRule(new ItemDrop(ItemID.GoldCoin, 1), "Me first dollar!", new NameTagCondition("mr krabs", "krabs", "mr. krabs")));
                    break;

                case NPCID.Pumpking:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<HorrificEnergy>()));
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HorrificEnergy>()));
                    break;
                case NPCID.MourningWood:
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<HorrificEnergy>(), 5));
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HorrificEnergy>()));
                    break;

                case NPCID.SlimeMasked:
                case NPCID.ZombieDoctor:
                case NPCID.ZombieSuperman:
                case NPCID.ZombiePixie:
                case NPCID.DemonEyeOwl:
                case NPCID.DemonEyeSpaceship:
                case NPCID.Raven:
                case NPCID.SkeletonTopHat:
                case NPCID.SkeletonAstonaut:
                case NPCID.SkeletonAlien:
                case NPCID.Ghost:
                case NPCID.HoppinJack:
                case NPCID.Splinterling:
                case NPCID.Hellhound:
                case NPCID.HeadlessHorseman:
                case NPCID.Poltergeist:
                    npcLoot.Add(ItemDropRule.ByCondition(new IsHalloweenCondition(), ModContent.ItemType<HorrificEnergy>()));
                    break;

                case NPCID.Demon:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonCursor>(), 100));
                    break;

                case NPCID.Pixie:
                    //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PixieCandle>(), 100));
                    break;

                case NPCID.BloodNautilus:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 6, maximumDropped: 12));
                    break;

                case NPCID.BloodEelHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 3, maximumDropped: 6));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 10));
                    break;

                case NPCID.GoblinShark:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 3, maximumDropped: 6));
                    break;

                case NPCID.EyeballFlyingFish:
                case NPCID.ZombieMerman:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 1, maximumDropped: 3));
                    break;

                case NPCID.DevourerHead:
                case NPCID.GiantWormHead:
                case NPCID.BoneSerpentHead:
                case NPCID.TombCrawlerHead:
                case NPCID.DiggerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.SeekerHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 10));
                    break;

                case NPCID.MoonLordCore:
                    if (GameplayConfig.Instance.EarlyGravityGlobe)
                        npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.GravityGlobe);
                    if (GameplayConfig.Instance.EarlyPortalGun)
                        npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.PortalGun);
                    break;

                case NPCID.CultistBoss:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.FlawlessCondition, ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.NotExpertCondition, ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => !AequusWorld.downedEventDemon, "DemonSiege", "Mods.Aequus.DropCondition.NotBeatenDemonSiege"), ModContent.ItemType<GoreNest>()));
                    break;
            }
        }
    }

    internal static partial class GlobalNPCExtensions
    {
        public static void LockDrops(int npcID, IItemDropRuleCondition conditon, Func<IItemDropRule, bool> check)
        {
            var rules = Main.ItemDropsDB.GetRulesForNPCID(npcID);
            var badRules = new List<IItemDropRule>();
            for (int i = 0; i < rules.Count; i++)
            {
                if (check(rules[i]))
                {
                    badRules.Add(rules[i]);
                }
            }
            foreach (var r in badRules)
            {
                Main.ItemDropsDB.RemoveFromNPC(npcID, r);
                Main.ItemDropsDB.RegisterToNPC(npcID, new LeadingConditionRule(conditon)).OnSuccess(r);
            }
        }
    }
}
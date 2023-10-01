﻿using Aequus.Common.Buffs;
using Aequus.Common.NPCs.Global;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus {
    public static partial class Helper {
        #region Drops
        public static void Clear(this NPCLoot npcLoot) {
            npcLoot.RemoveWhere(i => true);
        }

        public static void AddRange(this NPCLoot npcLoot, IEnumerable<IItemDropRuleChainAttempt> dropRules) {
            foreach (var i in dropRules) {
                npcLoot.Add(i.RuleToChain);
            }
        }
        public static void AddRange(this NPCLoot npcLoot, IEnumerable<IItemDropRule> dropRules) {
            foreach (var i in dropRules) {
                npcLoot.Add(i);
            }
        }
        
        public static List<IItemDropRule> FindAll(this NPCLoot npcLoot, Predicate<IItemDropRule> match) {
            var rules = npcLoot.Get(includeGlobalDrops: false);
            return rules.FindAll(match);
        }

        public static (IItemDropRule BaseRule, IItemDropRule LastConditionRule, IItemDropRule LastRule) AddMultiConditionRule(this NPCLoot npcLoot, IItemDropRule rule, params IItemDropRuleCondition[] condition) {
            IItemDropRule baseRule = new LeadingConditionRule(condition[0]);
            IItemDropRule conditionRule = baseRule;
            npcLoot.Add(conditionRule);
            for (int i = 1; i < condition.Length; i++) {
                conditionRule = conditionRule.OnSuccess(new LeadingConditionRule(condition[i]));
            }
            conditionRule.OnSuccess(rule);
            return (baseRule, conditionRule, rule);
        }
        public static IItemDropRule AddConditionRule(this NPCLoot npcLoot, IItemDropRuleCondition condition, IItemDropRule rule) {
            return npcLoot.Add(new LeadingConditionRule(condition)).OnSuccess(rule);
        }

        public static NPCLoot GetNPCLoot(int npcId, ItemDropDatabase database = null) {
            return new NPCLoot(npcId, database ?? Main.ItemDropsDB);
        }
        public static NPCLoot GetNPCLoot(this NPC npc, ItemDropDatabase database = null) {
            return GetNPCLoot(npc.netID, database);
        }
        public static List<IItemDropRule> GetDropRules(int npcId, ItemDropDatabase database = null) {
            return (database ?? Main.ItemDropsDB).GetRulesForNPCID(npcId, includeGlobalDrops: false);
        }
        public static List<IItemDropRule> GetDropRules(this NPC npc, ItemDropDatabase database = null) {
            return GetDropRules(npc.netID, database);
        }

        public static void InheritDropRules(int parentNPCId, int childNPCId, ItemDropDatabase database = null) {
            var drops = GetDropRules(parentNPCId, database);
            var npcLoot = GetNPCLoot(childNPCId, database);
            foreach (var d in drops) {
                npcLoot.Add(d);
            }
        }
        #endregion

        public static bool TryRetargeting(this NPC npc, bool faceTarget = true) {
            npc.TargetClosest(faceTarget: faceTarget);
            return npc.HasValidTarget;
        }

        public static bool IsImmune(int npcId, params int[] buffIds) {
            if (NPCID.Sets.ImmuneToAllBuffs[npcId]) {
                return true;
            }
            foreach (int buffId in buffIds) {
                if (NPCID.Sets.ImmuneToRegularBuffs[npcId] && !BuffID.Sets.IsATagBuff[buffId]) {
                    return true;
                }
                if (NPCID.Sets.SpecificDebuffImmunity[npcId][buffId] == true) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsImmune(this NPC npc, int buffId) {
            return IsImmune(npc.type, buffId);
        }
        public static bool IsImmune(int npcId, int buffId) {
            return NPCID.Sets.SpecificDebuffImmunity[npcId][buffId] == true;
        }
        /// <summary>
        /// Attempts to add buffs in array order.
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="duration"></param>
        /// <param name="amount">Amount of buffs to apply, this can be used to have a priority system.
        /// <para>For example, Crimson and Corruption Hellfire set this parameter to 1. So when they are inflicted and an enemy is immune, it will try to inflict Hellfire as Corruption/Crimson Hellfire failed to be applied.</para>Anything below 0 means it will try to apply all buffs.</param>
        /// <param name="buffs"></param>
        /// <returns></returns>
        public static bool AddBuffs(this NPC npc, int duration, int amount = 0, params int[] buffs) {
            if (buffs == null) {
                return false;
            }

            bool output = false;
            for (int i = 0; i < buffs.Length; i++) {
                if (npc.buffImmune[buffs[i]]) {
                    continue;
                }

                npc.AddBuff(buffs[i], duration);
                amount--;
                if (npc.HasBuff(buffs[i])) {
                    output = true;
                }
                if (amount == 0) {
                    break;
                }
            }
            return output;
        }

        public static bool AnyNPCWithCondition(Func<NPC, bool> conditon) {
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && conditon(Main.npc[i])) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsABoss(this NPC npc) {
            return npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
        }

        public static bool IsAPillar(this NPC npc) {
            return npc.type switch {
                NPCID.LunarTowerVortex => true,
                NPCID.LunarTowerStardust => true,
                NPCID.LunarTowerNebula => true,
                NPCID.LunarTowerSolar => true,
                _ => false
            };
        }

        public static bool DropsItem(IItemDropRule rule, int itemType) {
            switch (rule) {
                case CommonDrop commonDrop:
                    if (commonDrop.itemId == itemType)
                        return true;
                    break;
                case DropBasedOnExpertMode dropBasedOnExpertMode:
                    if (DropsItem(dropBasedOnExpertMode.ruleForNormalMode, itemType))
                        return true;
                    if (DropsItem(dropBasedOnExpertMode.ruleForExpertMode, itemType))
                        return true;
                    break;
            }
            return false;
        }

        public static bool DropsItem(int npcID, int itemType) {
            var rulesList = Main.ItemDropsDB.GetRulesForNPCID(npcID, includeGlobalDrops: false);

            foreach (var rule in rulesList) {
                if (DropsItem(rule, itemType)) {
                    return true;
                }
            }

            return false;
        }

        public static void SetIDStaticHitCooldown(this NPC npc, int projID, uint time) {
            Projectile.perIDStaticNPCImmunity[projID][npc.whoAmI] = Main.GameUpdateCount + time;
        }
        public static void SetIDStaticHitCooldown<T>(this NPC npc, uint time) where T : ModProjectile {
            SetIDStaticHitCooldown(npc, ModContent.ProjectileType<T>(), time);
        }

        #region Buffs
        public static void CleanupAndSyncBuffs(this NPC npc) {
            for (int i = 0; i < NPC.maxBuffs; i++) {
                if (npc.buffTime[i] == 0 || npc.buffType[i] == 0) {
                    for (int j = i + 1; j < NPC.maxBuffs; j++) {
                        npc.buffTime[j - 1] = npc.buffTime[j];
                        npc.buffType[j - 1] = npc.buffType[j];
                        npc.buffTime[j] = 0;
                        npc.buffType[j] = 0;
                    }
                }
            }
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.NPCBuffs, -1, -1, null, npc.whoAmI);
            }
        }

        public static void ClearAllDebuffs(this NPC npc) {
            bool needsSync = false;
            for (int i = 0; i < NPC.maxBuffs; i++) {
                if (npc.buffTime[i] > 0 && npc.buffType[i] > 0 && AequusBuff.IsDebuff(npc.buffType[i])) {
                    npc.buffTime[i] = 0;
                    npc.buffType[i] = 0;
                    needsSync = true;
                }
            }
            if (needsSync)
                CleanupAndSyncBuffs(npc);
        }

        public static void ClearBuffs(this NPC npc, IEnumerable<int> type) {
            bool needsSync = false;
            foreach (var buffType in type) {
                int index = npc.FindBuffIndex(buffType);
                if (index != -1) {
                    npc.buffTime[index] = 0;
                    npc.buffType[index] = 0;
                    needsSync = true;
                }
            }
            if (needsSync)
                CleanupAndSyncBuffs(npc);
        }

        public static void ClearBuff(this NPC npc, int type) {
            int index = npc.FindBuffIndex(type);
            if (index != -1) {
                npc.DelBuff(type);
            }
        }

        public static void AddBuffToHeadOrSelf(this NPC npc, int buffID, int buffDuration) {
            if (npc.realLife != -1) {
                Main.npc[npc.realLife].AddBuff(buffID, buffDuration);
                return;
            }

            npc.AddBuff(buffID, buffDuration);
        }
        #endregion

        public static void Kill(this NPC npc) {
            npc.StrikeInstantKill();
        }
        public static void KillEffects(this NPC npc, bool quiet = false) {
            npc.life = Math.Min(npc.life, -1);
            npc.HitEffect();
            npc.active = false;
            if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, 9999 + npc.lifeMax * 2 + npc.defense * 2);
            }
        }

        public static void HideBestiaryEntry(this ModNPC npc) {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(npc.Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true, });
        }

        public static void CollideWithOthers(this NPC npc, float speed = 0.05f) {
            var rect = npc.getRect();
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && i != npc.whoAmI && npc.type == Main.npc[i].type
                    && rect.Intersects(Main.npc[i].getRect())) {
                    npc.velocity += Main.npc[i].DirectionTo(npc.Center).UnNaN() * speed;
                }
            }
        }

        public static int FixedDamage(this NPC npc) {
            return Main.masterMode ? npc.damage / 3 : Main.expertMode ? npc.damage / 2 : npc.damage;
        }

        public static int ToBannerItem(this NPC npc) {
            return Item.BannerToItem(npc.ToBanner());
        }
        public static int ToBanner(this NPC npc) {
            return Item.NPCtoBanner(npc.BannerID());
        }

        public static bool IsProbablyACritter(this NPC npc) {
            return NPCID.Sets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1);
        }

        public static bool IsTheDestroyer(this NPC npc) {
            return npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail;
        }

        public static NPC SudoClone(NPC npc) {
            var npc2 = new NPC();
            npc2.SetDefaults(npc.type);
            for (int i = 0; i < npc.ai.Length; i++) {
                npc2.ai[i] = npc.ai[i];
            }
            for (int i = 0; i < npc.localAI.Length; i++) {
                npc2.localAI[i] = npc.localAI[i];
            }
            npc2.width = npc.width;
            npc2.height = npc.height;
            npc2.scale = npc.scale;
            npc2.frame = npc.frame;
            npc2.direction = npc.direction;
            npc2.spriteDirection = npc.spriteDirection;
            npc2.velocity = npc.velocity;
            npc2.rotation = npc.rotation;
            npc2.gfxOffY = npc.gfxOffY;

            var oldSlot = Main.npc[npc.whoAmI];
            try {
                npc2.position = npc.position;
                Main.npc[npc.whoAmI] = npc2;
                npc2.AI();
                Main.npc[npc.whoAmI] = oldSlot;
                npc2.position = npc.position;
            }
            catch {
                Main.npc[npc.whoAmI] = oldSlot;
            }
            return npc2;
        }

        public static void SyncNPC(NPC npc) {
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, Main.myPlayer, -1, null, npc.whoAmI);
        }

        /// <returns>The index of the player. -1 if none are found.</returns>
        public static int FindPlayerWithin(NPC npc) {
            return FindPlayerWithin(Utils.CenteredRectangle(npc.Center, new Vector2(2000f, 1200f)));
        }

        public static void ClearAI(this NPC npc, bool localAI = true) {
            int amt = Math.Min(npc.ai.Length, NPC.maxAI);
            for (int i = 0; i < amt; i++) {
                npc.ai[i] = 0;
            }

            if (!localAI)
                return;

            amt = Math.Min(npc.localAI.Length, NPC.maxAI);
            for (int i = 0; i < amt; i++) {
                npc.localAI[i] = 0;
            }
        }

        #region Town NPCs
        public static Point Home(this NPC npc) {
            return new Point(npc.homeTileX, npc.homeTileY);
        }
        #endregion

        #region Lazy
        public static IEntitySource GetSource_HitEffect(this NPC npc) {
            return new EntitySource_Parent(npc);
        }

        [Obsolete("Do not use.")]
        public static ref float StatSpeed(this NPC npc) {
            return ref npc.GetGlobalNPC<StatSpeedGlobalNPC>().statSpeed;
        }

        public static T ModNPC<T>(this NPC npc) where T : ModNPC {
            return (T)npc.ModNPC;
        }

        public static bool TryGetAequus(this NPC npc, out AequusNPC aequusNPC) {
            return npc.TryGetGlobalNPC<AequusNPC>(out aequusNPC);
        }
        public static AequusNPC Aequus(this NPC npc) {
            return npc.GetGlobalNPC<AequusNPC>();
        }
        #endregion
    }
}
using Aequus.Buffs;
using Aequus.Content.Necromancy;
using Aequus.NPCs;
using Aequus.NPCs.GlobalNPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus
{
    public static partial class Helper
    {
        public static void SetIDStaticHitCooldown(this NPC npc, int projID, uint time)
        {
            Projectile.perIDStaticNPCImmunity[projID][npc.whoAmI] = Main.GameUpdateCount + time;
        }
        public static void SetIDStaticHitCooldown<T>(this NPC npc, uint time) where T : ModProjectile
        {
            SetIDStaticHitCooldown(npc, ModContent.ProjectileType<T>(), time);
        }

        #region Buffs
        public static void CleanupAndSyncBuffs(this NPC npc)
        {
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] == 0 || npc.buffType[i] == 0)
                {
                    for (int j = i + 1; j < NPC.maxBuffs; j++)
                    {
                        npc.buffTime[j - 1] = npc.buffTime[j];
                        npc.buffType[j - 1] = npc.buffType[j];
                        npc.buffTime[j] = 0;
                        npc.buffType[j] = 0;
                    }
                }
            }
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SendNPCBuffs, -1, -1, null, npc.whoAmI);
            }
        }
        public static void ClearAllDebuffs(this NPC npc)
        {
            bool needsSync = false;
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] > 0 && npc.buffType[i] > 0 && (Main.debuff[npc.buffType[i]] || AequusBuff.IsFire.Contains(npc.buffType[i])))
                {
                    npc.buffTime[i] = 0;
                    npc.buffType[i] = 0;
                    needsSync = true;
                }
            }
            if (needsSync)
                CleanupAndSyncBuffs(npc);
        }
        public static void ClearBuffs(this NPC npc, IEnumerable<int> type)
        {
            bool needsSync = false;
            foreach (var buffType in type)
            {
                int index = npc.FindBuffIndex(buffType);
                if (index != -1)
                {
                    npc.buffTime[index] = 0;
                    npc.buffType[index] = 0;
                    needsSync = true;
                }
            }
            if (needsSync)
                CleanupAndSyncBuffs(npc);
        }
        public static void ClearBuff(this NPC npc, int type)
        {
            int index = npc.FindBuffIndex(type);
            if (index != -1)
            {
                npc.DelBuff(type);
            }
        }

        public static void AddBuffToHeadOrSelf(this NPC npc, int buffID, int buffDuration)
        {
            if (npc.realLife != -1)
            {
                Main.npc[npc.realLife].AddBuff(buffID, buffDuration);
                return;
            }

            npc.AddBuff(buffID, buffDuration);
        }
        #endregion

        public static void Kill(this NPC npc, bool quiet = false)
        {
            npc.life = 1;
            npc.StrikeNPC(npc.lifeMax, 0f, 0);
            npc.active = false;
            if (Main.netMode != NetmodeID.SinglePlayer && !quiet)
            {
                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, npc.lifeMax);
            }
        }
        public static void KillEffects(this NPC npc, bool quiet = false)
        {
            npc.life = Math.Min(npc.life, -1);
            npc.HitEffect();
            npc.active = false;
            if (Main.netMode != NetmodeID.SinglePlayer && !quiet)
            {
                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, 9999 + npc.lifeMax * 2 + npc.defense * 2);
            }
        }

        public static void HideBestiaryEntry(this ModNPC npc)
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(npc.Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true, });
        }

        public static void CollideWithOthers(this NPC npc, float speed = 0.05f)
        {
            var rect = npc.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && i != npc.whoAmI && npc.type == Main.npc[i].type
                    && rect.Intersects(Main.npc[i].getRect()))
                {
                    npc.velocity += Main.npc[i].DirectionTo(npc.Center).UnNaN() * speed;
                }
            }
        }

        public static int FixedDamage(this NPC npc)
        {
            return Main.masterMode ? npc.damage / 3 : Main.expertMode ? npc.damage / 2 : npc.damage;
        }

        public static int ToBannerItem(this NPC npc)
        {
            return Item.BannerToItem(npc.ToBanner());
        }
        public static int ToBanner(this NPC npc)
        {
            return Item.NPCtoBanner(npc.BannerID());
        }

        public static void AddRegen(this NPC npc, int regen)
        {
            if (regen < 0 && npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen += regen;
        }

        public static bool IsProbablyACritter(this NPC npc)
        {
            return NPCID.Sets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1);
        }

        public static bool IsTheDestroyer(this NPC npc)
        {
            return npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail;
        }

        public static NPC SudoClone(NPC npc)
        {
            var npc2 = new NPC();
            npc2.SetDefaults(npc.type);
            for (int i = 0; i < npc.ai.Length; i++)
            {
                npc2.ai[i] = npc.ai[i];
            }
            for (int i = 0; i < npc.localAI.Length; i++)
            {
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
            try
            {
                npc2.position = npc.position;
                Main.npc[npc.whoAmI] = npc2;
                npc2.AI();
                Main.npc[npc.whoAmI] = oldSlot;
                npc2.position = npc.position;
            }
            catch
            {
                Main.npc[npc.whoAmI] = oldSlot;
            }
            return npc2;
        }

        public static void SyncNPC(NPC npc)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, Main.myPlayer, -1, null, npc.whoAmI);
        }

        public static int FindFirstPlayerWithin(NPC npc)
        {
            return FindFirstPlayerWithin(Utils.CenteredRectangle(npc.Center, new Vector2(2000f, 1200f)));
        }

        public static void ClearAI(this NPC npc, bool localAI = true)
        {
            int amt = Math.Min(npc.ai.Length, NPC.maxAI);
            for (int i = 0; i < amt; i++)
            {
                npc.ai[i] = 0;
            }

            if (!localAI)
                return;

            amt = Math.Min(npc.localAI.Length, NPC.maxAI);
            for (int i = 0; i < amt; i++)
            {
                npc.localAI[i] = 0;
            }
        }

        [Obsolete("Liquid movement speed fields are now public.")]
        public static void SetLiquidSpeeds(this NPC npc, float water = 0.5f, float lava = 0.5f, float honey = 0.25f)
        {
            //npc.waterMovementSpeed = water;
            //npc.lavaMovementSpeed = water;
            //npc.honeyMovementSpeed = water;
        }

        public static Vector2 GetSpeedStats(this NPC npc)
        {
            var velocityBoost = new Vector2(npc.StatSpeed());
            if (!npc.noGravity)
            {
                velocityBoost.Y = MathHelper.Lerp(1f, velocityBoost.Y, npc.GetGlobalNPC<StatSpeedGlobalNPC>().jumpSpeedInterpolation);
            }
            return velocityBoost;
        }

        #region Town NPCs
        public static Point Home(this NPC npc)
        {
            return new Point(npc.homeTileX, npc.homeTileY);
        }
        #endregion

        #region Necromancy
        public static bool IsZombieAndInteractible(this NPC npc, int plr)
        {
            return npc.active && (npc.realLife == -1 || npc.realLife == npc.whoAmI) && !NPCID.Sets.ProjectileNPC[npc.type] &&
                    npc.TryGetGlobalNPC<NecromancyNPC>(out var n) && n.isZombie && n.slotsConsumed > 0 && n.zombieOwner == plr;
        }
        #endregion

        #region Lazy
        public static ref float StatSpeed(this NPC npc)
        {
            return ref npc.GetGlobalNPC<StatSpeedGlobalNPC>().statSpeed;
        }

        public static T ModNPC<T>(this NPC npc) where T : ModNPC
        {
            return (T)npc.ModNPC;
        }

        public static AequusNPC Aequus(this NPC npc)
        {
            return npc.GetGlobalNPC<AequusNPC>();
        }
        #endregion
    }
}
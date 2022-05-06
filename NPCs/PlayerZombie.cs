using Aequus.Buffs.Debuffs;
using Aequus.Common.Catalogues;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public class PlayerZombie : GlobalNPC
    {
        public static bool AI_NPCIsZombie { get; private set; }
        public static int AI_ZombieOwner { get; private set; }
        public static int AI_NPCTarget { get; private set; }
        public static Vector2 AI_ReturnPlayerLocation { get; private set; }

        public int zombieDrain;
        public bool isZombie;
        public int zombieOwner;
        public int zombieTimer;
        public float zombieDebuffTier;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            On.Terraria.NPC.SetTargetTrackingValues += NPC_SetTargetTrackingValues;
        }

        private void NPC_SetTargetTrackingValues(On.Terraria.NPC.orig_SetTargetTrackingValues orig, NPC self, bool faceTarget, float realDist, int tankTarget)
        {
            if (AI_NPCIsZombie)
            {
                self.target = self.GetGlobalNPC<PlayerZombie>().zombieOwner;
                if (AI_NPCTarget != -1)
                {
                    self.targetRect = Main.npc[AI_NPCTarget].getRect();
                }
                else
                {
                    self.targetRect = Main.player[self.target].getRect();
                }

                if (faceTarget)
                {
                    self.direction = self.targetRect.X + self.targetRect.Width / 2 < self.position.X + self.width / 2 ? -1 : 1;
                    self.directionY = self.targetRect.Y + self.targetRect.Height / 2 < self.position.Y + self.height / 2 ? -1 : 1;
                }

                return;
            }
            orig(self, faceTarget, realDist, tankTarget);
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!AI_NPCIsZombie)
            {
                return;
            }
            if (source is EntitySource_OnHit onHit)
            {
                ZombieCheck(onHit.EntityStruck, npc);
            }
            else if (source is EntitySource_Parent parent)
            {
                ZombieCheck(parent.Entity, npc);
            }
            else if (source is EntitySource_HitEffect hitEffect)
            {
                ZombieCheck(hitEffect.Entity, npc);
            }
            else if (source is EntitySource_Death death)
            {
                ZombieCheck(death.Entity, npc);
            }
        }
        public void ZombieCheck(Entity entity, NPC npc)
        {
            if (entity is NPC npc2 && npc2.GetGlobalNPC<PlayerZombie>().isZombie)
            {
                npc.friendly = true;
                npc.damage *= 5;
                npc.GetGlobalNPC<PlayerZombie>().zombieOwner = npc2.GetGlobalNPC<PlayerZombie>().zombieOwner;
                npc.GetGlobalNPC<PlayerZombie>().zombieTimer = npc2.GetGlobalNPC<PlayerZombie>().zombieTimer;
                npc.GetGlobalNPC<PlayerZombie>().zombieDebuffTier = npc2.GetGlobalNPC<PlayerZombie>().zombieDebuffTier;
                npc.GetGlobalNPC<PlayerZombie>().isZombie = true;
            }
        }

        public override bool? CanHitNPC(NPC npc, NPC target)
        {
            return isZombie ? (target.CanBeChasedBy(npc) ? true : null) : null;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (isZombie)
            {
                float wave = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
                drawColor.A = (byte)MathHelper.Clamp(drawColor.R - 100, byte.MinValue, byte.MaxValue);
                drawColor.G = (byte)MathHelper.Clamp(drawColor.G - (50 + (int)(Math.Max(0f, wave) * 10f)), drawColor.R, byte.MaxValue);
                drawColor.B = (byte)MathHelper.Clamp(drawColor.B + 100, drawColor.G, byte.MaxValue);
                drawColor.A = (byte)MathHelper.Clamp(drawColor.A + wave * 50f, byte.MinValue, byte.MaxValue - 60);
                return drawColor;
            }
            return null;
        }

        public override void ResetEffects(NPC npc)
        {
            if (zombieDrain > 0)
            {
                zombieDrain--;
            }
            if (isZombie)
            {
                npc.DelBuff(0);
            }
        }

        public override bool PreAI(NPC npc)
        {
            AI_NPCIsZombie = isZombie;
            AI_NPCTarget = -1;
            if (isZombie)
            {
                if (zombieTimer == 0)
                {
                    zombieTimer = 3600;
                }
                zombieTimer--;

                if (ShouldDespawnZombie(npc))
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false;
                }

                if (AI_ReturnPlayerLocation != Vector2.Zero)
                {
                    Main.player[zombieOwner].position = AI_ReturnPlayerLocation;
                    AI_ReturnPlayerLocation = Vector2.Zero;
                }

                AI_ZombieOwner = zombieOwner;

                npc.life = npc.lifeMax - 1; // Aggros slimes and stuff
                npc.GivenName = Main.player[zombieOwner].name + "'s " + Lang.GetNPCName(npc.netID);
                npc.friendly = true;
                npc.target = zombieOwner;
                npc.alpha = Math.Max(npc.alpha, 60);
                npc.dontTakeDamage = true;
                npc.npcSlots = 0f;
                int npcTarget = GetNPCTarget(npc);

                if (npcTarget != -1)
                {
                    AI_ReturnPlayerLocation = Main.player[zombieOwner].position;
                    AI_NPCTarget = npcTarget;
                    Main.player[zombieOwner].Center = Main.npc[npcTarget].Center;

                    try
                    {
                        ZombieHurtNPCsCheck(npc);
                    }
                    catch
                    {

                    }
                }
            }
            return true;
        }
        public bool ShouldDespawnZombie(NPC npc)
        {
            return zombieTimer <= 0 || !Main.player[zombieOwner].active || Main.player[zombieOwner].dead;
        }
        private int GetNPCTarget(NPC npc)
        {
            int target = -1;
            float distance = NecromancyTypes.NPCs.GetOrDefault(npc.type, NecromancyTypes.NecroStats.None).ViewDistance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(npc) &&
                    !NPCID.Sets.CountsAsCritter[Main.npc[i].type])
                {
                    float c = npc.Distance(Main.npc[i].Center);
                    if (c < distance)
                    {
                        target = i;
                        distance = c;
                    }
                }
            }
            return target;
        }
        public void ZombieHurtNPCsCheck(NPC npc)
        {
            var myRect = npc.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var n = Main.npc[i];
                if (n.dontTakeDamage || n.dontTakeDamageFromHostiles || n.immortal || n.immune[255] > 0)
                {
                    continue;
                }

                bool? modCanHit = NPCLoader.CanHitNPC(npc, Main.npc[i]);
                if ((!modCanHit.HasValue || modCanHit.Value) && myRect.Intersects(n.getRect()) && npc.type != NPCID.Gnome)
                {
                    ZombieHurtNPC(npc, n);
                }
            }
        }
        public void ZombieHurtNPC(NPC npc, NPC target)
        {
            int immuneTime = 30;
            //if (target.type == NPCID.DD2EterniaCrystal)
            //{
            //    immuneTime = 20;
            //}
            int damage = Main.DamageVar(GetNPCDamage(npc, target));
            float knockBack = 6f;
            int hitDirection = ((!(npc.Center.X > target.Center.X)) ? 1 : (-1));
            bool crit = false;
            NPCLoader.ModifyHitNPC(npc, target, ref damage, ref knockBack, ref crit);
            Main.player[npc.GetGlobalNPC<PlayerZombie>().zombieOwner].ApplyDamageToNPC(target, damage, knockBack, hitDirection, crit);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, target.whoAmI, damage, knockBack, hitDirection);
            }
            target.netUpdate = true;
            target.immune[255] = immuneTime;
            NPCLoader.OnHitNPC(npc, target, damage, knockBack, crit);
        }
        public int GetNPCDamage(NPC npc, NPC target)
        {
            double damage = ContentSamples.NpcsByNetId[npc.netID].damage;
            if (Main.masterMode)
            {
                damage /= 3f;
            }
            else if (Main.expertMode)
            {
                damage /= 2f;
            }
            return (int)damage;
        }

        public override void PostAI(NPC npc)
        {
            if (isZombie)
            {
                npc.dontTakeDamage = true;
                if (AI_ReturnPlayerLocation != Vector2.Zero)
                {
                    Main.player[zombieOwner].position = AI_ReturnPlayerLocation;
                    AI_ReturnPlayerLocation = Vector2.Zero;
                }
                if (Main.rand.NextBool(6))
                {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), newColor: new Color(50, 150, 255, 100));
                    d.velocity *= 0.3f;
                    d.velocity += npc.velocity * 0.2f;
                    d.scale *= npc.scale;
                    d.noGravity = true;
                }
            }
            AI_NPCIsZombie = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (zombieDrain > 0)
            {
                int dot = zombieDrain / AequusHelpers.NPCREGEN;
                npc.AddRegen(-dot);
                if (damage < dot)
                    damage = dot;
            }
        }

        public override void OnKill(NPC npc)
        {
            if (zombieDrain > 0 && CanBeTurnedIntoZombie(npc))
            {
                int n = NPC.NewNPC(npc.GetSource_Death("Aequus:Zombie"), (int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height / 2, npc.netID, npc.whoAmI + 1);
                Main.npc[n].GetGlobalNPC<PlayerZombie>().isZombie = true;
                Main.npc[n].damage *= GetDamageMultiplier(npc);
                Main.npc[n].friendly = true;
                Main.npc[n].extraValue = 0;
            }
        }   
        public int GetDamageMultiplier(NPC npc)
        {
            int dmgMultiplier = 1;
            if (npc.boss)
            {
                dmgMultiplier = 5;
            }
            dmgMultiplier += npc.lifeMax / 2500;
            return dmgMultiplier;
        }
        public bool CanBeTurnedIntoZombie(NPC npc)
        {
            if (npc.type == NPCID.DungeonGuardian)
            {
                return false;
            }
            return NecromancyTypes.NPCs.GetOrDefault(npc.type, NecromancyTypes.NecroStats.None).PowerNeeded <= zombieDebuffTier;
        }

        internal static void SetupBuffImmunities()
        {
            var buffList = new List<int>(NecromancyTypes.NecromancyDebuffs);
            buffList.Remove(ModContent.BuffType<EnthrallingDebuff>());
            for (int i = 0; i < Main.maxNPCTypes; i++)
            {
                if (!NecromancyTypes.NPCs.TryGetValue(i, out var stats) || stats == NecromancyTypes.NecroStats.None)
                {
                    if (!NPCID.Sets.DebuffImmunitySets.TryGetValue(i, out var value))
                    {
                        NPCID.Sets.DebuffImmunitySets.Add(i, new NPCDebuffImmunityData() { SpecificallyImmuneTo = buffList.ToArray() });
                        continue;
                    }
                    if (value == null)
                    {
                        value = NPCID.Sets.DebuffImmunitySets[i] = new NPCDebuffImmunityData();
                    }
                    if (value.SpecificallyImmuneTo == null)
                    {
                        value.SpecificallyImmuneTo = buffList.ToArray();
                        continue;
                    }
                    Array.Resize(ref value.SpecificallyImmuneTo, value.SpecificallyImmuneTo.Length + buffList.Count);
                    int k = 0;
                    for (int j = value.SpecificallyImmuneTo.Length - buffList.Count; j < value.SpecificallyImmuneTo.Length; j++)
                    {
                        value.SpecificallyImmuneTo[j] = buffList[k];
                        k++;
                    }
                }
            }
        }
    }
    public class PlayerZombieProj : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!PlayerZombie.AI_NPCIsZombie)
            {
                return;
            }
            if (source is EntitySource_OnHit onHit)
            {
                ZombieCheck(onHit.EntityStruck, projectile);
            }
            else if (source is EntitySource_Parent parent)
            {
                ZombieCheck(parent.Entity, projectile);
            }
            else if (source is EntitySource_HitEffect hitEffect)
            {
                ZombieCheck(hitEffect.Entity, projectile);
            }
            else if (source is EntitySource_Death death)
            {
                ZombieCheck(death.Entity, projectile);
            }
        }
        public void ZombieCheck(Entity entity, Projectile projectile)
        {
            if (entity is NPC npc && npc.GetGlobalNPC<PlayerZombie>().isZombie)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.owner = PlayerZombie.AI_ZombieOwner;
            }
        }
    }
}
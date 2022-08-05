using Aequus.Buffs.Debuffs;
using Aequus.Graphics;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Necromancy
{
    public class NecromancyProj : GlobalProjectile
    {
        public static HashSet<int> BlacklistExtraUpdates { get; private set; }

        public bool isZombie;
        public int zombieNPCOwner;
        public float zombieDebuffTier;
        public int renderLayer;
        public bool appliedEffects;

        public override bool InstancePerEntity => true;

        public override void Load()
        {
            BlacklistExtraUpdates = new HashSet<int>()
            {
                ProjectileID.SandnadoHostile,
            };
            On.Terraria.Projectile.Kill += Projectile_Kill;
        }

        private static void Projectile_Kill(On.Terraria.Projectile.orig_Kill orig, Projectile self)
        {
            if (!self.TryGetGlobalProjectile<NecromancyProj>(out var zombie) || !zombie.isZombie)
            {
                orig(self);
                return;
            }
            NecromancyNPC.Zombie.PlayerOwner = self.owner;
            try
            {
                orig(self);
            }
            catch
            {
            }
            NecromancyNPC.Zombie.Reset();
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
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
            else if (source is EntitySource_ItemUse itemUse)
            {
                ZombieCheck(itemUse.Entity, projectile);
            }
        }
        public void ZombieCheck(Entity entity, Projectile projectile)
        {
            if (entity is Projectile proj && proj.GetGlobalProjectile<NecromancyProj>().isZombie)
            {
                ZombifyChild(projectile, proj.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner, proj.GetGlobalProjectile<NecromancyProj>().zombieDebuffTier, proj.timeLeft + 10, proj.GetGlobalProjectile<NecromancyProj>().renderLayer);
            }
            else if (entity is NPC npc && npc.GetGlobalNPC<NecromancyNPC>().isZombie)
            {
                ZombifyChild(projectile, entity.whoAmI, npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier, npc.GetGlobalNPC<NecromancyNPC>().zombieTimer, npc.GetGlobalNPC<NecromancyNPC>().renderLayer);
            }
        }
        public void ZombifyChild(Projectile projectile, int npc, float tier, int timeLeft, int renderLayer)
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.owner = Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner;
            projectile.DamageType = NecromancyDamageClass.Instance;
            if (!projectile.usesLocalNPCImmunity)
            {
                projectile.usesIDStaticNPCImmunity = true;
                projectile.idStaticNPCHitCooldown = 10;
            }
            isZombie = true;
            zombieNPCOwner = npc;
            zombieDebuffTier = tier;
            this.renderLayer = renderLayer;
            projectile.timeLeft = Math.Max(Math.Min(projectile.timeLeft, timeLeft - 10), 2);
        }

        public override Color? GetAlpha(Projectile projectile, Color drawColor)
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

        public override bool PreAI(Projectile projectile)
        {
            NecromancyNPC.Zombie.Reset();
            if (isZombie)
            {
                var aequus = Main.player[projectile.owner].Aequus();
                if (!appliedEffects)
                {
                    if (aequus.ghostProjExtraUpdates > 0)
                    {
                        if (!BlacklistExtraUpdates.Contains(projectile.type))
                            projectile.extraUpdates = (projectile.extraUpdates + 1) * (aequus.ghostProjExtraUpdates + 1) - 1;
                    }
                    appliedEffects = true;
                }
                if (!Main.npc[zombieNPCOwner].active)
                {
                    projectile.Kill();
                    return true;
                }

                NecromancyNPC.Zombie.PlayerOwner = projectile.owner;
                NecromancyNPC.RestoreTarget();

                projectile.hostile = false;
                projectile.friendly = true;
                int npcTarget = NecromancyNPC.GetNPCTarget(projectile, Main.player[projectile.owner], Main.npc[zombieNPCOwner].netID, Main.npc[zombieNPCOwner].type);

                if (npcTarget != -1)
                {
                    NecromancyNPC.TargetHack = new PlayerTargetHack(Main.npc[zombieNPCOwner], Main.npc[npcTarget], Main.player[projectile.owner], Main.npc[npcTarget].Center);
                    NecromancyNPC.TargetHack.Move();
                    NecromancyNPC.Zombie.NPCTarget = npcTarget;
                }

                SpecialProjecitleAI(projectile);
            }
            return true;
        }
        public void SpecialProjecitleAI(Projectile projectile)
        {
        }

        public override void PostAI(Projectile projectile)
        {
            if (isZombie)
            {
                NecromancyNPC.RestoreTarget();
                if (Main.rand.NextBool(6))
                {
                    int index = GhostOutlineRenderer.GetScreenTargetIndex(Main.player[Main.npc[zombieNPCOwner].GetGlobalNPC<NecromancyNPC>().zombieOwner], renderLayer);
                    var color = new Color(50, 150, 255, 100);
                    if (GhostOutlineRenderer.necromancyRenderers.Length > index && GhostOutlineRenderer.necromancyRenderers[index] != null)
                    {
                        color = GhostOutlineRenderer.necromancyRenderers[index].DrawColor();
                        color.A = 100;
                    }

                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), newColor: color);
                    d.velocity *= 0.3f;
                    d.velocity += projectile.velocity * 0.2f;
                    d.scale *= projectile.scale;
                    d.noGravity = true;
                }
            }
            NecromancyNPC.Zombie.Reset();
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (isZombie)
            {
                float multiplier = NecromancyNPC.GetDamageMultiplier(Main.npc[zombieNPCOwner], projectile.damage);
                if (Main.masterMode)
                {
                    multiplier *= 3f;
                }
                else if (Main.expertMode)
                {
                    multiplier *= 2f;
                }
                damage = (int)(damage * multiplier);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (isZombie)
            {
                target.AddBuff(ModContent.BuffType<SoulStolen>(), 300);
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter writer)
        {
            bitWriter.WriteBit(isZombie);
            if (isZombie)
            {
                writer.Write(zombieNPCOwner);
                writer.Write(zombieDebuffTier);
                writer.Write((byte)renderLayer);
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader reader)
        {
            if (bitReader.ReadBit())
            {
                isZombie = true;
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.DamageType = NecromancyDamageClass.Instance;
                if (!projectile.usesLocalNPCImmunity)
                {
                    projectile.usesIDStaticNPCImmunity = true;
                    projectile.idStaticNPCHitCooldown = 10;
                }
                zombieNPCOwner = reader.ReadInt32();
                zombieDebuffTier = reader.ReadSingle();
                renderLayer = reader.ReadByte();
            }
        }
    }
}
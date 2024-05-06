using Aequus.Content.Necromancy.Rendering;
using Aequus.Old.Content.Particles;
using Aequus.Particles.Dusts;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Necromancy;

public class NecromancyProj : GlobalProjectile {
    public static HashSet<int> BlacklistExtraUpdates { get; private set; }

    public bool isZombie;
    public int zombieNPCOwner;
    public float zombieDebuffTier;
    public int renderLayer;
    public bool appliedEffects;

    public override bool InstancePerEntity => true;

    public override void Load() {
        BlacklistExtraUpdates = new HashSet<int>()
        {
                ProjectileID.SandnadoHostile,
            };
        Terraria.On_Projectile.Kill += Projectile_Kill;
    }

    private static void Projectile_Kill(Terraria.On_Projectile.orig_Kill orig, Projectile self) {
        if (!self.TryGetGlobalProjectile<NecromancyProj>(out var zombie) || !zombie.isZombie) {
            orig(self);
            return;
        }
        NecromancyNPC.Zombie.PlayerOwner = self.owner;
        try {
            orig(self);
        }
        catch {
        }
        NecromancyNPC.Zombie.Reset();
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        if (source is EntitySource_Parent parent) {
            ZombieCheck(parent.Entity, projectile);
        }
    }

    public void ZombieCheck(Entity entity, Projectile projectile) {
        if (entity is Projectile proj && proj.GetGlobalProjectile<NecromancyProj>().isZombie) {
            ZombifyChild(projectile, proj.GetGlobalProjectile<NecromancyProj>().zombieNPCOwner, proj.GetGlobalProjectile<NecromancyProj>().zombieDebuffTier, proj.timeLeft + 10, proj.GetGlobalProjectile<NecromancyProj>().renderLayer);
        }
        else if (entity is NPC npc && npc.GetGlobalNPC<NecromancyNPC>().isZombie) {
            ZombifyChild(projectile, entity.whoAmI, npc.GetGlobalNPC<NecromancyNPC>().zombieDebuffTier, npc.GetGlobalNPC<NecromancyNPC>().zombieTimer, npc.GetGlobalNPC<NecromancyNPC>().renderLayer);
        }
    }

    public void ZombifyChild(Projectile projectile, int npc, float tier, int timeLeft, int renderLayer) {
        projectile.hostile = false;
        projectile.friendly = true;
        var zombie = Main.npc[npc].GetGlobalNPC<NecromancyNPC>();
        projectile.owner = zombie.zombieOwner;
        if (zombie.ghostDamage > 0) {
            projectile.damage = zombie.ghostDamage;
        }

        projectile.DamageType = Aequus.NecromancyClass;
        if (!projectile.usesLocalNPCImmunity) {
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }
        isZombie = true;
        zombieNPCOwner = npc;
        zombieDebuffTier = tier;
        this.renderLayer = renderLayer;
        projectile.timeLeft = Math.Max(Math.Min(projectile.timeLeft, timeLeft - 10), 2);
    }

    public override Color? GetAlpha(Projectile projectile, Color drawColor) {
        if (isZombie) {
            var color = GhostRenderer.GetColorTarget(Main.player[projectile.owner], renderLayer).getDrawColor() with { A = 100 };

            float wave = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f);
            color *= (wave + 1f) / 2f * 0.5f;
            drawColor.A = (byte)MathHelper.Clamp(drawColor.R + color.R, byte.MinValue, byte.MaxValue);
            drawColor.G = (byte)MathHelper.Clamp(drawColor.G + color.G, byte.MinValue, byte.MaxValue);
            drawColor.B = (byte)MathHelper.Clamp(drawColor.B + color.B, byte.MinValue, byte.MaxValue);
            drawColor.A = (byte)MathHelper.Clamp(drawColor.A + wave * 50f, byte.MinValue, byte.MaxValue - 60);
            return drawColor;
        }
        return null;
    }

    public override bool PreAI(Projectile projectile) {
        NecromancyNPC.Zombie.Reset();
        if (isZombie) {
            var aequus = Main.player[projectile.owner].GetModPlayer<AequusPlayer>();
            if (!appliedEffects) {
                if (aequus.ghostProjExtraUpdates > 0) {
                    if (!BlacklistExtraUpdates.Contains(projectile.type))
                        projectile.extraUpdates = (projectile.extraUpdates + 1) * (aequus.ghostProjExtraUpdates + 1) - 1;
                }
                appliedEffects = true;
            }
            if (!Main.npc[zombieNPCOwner].active) {
                projectile.Kill();
                return true;
            }

            NecromancyNPC.Zombie.PlayerOwner = projectile.owner;
            NecromancyNPC.RestoreTarget();

            projectile.hostile = false;
            projectile.friendly = true;
            int npcTarget = NecromancyNPC.GetNPCTarget(projectile, Main.player[projectile.owner], Main.npc[zombieNPCOwner].netID, Main.npc[zombieNPCOwner].type);

            if (npcTarget != -1) {
                NecromancyNPC.TargetHack = new PlayerTargetHack(Main.npc[zombieNPCOwner], Main.npc[npcTarget], Main.player[projectile.owner], Main.npc[npcTarget].Center);
                NecromancyNPC.TargetHack.Move();
                NecromancyNPC.Zombie.NPCTarget = npcTarget;
            }

            SpecialProjecitleAI(projectile);
        }
        return true;
    }
    public void SpecialProjecitleAI(Projectile projectile) {
    }

    public override void PostAI(Projectile projectile) {
        if (isZombie && Main.netMode != NetmodeID.Server) {
            NecromancyNPC.RestoreTarget();
            if (Main.rand.NextBool(6)) {
                var color = GhostRenderer.GetColorTarget(Main.player[projectile.owner], renderLayer).getDrawColor() with { A = 100 };
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), newColor: color);
                d.velocity *= 0.3f;
                d.velocity += projectile.velocity * 0.2f;
                d.scale *= projectile.scale;
                d.noGravity = true;
            }
        }
        NecromancyNPC.Zombie.Reset();
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter writer) {
        bitWriter.WriteBit(isZombie);
        if (isZombie) {
            writer.Write(zombieNPCOwner);
            writer.Write(zombieDebuffTier);
            writer.Write((byte)renderLayer);
        }
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader reader) {
        if (bitReader.ReadBit()) {
            isZombie = true;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.DamageType = Aequus.NecromancyClass;
            if (!projectile.usesLocalNPCImmunity) {
                projectile.usesIDStaticNPCImmunity = true;
                projectile.idStaticNPCHitCooldown = 10;
            }
            zombieNPCOwner = reader.ReadInt32();
            zombieDebuffTier = reader.ReadSingle();
            renderLayer = reader.ReadByte();
        }
    }

    public static bool AnyOwnedByNPC(NPC npc, int proj) {
        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && Main.projectile[i].friendly && Main.projectile[i].type == proj && Main.projectile[i].TryGetGlobalProjectile<NecromancyProj>(out var zombie)) {
                if (zombie.isZombie && zombie.zombieNPCOwner == npc.whoAmI) {
                    return true;
                }
            }
        }
        return false;
    }
}
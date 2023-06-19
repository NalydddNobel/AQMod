using Aequus.Buffs.Debuffs;
using Aequus.Common.Graphics;
using Aequus.Content;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic;

public class WindFanProj : ModProjectile
{
    // the array of pain and suffering
    private readonly int[] hitCooldown = new int[Main.maxNPCs];
    public readonly List<int> hitEnemies = new List<int>();

    public override string Texture => Aequus.BlankTexture;

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 250;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft *= 5;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.aiStyle = -1;
        hitEnemies.Clear();
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (Projectile.numUpdates == -1)
            Projectile.ai[0]++;
        if (Projectile.numUpdates == -1 && Projectile.ai[0] % 20 == 0)
        {
            if (!player.CheckMana(player.HeldItemFixed(), pay: true))
            {
                Projectile.timeLeft = 1;
                return;
            }
        }
        if (player.active && !player.dead && player.channel && !player.CCed && !player.noItems)
        {
            Projectile.timeLeft = 2;
            player.itemAnimation = player.itemTime = 2;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = Main.MouseWorld;
                Projectile.rotation = player.DirectionTo(Projectile.Center).ToRotation();
                Projectile.netUpdate = true; // Syncs the projectile onto every other client
            }
            player.direction = Projectile.Center.X > player.Center.X ? 1 : -1;
            player.itemRotation = Projectile.rotation + player.direction == -1 ? MathHelper.Pi : 0;
        }
        else
        {
            return;
        }

        hitEnemies.Clear();
        // d i s t a n c e
        (int npc, float dist)[] targets = new (int, float)[3];
        int targetCount = 0;
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC npc = Main.npc[i];
            if (hitCooldown[i] > 0)
            {
                hitCooldown[i]--;
            }
            if (PushableEntities.NPCIDs.Contains(npc.type) && npc.CanBeChasedBy(Projectile) && npc.Distance(Projectile.Center) < 200 && (npc.noTileCollide || Collision.CanHit(Projectile.position, 1, 1, npc.position, 1, 1)))
            {
                if (targetCount < 3)
                {
                    targets[targetCount] = (i, npc.Distance(Projectile.Center));
                    targetCount++;
                }
                else
                {
                    float lowest = targets[0].dist;
                    int lowestIndex = 0;
                    for (int j = 1; j < 3; j++)
                    {
                        if (targets[j].dist < lowest)
                        {
                            lowest = targets[j].dist;
                            lowestIndex = j;
                        }
                    }
                    if (npc.Distance(Projectile.Center) < lowest)
                    {
                        targets[lowestIndex] = (i, npc.Distance(Projectile.Center));
                    }
                }
            }
        }

        // literally just same particles as the cloak but slightly modified
        Vector2 widthMethod(float p) => new Vector2(7f) * (float)Math.Sin(p * MathHelper.Pi);
        Color colorMethod(float p) => Color.White.UseA(150) * 0.45f * (float)Math.Sin(p * MathHelper.Pi);
        for (int i = 0; i < targetCount + 1; i++)
        {
            var v = Main.rand.NextVector2Unit();
            Dust.NewDustPerfect(Projectile.Center + v * Main.rand.NextFloat(40, 200), ModContent.DustType<MonoDust>(), v.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(8f), newColor: new Color(128, 128, 128, 0));
            if (Main.rand.NextBool(3))
            {
                var prim = new TrailRenderer(TrailTextures.Trail[4].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                float rotation = 0.3f;
                var particle = ParticleSystem.New<StormcloakTrailParticle>(ParticleLayer.BehindPlayers).Setup(prim, Projectile.Center + v * Main.rand.NextFloat(35, 190), v.RotatedBy(MathHelper.PiOver2) * 10f,
                    scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10);
                particle.StretchTrail(v.RotatedBy(-MathHelper.PiOver2) * 2f);
                particle.rotationValue = rotation / 4f;
                particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                particle.prim.GetColor = (p) => colorMethod(p) * particle.Rotation * Math.Min(particle.Scale, 1.5f);
            }
        }

        // collision check and velocity changes + debuff
        for (int i = 0; i < targetCount; i++)
        {
            NPC npc = Main.npc[targets[i].npc];
            npc.AddBuff(ModContent.BuffType<WindFanDebuff>(), 60);
            Vector2 direction = (Projectile.Center - npc.Center).RotatedBy(-MathHelper.PiOver4 * 1.1f);
            direction.Normalize();
            npc.velocity = Vector2.Lerp(npc.velocity, direction * 23f, Projectile.knockBack / 100f);
            npc.netUpdate = true;
            if (npc.collideX && Math.Abs(npc.velocity.X) > 2f)
            {
                hitEnemies.Add(npc.whoAmI);
                npc.velocity = new Vector2(npc.velocity.X * -1.1f, npc.velocity.Y);
            }
            else if (npc.collideY && Math.Abs(npc.velocity.Y) > 2f)
            {
                hitEnemies.Add(npc.whoAmI);
                npc.velocity = new Vector2(npc.velocity.X, npc.velocity.Y * -1.1f);
            }
            else if (Projectile.numUpdates == -1 && Projectile.ai[0] % 30 == 0)
            {
                hitEnemies.Add(npc.whoAmI);
            }
        }

        // npc collision check + hit cooldown (else if enemies get stuck its a lot of damage)
        for (int i = 0; i < targetCount; i++)
        {
            NPC npc = Main.npc[targets[i].npc];
            var r = npc.Hitbox;
            r.Inflate(20, 20);
            for (int j = 0; j < targetCount; j++)
            {
                if (i == j)
                    continue;
                NPC other = Main.npc[targets[j].npc];
                if (r.Intersects(other.Hitbox) && hitCooldown[npc.whoAmI] <= 0)
                {
                    hitEnemies.Add(npc.whoAmI);
                    npc.velocity = (npc.Center - other.Center).SafeNormalize(Vector2.Zero)
                        .RotatedBy(MathHelper.PiOver2) * 20f;
                    hitCooldown[npc.whoAmI] = 30;
                }
            }
        }

        for (int p = 0; p < Main.maxProjectiles; p++)
        {
            if (p == Projectile.whoAmI)
                continue;
            if (Main.projectile[p].IsHostile(Main.player[Projectile.owner]) && PushableEntities.ProjectileIDs.Contains(Projectile.type) && Projectile.Distance(Main.projectile[p].Center) < Projectile.width / 2f && (!Main.projectile[p].tileCollide || Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height)))
            {
                var direction = (Projectile.Center - Main.projectile[p].Center).RotatedBy(-MathHelper.PiOver4 * 0.8f);
                direction.Normalize();
                Main.projectile[p].velocity = Vector2.Lerp(Main.projectile[p].velocity, direction * 23f, Projectile.knockBack / 150f);
                Main.projectile[p].netUpdate = true;
            }
        }
    }

    public override bool? CanCutTiles()
    {
        return false;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return hitEnemies.Contains(target.whoAmI) ? null : false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.Knockback *= 0f;
        modifiers.HitDirectionOverride = 0;
    }
}
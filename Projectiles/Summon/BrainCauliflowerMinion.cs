using Aequus.Buffs.Minion;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon {
    public class BrainCauliflowerMinion : MinionBase
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            Main.projPet[Type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 36;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (!Helper.UpdateProjActive<BrainCauliflowerBuff>(Projectile))
            {
                return;
            }

            int leader = -1;
            int minionPos = 0;
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
                {
                    if ((int)Main.projectile[i].ai[0] == 0)
                    {
                        if (leader == -1)
                        {
                            leader = i;
                        }
                        if (i == Projectile.whoAmI)
                        {
                            minionPos = count;
                        }
                    }
                    count++;
                }
            }

            Projectile.gfxOffY = (int)Helper.Wave(Main.GlobalTimeWrappedHourly * 5f - minionPos * 0.5f, -3f, 0f);
            Projectile.LoopingFrame(5);
            Projectile.CollideWithOthers();
            Projectile.rotation = Projectile.velocity.X * 0.03f;
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    {
                        var goalPosition = IdlePosition(Main.player[Projectile.owner], leader, minionPos, count);
                        float speed = Projectile.GetMinionReturnSpeed(12f, 1.25f);
                        Projectile.tileCollide = false;
                        int target = Projectile.FindTargetWithLineOfSight(600f);
                        if (target != -1)
                        {
                            Projectile.ai[0] = 1f;
                            Projectile.ai[1] = minionPos * 8 + Main.rand.Next(10);
                            break;
                        }
                        if ((Projectile.Center - goalPosition).Length() <= 30f)
                        {
                            Projectile.velocity *= 0.975f;
                            Projectile.localAI[1] = 40f;
                            Projectile.spriteDirection = Main.player[Projectile.owner].direction;
                            break;
                        }
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(goalPosition - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * speed, 0.05f);
                        if (Projectile.localAI[1] <= 0)
                        {
                            Projectile.spriteDirection = Projectile.direction;
                        }
                        else
                        {
                            Projectile.localAI[1]--;
                        }
                    }
                    break;

                case 1:
                    {
                        int target = Projectile.FindTargetWithLineOfSight(1000f);
                        if (target == -1)
                        {
                            target = Projectile.FindTargetWithinRange(800f)?.whoAmI ?? -1;
                        }
                        if (target == -1 || Main.player[Projectile.owner].Distance(Main.npc[target].Center) > 2000f)
                        {
                            Projectile.ai[0] = 0f;
                            Projectile.ai[1] = 0f;
                            Projectile.localAI[1] = 0f;
                            break;
                        }

                        if (Projectile.ai[1] < -80f)
                        {
                            Projectile.ai[1] = Main.rand.Next(10);
                            var velocity = Projectile.DirectionTo(Main.npc[target].Center) * 6.66f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                                ModContent.ProjectileType<BrainCauliflowerBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Projectile.velocity -= velocity;
                        }
                        var goalPosition = Main.npc[target].Center + -Projectile.DirectionTo(Main.npc[target].Center) * 180f;
                        if ((Projectile.Center - Main.npc[target].Center).Length() <= 200f)
                        {
                            Projectile.ai[1] -= 1.5f;
                            for (int i = 0; i < 4; i++)
                            {
                                if (Main.rand.NextBool((int)Math.Max(10 + Projectile.ai[1] / 10f, 1)))
                                {
                                    var off = Main.rand.NextVector2Unit() * Main.rand.NextFloat(Math.Max(80 + Projectile.ai[1], 1), Math.Max(100 + Projectile.ai[1], 1));
                                    var spawnLoc = new Vector2(Projectile.position.X + Projectile.width / 2f, Projectile.position.Y - Projectile.height / 4f) + off;
                                    Dust.NewDustPerfect(spawnLoc, ModContent.DustType<MonoDust>(), off / Main.rand.NextFloat(-12f, -8f), newColor: Color.Lerp(Color.Red, Color.Yellow, Main.rand.NextFloat(0.33f, 0.66f)).UseA(0), Scale: Main.rand.NextFloat(0.75f, 1.5f));
                                }
                            }
                            Projectile.velocity *= 0.9f;
                            break;
                        }
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(goalPosition - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 16f, 0.05f);
                        Projectile.spriteDirection = Projectile.direction;
                    }
                    break;
            }
        }

        public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            return new Vector2(player.position.X + player.width / 2f - (Projectile.width * 2 * (minionPos + 2)) * player.direction, player.position.Y);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            off.Y += Projectile.gfxOffY;
            var v = Projectile.position + off - Main.screenPosition;
            Main.EntitySpriteDraw(t, v, frame, lightColor.MaxRGBA(200), Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection.ToSpriteEffect(), 0);
            return false;
        }
    }
}
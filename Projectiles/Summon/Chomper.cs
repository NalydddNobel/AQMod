using AQMod.Common.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class Chomper : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hide = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 1;
            projectile.manualDirectionChange = true;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => (int)projectile.ai[1] > 120 && damageDelay <= 0;

        private void GotoPosition(Vector2 gotopos, float amount)
        {
            _gotoX = gotopos.X;
            _gotoY = gotopos.Y;
            var lerpPosition = Vector2.Lerp(projectile.Center, gotopos, amount);
            var difference = lerpPosition - projectile.Center;
            projectile.velocity = difference;
            if (difference.X < 0f)
            {
                projectile.direction = -1;
            }
            else
            {
                projectile.direction = 1;
            }
        }

        private void Movement(Vector2 center, float time, float rotation, int count, int index)
        {
            float idleDistance = projectile.width * 2.5f + Main.player[projectile.owner].width + 8f * count;
            var off = new Vector2(0f, -idleDistance).RotatedBy(rotation);
            time += index * 0.6f;
            off += new Vector2((float)Math.Sin(time) * 4, (float)Math.Cos(time) * 4);
            var gotoPosition = Main.player[projectile.owner].Center + off;
            var difference = gotoPosition - center;
            float l = difference.Length();
            GotoPosition(gotoPosition, MathHelper.Clamp(l / 1000f, 0.1f, 0.5f));
        }

        public float _gotoX;
        public float _gotoY;
        public int _targetCache = -1;
        public int _eatenTypeCache = -1;

        public int damageDelay;
        public int eatingDelay;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var center = projectile.Center;
            if (player.dead)
                aQPlayer.chomper = false;
            if (aQPlayer.chomper)
                projectile.timeLeft = 2;
            int target = -1;
            float distance = 1000f;

            if (player.HasMinionAttackTargetNPC)
            {
                int t = player.MinionAttackTargetNPC;
                float d = (Main.npc[t].Center - center).Length();
                if (d < distance)
                {
                    target = t;
                    distance = d;
                }
            }

            if (target == -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(attacker: projectile, ignoreDontTakeDamage: false))
                    {
                        var difference = npc.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (!Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[projectile.owner].position, Main.player[projectile.owner].width, Main.player[projectile.owner].height))
                            c *= 8;
                        if (c < distance)
                        {
                            target = i;
                            distance = c;
                        }
                    }
                }
            }

            _targetCache = target;
            if (target == -1)
            {
                damageDelay = 0;
                projectile.direction = Main.player[projectile.owner].direction;
                projectile.spriteDirection = projectile.direction;
                int count = 0;
                int index = 0;
                int leaderIndex = -1;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i == projectile.whoAmI)
                    {
                        if (leaderIndex == -1)
                        {
                            leaderIndex = i;
                            aQPlayer.SetMinionCarryPos((int)projectile.position.X + projectile.width / 2, (int)projectile.position.Y);
                        }
                        count++;
                        index = count;
                    }
                    else if (Main.projectile[i].active && Main.projectile[i].type == projectile.type
                        && Main.projectile[i].owner == projectile.owner)
                    {
                        if (Main.projectile[i].type == projectile.type && Main.projectile[i].ai[1] < 120)
                        {
                            continue;
                        }
                        if (leaderIndex == -1)
                        {
                            leaderIndex = i;
                        }
                        count++;
                    }
                }
                float rotation;
                if (count == 1)
                {
                    rotation = 0f;
                }
                else
                {
                    rotation = MathHelper.PiOver2 / (count - 1) * (index - 1) - MathHelper.PiOver4;
                }
                if (eatingDelay > 0)
                {
                    eatingDelay--;
                }
                if (eatingDelay < 20)
                {
                    if (_eatenTypeCache != -1)
                    {
                        NPC npc = new NPC();
                        npc.SetDefaults(_eatenTypeCache);
                        npc.life = -1;
                        npc.rotation = projectile.rotation;
                        if (projectile.spriteDirection == -1)
                        {
                            npc.rotation += MathHelper.Pi;
                        }
                        npc.velocity = npc.rotation.ToRotationVector2() * 6f;
                        npc.Center = projectile.Center;
                        Main.PlaySound(SoundID.NPCDeath13.WithVolume(0.3f).WithPitchVariance(1f));
                        npc.HitEffect(projectile.spriteDirection, 100f * AQConfigClient.Instance.EffectIntensity);
                        _eatenTypeCache = -1;
                    }
                }
                if (projectile.ai[0] < 0f && Main.myPlayer == projectile.owner)
                {
                    damageDelay = 0;
                    projectile.ai[0] = 0f;
                    projectile.ai[1] = 0f;
                }
                projectile.ai[0] += 0.08f;
                float time;
                if (leaderIndex != -1 && Main.projectile[leaderIndex].ai[0] > 0f)
                {
                    if (leaderIndex == projectile.whoAmI)
                    {
                        projectile.ai[0] += 0.04f;
                    }
                    else
                    {
                        projectile.ai[0] = 0f;
                    }
                    time = Main.projectile[leaderIndex].ai[0];
                }
                else
                {
                    time = projectile.ai[0];
                    projectile.ai[0] += 0.04f;
                }

                Movement(center, time, rotation, count, index);

                if (projectile.rotation.Abs() < 0.1f)
                {
                    projectile.rotation = 0f;
                }
                else
                {
                    projectile.rotation = projectile.rotation.AngleLerp(0f, 0.025f);
                }
                if (eatingDelay > 0)
                {
                    projectile.frame = 0;
                    if (eatingDelay < 35)
                    {
                        projectile.frame = 2;
                    }
                }
                else
                {
                    projectile.frameCounter++;
                    if (projectile.frameCounter > 5)
                    {
                        projectile.frameCounter = 0;
                        projectile.frame++;
                        if (projectile.frame >= 3)
                        {
                            projectile.frame = 0;
                        }
                    }
                }
            }
            else
            {
                projectile.ai[0] = -1f;
                projectile.ai[1]++;
                if ((int)projectile.ai[1] < 120)
                {
                    if (projectile.ai[1] < 110 && CanEat(Main.npc[target]))
                    {
                        projectile.ai[1] += Main.rand.NextFloat(4f);
                    }
                    if (damageDelay > 0)
                    {
                        damageDelay--;
                        if ((int)projectile.ai[1] > 110)
                        {
                            projectile.ai[1] = 110f;
                        }
                    }
                    if (eatingDelay > 0)
                    {
                        eatingDelay--;
                        if ((int)projectile.ai[1] > 110)
                        {
                            projectile.ai[1] = 110f;
                        }
                        if (eatingDelay <= 20 && _eatenTypeCache != -1 && Main.myPlayer == projectile.owner)
                        {
                            if (_eatenTypeCache != -1)
                            {
                                NPC npc = new NPC();
                                npc.SetDefaults(_eatenTypeCache);
                                npc.life = -1;
                                npc.rotation = projectile.rotation;
                                npc.velocity = projectile.rotation.ToRotationVector2() * 6f;
                                npc.Center = projectile.Center;
                                Main.PlaySound(SoundID.NPCDeath13.WithVolume(0.3f).WithPitchVariance(1f));
                                npc.HitEffect(projectile.spriteDirection, 100);
                                _eatenTypeCache = -1;
                            }
                        }
                        projectile.frame = 0;
                        if (eatingDelay < 35)
                        {
                            projectile.frame = 2;
                        }
                    }
                    else
                    {
                        if ((int)projectile.ai[1] < 40)
                        {
                            projectile.frame = 0;
                        }
                        else if ((int)projectile.ai[1] < 80)
                        {
                            projectile.frame = 1;
                        }
                        else
                        {
                            projectile.frame = 2;
                        }
                    }
                    projectile.direction = Main.npc[target].Center.X < projectile.Center.X ? -1 : 1;
                    projectile.spriteDirection = projectile.direction;
                    int count = 0;
                    int index = 0;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (i == projectile.whoAmI)
                        {
                            if (count == 0)
                            {
                                aQPlayer.SetMinionCarryPos((int)projectile.position.X + projectile.width / 2, (int)projectile.position.Y);
                            }
                            count++;
                            index = count;
                        }
                        else if (Main.projectile[i].active && Main.projectile[i].type == projectile.type
                            && Main.projectile[i].owner == projectile.owner)
                        {
                            if (Main.projectile[i].type == projectile.type && Main.projectile[i].ai[1] < 120)
                            {
                                continue;
                            }
                            count++;
                        }
                    }
                    float rotation;
                    if (count == 1)
                    {
                        rotation = 0f;
                    }
                    else
                    {
                        rotation = MathHelper.PiOver2 / (count - 1) * (index - 1) - MathHelper.PiOver4;
                    }

                    Movement(center, 0f, rotation, count, index);

                    if (projectile.rotation.Abs() < 0.1f)
                    {
                        projectile.rotation = 0f;
                    }
                    else
                    {
                        if (projectile.rotation < -MathHelper.PiOver2)
                        {
                            projectile.rotation += MathHelper.PiOver2;
                        }
                        else if (projectile.rotation > MathHelper.PiOver2)
                        {

                        }
                        projectile.rotation = projectile.rotation.AngleLerp(0f, 0.025f);
                    }
                    projectile.rotation = (Main.npc[target].Center - projectile.Center).ToRotation();
                }
                else
                {
                    if ((int)projectile.ai[1] == 120)
                    {
                        projectile.velocity = Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 13f;
                        projectile.netUpdate = true;
                    }
                    projectile.ai[1] += Main.rand.NextFloat(0.5f, 2f);
                    if ((int)projectile.ai[1] > 180)
                    {
                        projectile.ai[1] = 0f;
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public bool CanEat(NPC npc)
        {
            return eatingDelay <= 0 && !npc.boss && npc.width * npc.height <= projectile.width * projectile.height && npc.life <= 100;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.Center.X < Main.player[projectile.owner].Center.X)
            {
                hitDirection = -1;
            }
            else
            {
                hitDirection = 1;
            }
            projectile.ai[1] = 0f;
            damageDelay = damage;
            if (CanEat(target))
            {
                damage = 0;
                damageDelay = target.life;
                eatingDelay = 120;
                _eatenTypeCache = target.netID;
                switch (target.type)
                {
                    default:
                        Main.PlaySound(SoundID.Item2, target.Center);
                        break;

                    case NPCID.BlueSlime:
                    case NPCID.SpikedIceSlime:
                    case NPCID.SpikedJungleSlime:
                    case NPCID.SlimeSpiked:
                        Main.PlaySound(SoundID.Item3, target.Center);
                        break;
                }
                int oldLife = target.life;
                target.life = -1;
                target.active = false;
                target.NPCLoot();
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(damageDelay);
            writer.Write(eatingDelay);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            damageDelay = reader.ReadInt32();
            eatingDelay = reader.ReadInt32();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            Rectangle frame = new Rectangle(0, projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 center = new Vector2(projectile.width / 2, projectile.height / 2);
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var drawPos = projectile.position + center - Main.screenPosition;
            drawPos = new Vector2((int)drawPos.X, (int)drawPos.Y);
            Main.spriteBatch.Draw(texture, drawPos, frame, lightColor, projectile.rotation, frame.Size() / 2f, 1f, effects, 0f);
            return false;
        }
    }
}
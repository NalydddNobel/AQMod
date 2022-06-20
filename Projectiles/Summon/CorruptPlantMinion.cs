using Aequus.Buffs.Minion;
using Aequus.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class CorruptPlantMinion : MinionBase
    {
        public const int FRAMES_LARGE = 14;
        public const int FRAMES_MED = 10;

        public int typeEaten;
        public int eatTimer;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            Main.projPet[Type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            this.SetTrail(12);
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override bool MinionContactDamage()
        {
            return Projectile.ai[1] >= 1000f && Projectile.ai[1] < 2000f;
        }

        public override void AI()
        {
            int slots;
            Projectile.localAI[0] = slots = CountSlots();
            if (!AequusHelpers.UpdateProjActive<CorruptPlantBuff>(Projectile) || Projectile.localAI[0] <= 0)
            {
                return;
            }

            Projectile.damage = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<CorruptPlantCounter>() && Main.projectile[i].owner == Projectile.owner)
                {
                    if (Main.projectile[i].damage > Projectile.damage)
                    {
                        Projectile.damage = Projectile.originalDamage = Main.projectile[i].damage;
                    }
                }
            }

            int size = Size();
            if (Projectile.frame == (size == 2 ? 5 : 4))
            {
                Projectile.frame = 0;
            }
            int range = Range(slots);
            if (Projectile.ai[1] >= 1000f)
            {
                Projectile.ai[1] += 1f / (Projectile.extraUpdates + 1);

                if (Projectile.ai[1] % 1000f >= 20f)
                {
                    Projectile.tileCollide = true;
                }
                if (Projectile.ai[1] >= 2000f || Projectile.Distance(Main.player[Projectile.owner].Center) > range * 16f)
                {
                    Projectile.velocity *= 0.95f;
                    Projectile.tileCollide = false;
                    Projectile.position += Main.player[Projectile.owner].velocity;
                    Projectile.extraUpdates = 0;
                }
                else
                {
                    Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    if (Projectile.spriteDirection == -1)
                    {
                        Projectile.rotation -= MathHelper.Pi;
                    }
                }

                if (Projectile.ai[1] % 1000f >= 40f)
                {
                    Projectile.ai[1] = 0f;
                }
                return;
            }

            Projectile.rotation = 0f;
            int target = Projectile.GetMinionTarget(Main.player[Projectile.owner].Center, out float d, range * 16f, 0f);
            if (target != -1)
            {
                Projectile.ai[1]++;

                int openingFrames = size == 2 ? 5 : 4;
                Projectile.frame = (int)(Projectile.ai[1] / (60f / openingFrames));

                if (Projectile.ai[1] > 60f)
                {
                    Projectile.frame = openingFrames - 1;
                    Projectile.ai[1] = 1000f;
                    Projectile.extraUpdates = 2;
                    float speed = (14f + slots * 0.8f) / (Projectile.extraUpdates + 1);
                    Projectile.velocity = Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * speed;
                    Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    if (Projectile.spriteDirection == -1)
                    {
                        Projectile.rotation -= MathHelper.Pi;
                    }
                    return;
                }
            }
            else
            {
                if (typeEaten != 0)
                {
                    Projectile.ai[1] = 0f;
                    eatTimer++;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (Projectile.frame > 0 && eatTimer % 6 == 0)
                        {
                            Projectile.frame--;
                        }
                        if (eatTimer > 80 + Main.rand.Next(100))
                        {
                            if (eatTimer < 1000)
                            {
                                Projectile.netUpdate = true;
                                Burp(typeEaten);
                            }
                            else
                            {
                                typeEaten = 0;
                            }
                            eatTimer = 1000;
                        }
                    }
                }
                else if (eatTimer != 0)
                {
                    eatTimer++;
                    if (Projectile.frame > 0 && eatTimer % 6 == 0)
                    {
                        Projectile.frame--;
                    }
                    if (eatTimer % 1000 > 25)
                    {
                        eatTimer = 0;
                        Projectile.netUpdate = true;
                    }
                }

                if (Projectile.ai[1] > 0f)
                {
                    Projectile.ai[1] -= 0.33f;
                    if (Projectile.ai[1] < 0f)
                    {
                        Projectile.ai[1] = 0f;
                    }
                }
            }

            Projectile.extraUpdates = 0;
            var gotoPosition = DefaultIdlePosition();
            var diff = gotoPosition - Projectile.Center;
            if ((Main.player[Projectile.owner].Center - Projectile.Center).Length() > 2000f)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity *= 0.1f;
                diff = Vector2.UnitY;
            }
            var ovalDiff = new Vector2(diff.X, diff.Y *= 3f);
            float ovalLength = ovalDiff.Length();
            if (ovalLength > 20f)
            {
                var velocity = diff / 20f;
                if (velocity.Length() < 4f)
                {
                    velocity = Vector2.Normalize(velocity).UnNaN() * 4f;
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.1f);
            }

            Projectile.spriteDirection = Main.player[Projectile.owner].direction;
        }
        public void Burp(int npcType)
        {
            Projectile.frame = 3;
            NPC npc = new NPC();
            npc.SetDefaults(npcType);
            npc.life = -1;
            npc.rotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
            {
                npc.rotation -= MathHelper.Pi;
            }
            npc.velocity = npc.rotation.ToRotationVector2() * 6f;
            npc.Center = Projectile.Center;
            SoundEngine.PlaySound(SoundID.NPCDeath13.WithVolume(0.3f).WithPitch(1f - Size() * 0.5f));
            npc.HitEffect(Projectile.spriteDirection, 10f);
        }

        public int CountSlots()
        {
            return Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<CorruptPlantCounter>()];
        }
        public int Range(int slots)
        {
            return 15 + slots;
        }
        public int Size()
        {
            return (int)Projectile.localAI[0] >= 6 ? 2 : (int)Projectile.localAI[0] >= 3 ? 1 : 0;
        }

        public override Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            return base.IdlePosition(player, leader, minionPos, count) + new Vector2(0f, -20f + -10f * Math.Min(Range(CountSlots()) / 1.5f, 10));
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (1f + CountSlots() * 0.15f));
            if ((target.life - damage) < 75 + CountSlots() * 15f)
            {
                typeEaten = target.netID;
                target.active = false;
                target.life = -1;
                target.NPCLoot();
                SoundEngine.PlaySound(SoundID.Item2, target.Center);
                if (Main.netMode != NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, target.whoAmI);
                }
                damage = 1;
                return;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.ai[1] > 1000f)
            {
                Projectile.ai[1] += 1000f;
                Projectile.velocity = Projectile.velocity * 0.2f;
            }
            Projectile.frame = Size() == 2 ? 5 : 4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] > 1000f)
            {
                Projectile.ai[1] += 1000f;
                Projectile.velocity = -oldVelocity * 0.5f;
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            int slots = (int)Projectile.localAI[0];
            if (slots >= 6)
            {
                t = ModContent.Request<Texture2D>(Texture + "_Large").Value;
                frame = t.Frame(verticalFrames: FRAMES_LARGE, frameY: Projectile.frame);
                origin = frame.Size() / 2f;
            }
            else if (slots >= 3)
            {
                t = ModContent.Request<Texture2D>(Texture + "_Med").Value;
                frame = t.Frame(verticalFrames: FRAMES_MED, frameY: Projectile.frame);
                origin = frame.Size() / 2f;
            }

            var chainTexture = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
            var chainOrigin = new Vector2(chainTexture.Width * 0.5f, 4f);
            var chainPos = Main.player[Projectile.owner].Center;
            var chainDir = Vector2.Normalize(new Vector2(Main.player[Projectile.owner].velocity.X / -4f - Projectile.spriteDirection, -2f));

            var vineColor = new Color(80, 195, 100);
            var v = Main.rgbToHsl(vineColor);
            v.X += slots / 50f;
            v.Y *= 0.5f;
            v.Y += AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, -0.1f, 0.1f);
            vineColor = Main.hslToRgb(v);
            var chainEnd = Projectile.Center + new Vector2(t.Width * 0.25f * -Projectile.spriteDirection, 0f).RotatedBy(Projectile.rotation);
            float maxMove = (chainTexture.Height - 4f) * 0.75f;
            float minMin = 0.06f;
            var chainLength = (chainEnd - chainPos).Length();
            float chainLengthStrengthingRange = Range(slots) * 12f; // * 0.75f * 16f;
            if (chainLength > chainLengthStrengthingRange)
            {
                minMin += (chainLength - chainLengthStrengthingRange) / (Range(slots) * 0.25f) * 0.33f;
                minMin = Math.Min(minMin, 0.4f);
            }
            for (int i = 0; i < 100 && (chainPos - chainEnd).Length().UnNaN() > chainTexture.Height / 2f; i++)
            {
                float movement = (chainPos - chainEnd).Length().UnNaN();
                if (movement > maxMove)
                {
                    movement = maxMove;
                }
                chainPos += chainDir * movement;
                float length = (chainPos - chainEnd).Length().UnNaN();
                float min = minMin;
                if (length < 80f)
                {
                    min += length / 240f;
                }
                chainDir = Vector2.Lerp(chainDir, Vector2.Normalize(chainEnd - chainPos), Math.Max(1f - AequusHelpers.CalcProgress(100, i), min));
                Main.EntitySpriteDraw(chainTexture, chainPos.Floor() - Main.screenPosition, null, Lighting.GetColor((int)(chainPos.X / 16f), (int)(chainPos.Y / 16f), vineColor), chainDir.ToRotation() + MathHelper.PiOver2, chainOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            bool drawT = Projectile.ai[1] > 1000f && Projectile.velocity.Length() > 2f && (Projectile.oldPos[1] - Projectile.position).Length() > 2f;
            int endT = trailLength;
            if (drawT)
            {
                for (int i = 0; i < trailLength; i++)
                {
                    if (i != trailLength - 1 && (Projectile.oldPos[i] - Projectile.oldPos[i + 1]).Length() <= 1f)
                    {
                        endT = i;
                        break;
                    }
                    float p = AequusHelpers.CalcProgress(trailLength, i);
                    Main.EntitySpriteDraw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, new Color(30, 30, 30, 0) * p * p, Projectile.oldRot[i], origin, Projectile.scale + 0.4f * (1f - p), Projectile.spriteDirection.ToSpriteEffect(), 0);
                }
            }
            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection.ToSpriteEffect(), 0);
            if (drawT)
            {
                for (int i = 0; i < trailLength; i++)
                {
                    if (i == endT)
                    {
                        break;
                    }
                    float p = AequusHelpers.CalcProgress(trailLength, i);
                    Main.EntitySpriteDraw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, new Color(20, 20, 20, 0) * p, Projectile.oldRot[i], origin, (Projectile.scale + 0.4f * (1f - p)) * 1.25f, Projectile.spriteDirection.ToSpriteEffect(), 0);
                }
            }

            return false;
        }
    }

    public class CorruptPlantCounter : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetStaticDefaults()
        {
            GlowCore.ProjectileBlacklist.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.AbigailCounter);
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (!AequusHelpers.UpdateProjActive<CorruptPlantBuff>(Projectile))
            {
                return;
            }
            Projectile.position = Main.player[Projectile.owner].position;
            Projectile.position.X += Projectile.minionPos * (Projectile.width + 4f);
        }
    }
}
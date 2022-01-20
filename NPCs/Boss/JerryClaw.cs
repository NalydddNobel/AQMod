using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Effects.ScreenEffects;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Boss
{
    [AutoloadBossHead]
    public class JerryClaw : ModNPC, IDecideFallThroughPlatforms
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.width = 100;
            npc.height = 100;
            npc.lifeMax = 1000;
            npc.damage = 20;
            npc.defense = 80;
            npc.aiStyle = -1;
            npc.dontTakeDamage = true;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.SetLiquidSpeed(1f, 0.5f, 0.25f);
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Gore.NewGore(npc.position + new Vector2(20f, 20f), npc.velocity, mod.GetGoreSlot("Gores/JerryClaw_0"));
            Gore.NewGore(npc.position + new Vector2(npc.width - 20, 20f), npc.velocity, mod.GetGoreSlot("Gores/JerryClaw_1"));
            Gore.NewGore(npc.position + new Vector2(npc.width / 2f - 10f, npc.height / 2f + 10f), npc.velocity, mod.GetGoreSlot("Gores/JerryClaw_2"));
            for (int i = 0; i < 50; i++)
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
        }

        private const int PHASE_JUMPING_AND_SETTING_UP_ATTACKS_I_THINK = 1;
        private const int PHASE_PEARL = 4;

        public override void AI()
        {
            if (!Main.npc[(int)npc.ai[0]].active)
            {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
                return;
            }
            if ((int)npc.ai[2] == 0)
            {
                npc.ai[2] = 3f;
                if (npc.direction == -1)
                    return; // direction 1 needs to sync some things with direction -1 first
            }
            if (npc.ai[2] == -1)
            {
                if (npc.velocity.Y < 0)
                    npc.velocity.Y = 0;
                npc.velocity.Y += 0.1f;
                return;
            }
            bool inTiles = Collision.SolidCollision(npc.position + new Vector2(0, -2), npc.width, npc.height);
            bool canHit = Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
            int otherClaw = (int)Main.npc[(int)npc.ai[0]].localAI[npc.direction == 1 ? 0 : 1];
            if (Main.npc[otherClaw].active)
            {
                if (npc.ai[2] != Main.npc[otherClaw].ai[2] && npc.ai[2] != 2) // if their phases go out of sync and the claw is not adjusting itself
                {
                    npc.target = Main.npc[otherClaw].target;
                    npc.ai[2] = Main.npc[otherClaw].ai[2];
                    npc.ai[3] = Main.npc[otherClaw].ai[3];
                    npc.noGravity = Main.npc[otherClaw].noGravity;
                    npc.noTileCollide = Main.npc[otherClaw].noTileCollide;
                    Main.npc[otherClaw].behindTiles = false;
                    npc.behindTiles = false;
                    npc.netUpdate = true;
                }
                if (npc.direction == 1) // sync the target with the -1 direction claw
                {
                    npc.TargetClosest(faceTarget: false);
                    if (npc.target != Main.npc[otherClaw].target)
                    {
                        Main.npc[otherClaw].target = npc.target;
                        Main.npc[otherClaw].netUpdate = true;
                    }
                }
                if (!npc.HasValidTarget) // Flee if there is no target
                {
                    npc.ai[2] = -1;
                    if (npc.timeLeft > 80)
                        npc.timeLeft = 80;
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                    npc.netUpdate = true;
                }
                if (npc.velocity.Y == 0f)
                    npc.velocity.X *= 0.8f;
                if ((int)npc.ai[2] == PHASE_JUMPING_AND_SETTING_UP_ATTACKS_I_THINK && !npc.noTileCollide)
                {
                    if (inTiles)
                    {
                        if (npc.ai[3] > 0)
                            npc.ai[3] = 0;
                        npc.ai[3]--;
                        if (npc.ai[3] < -150f)
                        {
                            if (Main.npc[otherClaw].active)
                            {
                                npc.ai[2] = Main.npc[otherClaw].ai[2];
                                npc.ai[3] = Main.npc[otherClaw].ai[3];
                                npc.target = Main.npc[otherClaw].target;
                                if (npc.HasValidTarget)
                                {
                                    npc.noTileCollide = true;
                                    npc.ai[2] = 2f;
                                    npc.netUpdate = true;
                                }
                                else
                                {
                                    npc.lifeMax = -1;
                                    npc.HitEffect();
                                    npc.active = false;
                                }
                            }
                            else
                            {
                                npc.lifeMax = -1;
                                npc.HitEffect();
                                npc.active = false;
                            }
                        }
                        return;
                    }
                    else if (!canHit)
                    {
                        if (npc.ai[3] > 0)
                            npc.ai[3] = 0;
                        npc.ai[3]--;
                        if (npc.ai[3] < -300f)
                        {
                            if (Main.npc[otherClaw].active)
                            {
                                npc.ai[2] = Main.npc[otherClaw].ai[2];
                                npc.ai[3] = Main.npc[otherClaw].ai[3];
                                npc.target = Main.npc[otherClaw].target;
                                if (npc.HasValidTarget)
                                {
                                    npc.noTileCollide = true;
                                    npc.ai[2] = 2;
                                    npc.netUpdate = true;
                                }
                                else
                                {
                                    npc.lifeMax = -1;
                                    npc.HitEffect();
                                    npc.active = false;
                                }
                            }
                            else
                            {
                                npc.lifeMax = -1;
                                npc.HitEffect();
                                npc.active = false;
                            }
                        }
                    }
                    Vector2 gotoPos = Main.player[npc.target].Center;
                    if (canHit)
                        gotoPos += new Vector2(12f * npc.localAI[0] * npc.direction, 0f);
                    float distX = gotoPos.X - npc.Center.X;
                    float distY = gotoPos.Y - npc.Center.Y;
                    if (!inTiles && canHit && npc.ai[3] < 0)
                        npc.ai[3]++;
                    if (npc.localAI[1] > 0)
                    {
                        npc.localAI[1]--;
                        if (npc.velocity.Y < 0)
                        {
                            if (Math.Abs(distX) > npc.width)
                            {
                                if (distX > 0f)
                                {
                                    npc.velocity.X += 0.1f;
                                    if (npc.velocity.X > 4)
                                        npc.velocity.X = 4f;
                                }
                                else
                                {
                                    npc.velocity.X -= 0.1f;
                                    if (npc.velocity.X < -4)
                                        npc.velocity.X = -4f;
                                }
                            }
                            else
                            {
                                npc.velocity.X *= 0.95f;
                            }
                        }
                    }
                    else
                    {
                        // jump logic
                        if (npc.velocity.Y == 0)
                        {
                            if (distY < -700f)
                            {
                                npc.velocity.Y = -35f; // biggest jump
                                npc.localAI[1] = 400;
                                npc.localAI[2] = 0;
                            }
                            else if (distY < -300f)
                            {
                                npc.velocity.Y = -20f; // bigger jump
                                npc.localAI[1] = 300;
                                npc.localAI[2] = 0;
                            }
                            else if (distY < npc.height * -1.15f)
                            {
                                npc.velocity.Y = -14f; // big jump
                                npc.localAI[1] = 200;
                                npc.localAI[2] = 0;
                            }
                            else
                            {
                                if (Math.Abs(distX) > npc.width)
                                {
                                    float xSpeed = 4f;
                                    float ySpeed = 8f;
                                    if (distX > 150f)
                                    {
                                        xSpeed *= 1.5f;
                                        ySpeed *= 0.9f;
                                    }
                                    if (distX > 350f)
                                    {
                                        xSpeed *= 2f;
                                        ySpeed *= 0.9f;
                                    }
                                    if (distX > 800f)
                                    {
                                        xSpeed *= 2.5f;
                                        ySpeed *= 0.9f;
                                    }
                                    npc.velocity.Y = -ySpeed;
                                    npc.localAI[1] = 100;
                                    npc.localAI[2] = 0;
                                    if (distX > 0f)
                                    {
                                        npc.velocity.X = xSpeed;
                                    }
                                    else
                                    {
                                        npc.velocity.X = -xSpeed;
                                    }
                                }
                            }
                        }
                    }
                    if (npc.localAI[2] == 0f && npc.velocity.Y == 0f && npc.collideY) // When it falls back onto the ground, do some effects
                    {
                        npc.netUpdate = true;
                        npc.localAI[2] = 1f;
                        var spawnPos = new Vector2(npc.position.X, npc.position.Y + npc.height - 2);
                        if (Main.netMode != NetmodeID.Server && AQConfigClient.c_TonsofScreenShakes)
                        {
                            float distance = Vector2.Distance(npc.Center, Main.LocalPlayer.Center);
                            if (distance < 600)
                                ScreenShakeManager.AddShake(new BasicScreenShake(16, AQGraphics.MultIntensity((int)(600f - distance) / 64)));
                        }
                        Main.PlaySound(SoundID.Item14, npc.position);
                        for (int i = 0; i < 40; i++)
                        {
                            Dust.NewDust(spawnPos, npc.width, 2, 31);
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            Gore.NewGore(new Vector2(spawnPos.X + Main.rand.NextFloat(npc.width), spawnPos.Y), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 2f)), 61 + Main.rand.Next(3));
                        }
                        if ((Main.player[npc.target].Center - Main.npc[(int)npc.ai[0]].Center).Length() < 1200f)
                        {
                            int chance = 3;
                            if (AQProjectile.CountProjectiles(ModContent.ProjectileType<JerryPearl>()) >= 8)
                                chance += 5;
                            if (Main.rand.NextBool(chance))
                            {
                                npc.ai[2] = PHASE_PEARL;
                                npc.ai[3] = 0f;
                                Main.npc[otherClaw].ai[2] = PHASE_PEARL;
                                if (Main.npc[(int)npc.ai[0]].life / (float)Main.npc[(int)npc.ai[0]].lifeMax > 0.5f)
                                    Main.npc[otherClaw].ai[3] = -1f;
                                else
                                    Main.npc[otherClaw].ai[3] = 0f;
                            }
                        }
                    }
                } // Jumping and most attack phase logic
                else if ((int)npc.ai[2] == 2)
                {
                    npc.ai[3] = 0f;
                    if (!Collision.SolidCollision(npc.position, npc.width, npc.height) && canHit || npc.getRect().Intersects(new Rectangle((int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, Main.player[npc.target].width, Main.player[npc.target].height)))
                    {
                        npc.target = Main.npc[otherClaw].target;
                        npc.ai[2] = Main.npc[otherClaw].ai[2];
                        if (npc.ai[2] == 2)
                            npc.ai[2] = 1;
                        npc.ai[3] = Main.npc[otherClaw].ai[3];
                        npc.noTileCollide = false;
                        npc.netUpdate = true;
                        return;
                    }
                    npc.noTileCollide = true;
                    npc.velocity = new Vector2(8f, 0f).RotatedBy((Main.player[npc.target].Center + new Vector2(12f * npc.localAI[0] * npc.direction, 0f) - npc.Center).ToRotation());
                    Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 15)].velocity = -npc.velocity * 0.15f;
                } // Flows through tiles if stuck
                else if ((int)npc.ai[2] == 3)
                {
                    npc.behindTiles = true;
                    npc.noTileCollide = true;
                    npc.noGravity = true;
                    if (!Collision.SolidCollision(npc.position, npc.width, npc.height) || Main.player[npc.target].position.Y < npc.position.Y + npc.height / 2f)
                    {
                        if (Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            npc.behindTiles = false;
                            npc.noTileCollide = false;
                            npc.noGravity = false;
                            npc.ai[2] = 1f;
                        }
                        else
                        {
                            npc.velocity = new Vector2(8f, 0f).RotatedBy((Main.player[npc.target].Center + new Vector2(12f * npc.localAI[0] * npc.direction, 0f) - npc.Center).ToRotation());
                        }
                    }
                    else
                    {
                        npc.velocity.Y -= 0.25f;
                    }
                } // Intro sand sequence
                else if ((int)npc.ai[2] == PHASE_PEARL)
                {
                    if (npc.ai[3] != -1)
                    {
                        int pearlType = ModContent.ProjectileType<JerryPearl>();
                        npc.ai[3]++;
                        if (npc.ai[3] < 30f)
                        {
                            _showPearl = true;
                            _mouthGotoRotation = -MathHelper.PiOver4;
                            _mouthGotoLerp = 0.2f;
                        }
                        else if (npc.ai[3] == 30f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, new Vector2(4f * -npc.direction, -11f), pearlType, 10, 1f, Main.myPlayer);
                            Main.PlaySound(SoundID.Item61, npc.position);
                            _showPearl = false;
                        }
                        else if (npc.ai[3] >= 60f)
                        {
                            npc.ai[2] = PHASE_JUMPING_AND_SETTING_UP_ATTACKS_I_THINK;
                            npc.ai[3] = 0f;
                            npc.localAI[2] = 1f;
                            Main.npc[otherClaw].ai[2] = PHASE_JUMPING_AND_SETTING_UP_ATTACKS_I_THINK;
                            Main.npc[otherClaw].ai[3] = 0f;
                            Main.npc[otherClaw].localAI[2] = 1f;
                            _mouthGotoRotation = 0f;
                            _mouthGotoLerp = 0.1f;
                            ((JerryClaw)Main.npc[otherClaw].modNPC)._mouthGotoRotation = _mouthGotoRotation;
                            ((JerryClaw)Main.npc[otherClaw].modNPC)._mouthGotoLerp = _mouthGotoLerp;
                        }
                    }
                } // NPC is idle and awaits for a pearl
            }
            else
            {
                npc.lifeMax = -1;
                npc.HitEffect();
                npc.active = false;
            }
            if ((int)npc.ai[2] == 2)
                return;
            if (npc.wet)
            {
                var center = npc.Center;
                if (center.Y < Main.player[npc.target].Center.Y)
                {
                    npc.velocity.Y += 0.25f;
                    if (npc.velocity.Y > 16f)
                        npc.velocity.Y = 16f;
                }
                else
                {
                    npc.velocity.Y -= 0.25f;
                    if (npc.velocity.Y < -16f)
                        npc.velocity.Y = -16f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        private float _mouthRotation = 0f;
        private float _mouthGotoRotation = 0f;
        private float _mouthGotoLerp = 0.1f;
        private bool _showPearl = false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (_mouthGotoRotation != _mouthRotation)
            {
                _mouthRotation = _mouthRotation.AngleLerp(_mouthGotoRotation, _mouthGotoLerp);
                if ((_mouthRotation - _mouthGotoRotation).Abs() < 0.0157f)
                    _mouthRotation = _mouthGotoRotation;
            }
            var texture = TextureGrabber.GetNPC(npc.type);
            var drawPosition = npc.Center;
            var origin = npc.frame.Size() / 2f;
            var spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (_mouthRotation != 0f)
            {
                if (_showPearl)
                {
                    var pearlTexture = TextureGrabber.GetProjectile(ModContent.ProjectileType<JerryPearl>());
                    var pearlFrame = new Rectangle(0, 0, pearlTexture.Width, pearlTexture.Height);
                    var pearlOrig = pearlFrame.Size() / 2f;
                    var pearlDrawPosition = drawPosition - Main.screenPosition;
                    if (npc.spriteDirection == 1)
                    {
                        pearlDrawPosition += new Vector2(2f, 8f);
                    }
                    else
                    {
                        pearlDrawPosition += new Vector2(-2f, 8f);
                    }
                    Main.spriteBatch.Draw(pearlTexture, pearlDrawPosition, pearlFrame, drawColor, npc.rotation, pearlOrig, npc.scale, SpriteEffects.None, 0f);
                }
                var frameHeight = texture.Height / Main.npcFrameCount[npc.type];
                var topFrame = new Rectangle(npc.frame.X, npc.frame.Y + frameHeight, npc.frame.Width, npc.frame.Height + 2);
                var bottomFrame = new Rectangle(npc.frame.X, npc.frame.Y + frameHeight * 2, npc.frame.Width, npc.frame.Height + 2);
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition + new Vector2(0f, -2f), bottomFrame, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
                var topOrigin = new Vector2(18f, 50f);
                float rotation = _mouthRotation;
                if (npc.spriteDirection == 1)
                {
                    topOrigin.X = npc.frame.Width - topOrigin.X;
                    rotation = -rotation;
                }
                var jawConnectionPosition = npc.position + topOrigin + new Vector2(-13f, -2f);
                Main.spriteBatch.Draw(texture, jawConnectionPosition - Main.screenPosition, topFrame, drawColor, rotation, topOrigin, npc.scale, spriteEffects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, npc.frame, drawColor, npc.rotation, origin, npc.scale, spriteEffects, 0f);
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            return !npc.HasValidTarget ? true : npc.position.Y + npc.height < Main.player[npc.target].position.Y;
        }
    }
}
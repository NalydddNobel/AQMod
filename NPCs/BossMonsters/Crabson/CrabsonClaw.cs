using Aequus;
using Aequus.Common;
using Aequus.NPCs.BossMonsters.Crabson.Common;
using Aequus.NPCs.BossMonsters.Crabson.Projectiles;
using Aequus.NPCs.BossMonsters.Crabson.Projectiles.Old;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.Crabson;

[AutoloadBossHead]
[WorkInProgress]
public class CrabsonClaw : CrabsonBossNPC {
    public float mouthAnimation;

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DevelopmentFeatures;
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
            Hide = true,
        };
    }

    public override void SetDefaults() {
        base.SetDefaults();
        NPC.width = 90;
        NPC.height = 90;
        NPC.damage = 40;
        NPC.defense = 20;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        DrawOffsetY = 14f;
    }

    public override void OnSpawn(IEntitySource source) {
        if (!Helper.HereditarySource(source, out var entity) || entity is not NPC parentNPC) {
            return;
        }

        npcBody = parentNPC.whoAmI;
    }

    private void SetHand(CrabsonBossNPC segment) {
        if (segment.NPC.direction == -1) {
            npcHandLeft = segment.NPC.whoAmI;
        }
        else {
            npcHandRight = segment.NPC.whoAmI;
        }
    }

    private Vector2 GetRestingPosition(NPC body, Vector2 offset) {
        return body.Center + new Vector2(NPC.direction * 170f, body.height * -1.5f) + offset;
    }

    private void GoTo(Vector2 where, float scalingSpeed, float flatSpeed, float maxSpeed = 6f, float restingDistance = 50f, float restingSpeed = 0.9f) {
        var diff = where - NPC.Center;
        NPC.velocity += diff * scalingSpeed;
        NPC.velocity += Vector2.Normalize(diff) * flatSpeed;
        if (NPC.velocity.Length() > maxSpeed || diff.Length() < restingDistance) {
            NPC.velocity *= restingSpeed;
        }
    }

    private void GoToDefaultPosition(NPC body, float scalingSpeed, float flatSpeed, float maxSpeed = 6f, float restingDistance = 50f, float restingSpeed = 0.9f) {
        GoTo(GetRestingPosition(body, Vector2.Zero), scalingSpeed, flatSpeed, maxSpeed, restingDistance, restingSpeed);
    }

    private void GoToDefaultPosition(NPC body) {
        GoToDefaultPosition(body, 0.01f, 1f);
    }

    public override void AI() {

        SharedAI();
        NPC.direction = (int)NPC.ai[3];
        NPC.spriteDirection = (int)NPC.ai[3];
        if (npcBody == -1 || !Main.npc[npcBody].active || Main.npc[npcBody].ModNPC is not CrabsonBossNPC body) {
            NPC.KillEffects(quiet: true);
            return;
        }

        SetHand(this);
        NPC.realLife = npcBody;

        NPC.CollideWithOthers(0.3f);

        NPC.target = body.NPC.target;
        Player target = Main.player[NPC.target];
        float lifeRatio = Math.Clamp(body.NPC.life / (float)body.NPC.lifeMax, 0f, 1f);
        float battleProgress = 1f - lifeRatio;
        float startingRotation = (int)NPC.ai[3] == -1 ? MathHelper.Pi : 0f;
        var topLeftTile = NPC.position.ToTileCoordinates();
        var centerTile = NPC.Center.ToTileCoordinates();
        NPC.noTileCollide = true;
        switch (SharedAction) {
            case ACTION_CLAWSHOTS: {
                    if (NPC.ai[1] <= 0f) {
                        break;
                    }

                    if (NPC.Distance(target.Center) > 450f) {
                        NPC.velocity += NPC.DirectionTo(target.Center);
                    }
                    else {
                        NPC.velocity *= 0.92f;
                    }

                    ActionTimer += Main.rand.NextFloat(0.5f);
                    if (ActionTimer > 90f) {
                        mouthAnimation = MathHelper.Lerp(mouthAnimation, 0.4f, 0.3f);

                        if (ActionTimer > 100f && (int)ActionTimer % 80 == 0) {
                            NPC.velocity = -NPC.DirectionTo(target.Center) * 10f;
                            NPC.netUpdate = true;
                            SoundEngine.PlaySound(AequusSounds.shoot_Umystick, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -NPC.velocity * 1.5f, ModContent.ProjectileType<CrabsonPearl>(), 20, 0f, Main.myPlayer, ai1: 1f);
                            }
                        }
                    }
                    NPC.rotation = (NPC.Center - target.Center).ToRotation();
                    break;
                }

            case ACTION_CLAWRAIN: {
                    GoToDefaultPosition(body.NPC);
                    if (NPC.rotation == 0f) {
                        NPC.rotation = startingRotation;
                    }
                    mouthAnimation = MathHelper.Lerp(mouthAnimation, 0.4f, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, startingRotation + MathHelper.PiOver2 * NPC.ai[3], 0.1f);
                    ActionTimer++;
                    float wantedTime = 20f + 40f * lifeRatio;
                    if (ActionTimer == (int)wantedTime) {
                        ActionTimer = wantedTime;
                        NPC.velocity = NPC.rotation.ToRotationVector2() * 20f;
                        NPC.netUpdate = true;
                        SoundEngine.PlaySound(AequusSounds.shoot_Umystick, NPC.Center);

                        int projAmount = 13;
                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            for (int i = 0; i < projAmount; i++) {
                                var p = Projectile.NewProjectileDirect(
                                    NPC.GetSource_FromThis(),
                                    NPC.Center,
                                    -Vector2.Normalize(NPC.velocity).RotatedBy((i - projAmount / 2f) * 0.1f) * 9f,
                                    ProjectileID.MoonlordArrowTrail,
                                    20, 0f,
                                    Main.myPlayer);
                                p.friendly = false;
                                p.hostile = true;
                                p.tileCollide = false;
                                if (!PhaseTwo) {
                                    p.extraUpdates /= 2;
                                }
                                p.timeLeft *= 4;
                            }
                        }
                    }
                    break;
                }

            case ACTION_WELCOMETOTHESLAMJAM: {
                    if (body.NPC.ai[1] <= 0f) {
                        goto default;
                    }

                    if (body.NPC.ai[1] > 90f) {
                        if (ActionTimer == 0) {
                            int floor = Helper.FindFloor(centerTile.X, centerTile.Y, 16);
                            if (floor != -1) {
                                if (Main.netMode != NetmodeID.Server) {
                                    Vector2 where = new((body.HandLeft.Center.X + body.HandRight.Center.X) / 2f, floor * 16f);
                                    SoundEngine.PlaySound(AequusSounds.largeSlam with { Volume = 0.5f, }, where);
                                    SoundEngine.PlaySound(AequusSounds.superAttack, where);
                                    ScreenShake.SetShake(80f, where: where);
                                    ScreenFlash.Flash.Set(where, 1f, multiplier: 0.75f);
                                }
                                ActionTimer++;
                            }
                        }
                        if (NPC.velocity.Y == 0f) {
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                var p = Helper.FindProjectile(ModContent.ProjectileType<CrabsonSlamProj>(), Main.myPlayer);
                                if (p != null) {
                                    p.ai[0]++;
                                    p.netUpdate = true;
                                }
                            }
                        }
                        if (NPC.velocity.Y < 31f)
                            NPC.velocity.Y += 2f + 2f * BattleProgress;
                        NPC.stepSpeed = 2f;
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, startingRotation, 0.1f);
                        NPC.noTileCollide = false;
                        break;
                    }
                    if (body.NPC.ai[1] > 85f) {
                        NPC.velocity *= 0.8f;
                        break;
                    }
                    NPC.velocity *= 0.9f;

                    var gotoPosition = GetRestingPosition(body.NPC, Vector2.Zero);
                    if (body.NPC.ai[1] > 50f) {

                        NPC.rotation = startingRotation + ((body.NPC.ai[1] - 50f) * -0.04f + 1f) * -NPC.direction;
                        gotoPosition.X -= body.NPC.ai[1] * NPC.direction;
                        gotoPosition.Y -= body.NPC.ai[1] * 2f;
                        NPC.Center = Vector2.Lerp(NPC.Center, gotoPosition, 0.1f);
                        break;
                    }
                    NPC.rotation = startingRotation + body.NPC.ai[1] * 0.02f * -NPC.direction;
                    GoTo(gotoPosition, 0.0001f, 0.005f);
                    NPC.velocity.Y = Math.Min(NPC.velocity.Y, -1f);
                }
                break;

            default: {

                    GoToDefaultPosition(body.NPC);
                    mouthAnimation *= 0.95f;
                    if (NPC.rotation != 0f) {
                        NPC.rotation = NPC.rotation.AngleLerp(startingRotation, 0.1f);
                        if (Math.Abs(NPC.rotation) <= 0.1f) {
                            NPC.rotation = 0f;
                        }
                    }
                    break;
                }
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 1200);
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
        return false;
    }

    public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects) {
        if (NPC.spriteDirection == 1) {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (!NPC.IsABestiaryIconDummy) {
            switch (SharedAction) {
                case ACTION_WELCOMETOTHESLAMJAM: {
                        if (Body.ai[1] > 60f && NPC.velocity.Y != 0f) {
                            float intensity = Math.Min(Body.ai[1] / 60f, 1f);
                            screenPos += new Vector2(Main.rand.NextFloat(-intensity, intensity), Main.rand.NextFloat(-intensity, intensity) * 2f);

                            var glowColor = new Color(0, 10, 250, 0);
                            foreach (var v in Helper.CircularVector(4, NPC.rotation)) {
                                DrawClaw(NPC, spriteBatch, screenPos + v * 8f * intensity, glowColor * intensity, mouthAnimation);
                            }

                            if (NPC.velocity.Y > 0f) {
                                for (int i = 0; i < 10; i++) {
                                    DrawClaw(NPC, spriteBatch, screenPos + NPC.velocity * i * 0.1f, glowColor * intensity * 0.33f, mouthAnimation);
                                }
                            }
                        }
                        break;
                    }
            }
        }
        DrawClaw(NPC, spriteBatch, screenPos, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), mouthAnimation);
        return false;
    }
}
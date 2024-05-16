using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.Graphics.Particles;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Content.Enemies.PollutedOcean.Eel;

[ModBiomes(typeof(PollutedOceanBiomeSurface), typeof(PollutedOceanBiomeUnderground))]
internal class Eel : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 3;
        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = false,
            Scale = 1f,
            CustomTexturePath = AequusTextures.Eel_Bestiary.Path,
        };
    }

    public override void SetDefaults() {
        NPC.lifeMax = 120;
        NPC.damage = 40;
        NPC.npcSlots = 3.5f;
        NPC.width = 22;
        NPC.height = 22;
        NPC.defense = 2;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        NPC.knockBackResist = 0f;
        NPC.behindTiles = true;
        NPC.value = Item.silver * 2;
        NPC.netAlways = true;
        NPC.waterMovementSpeed = 1f;
    }

    private void SpawnSegments() {
        int length = Main.rand.Next(8, 12);
        IEntitySource source = NPC.GetSource_FromThis();
        NPC lastNPC = NPC;

        for (int i = 0; i < length; i++) {
            int newNPC = NPC.NewNPC(source, (int)lastNPC.Center.X, (int)lastNPC.Bottom.Y + 16, Type, lastNPC.whoAmI, lastNPC.ai[0] + 1f, lastNPC.whoAmI);
            if (newNPC == Main.maxNPCs) {
                break;
            }

            lastNPC = Main.npc[newNPC];
            lastNPC.realLife = NPC.whoAmI;
        }

        lastNPC.ai[0] = -lastNPC.ai[0];
    }

    private void GetFrame() {
        if (NPC.ai[0] < 0f) {
            NPC.frame.Y = NPC.frame.Height * 2;
        }
        else if (NPC.ai[0] != 1f) {
            NPC.frame.Y = NPC.frame.Height * 1;
        }
        else {
            NPC.frame.Y = 0;
        }
    }

    public override void AI() {
        if (NPC.ai[0] == 0f) {
            NPC.ai[0]++;
            NPC.ai[1] = -1f;
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                SpawnSegments();
            }
        }

        if (NPC.localAI[0] == 0f) {
            GetFrame();
            NPC.localAI[0]++;
        }

        bool inGround = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
        bool inWater = Collision.WetCollision(NPC.position, NPC.width, NPC.height);
        if (inWater) {
            if (Collision.LavaCollision(NPC.position, NPC.width, NPC.height)) {
                NPC.noTileCollide = false;
            }

            if (Main.netMode != NetmodeID.Server) {
                Vector2 velocity = NPC.position - NPC.oldPosition;
                if (velocity.Length() > 3f) {
                    int bubbleChance = Math.Clamp(10 - (int)velocity.Length(), 1, 20);
                    if (Main.rand.NextBool(bubbleChance)) {
                        var bubble = ModContent.GetInstance<UnderwaterBubbleParticles>().New();
                        bubble.Location = NPC.Center + Vector2.Normalize(NPC.velocity).RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2) * 12f;
                        bubble.Frame = (byte)Main.rand.Next(4);
                        bubble.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.3f) + velocity * 0.1f;
                        bubble.UpLift = 0.001f;
                        bubble.Opacity = Main.rand.NextFloat(0.8f, 1f);
                    }
                }
            }
        }

        if (NPC.ai[0] == 1f || NPC.ai[1] < 0f) {
            NPC.TargetClosest(faceTarget: true);

            bool hasTarget = NPC.HasValidTarget;
            Player target = Main.player[NPC.target];
            float distance = NPC.Distance(target.Center);
            if (NPC.ai[2] == 0f && NPC.direction != NPC.oldDirection && hasTarget && distance < 96f) {
                if (NPC.ai[3] > 20f) {
                    NPC.ai[3] = 0f;
                    NPC.ai[2] = 1f;
                }
                NPC.netUpdate = true;
            }

            if (NPC.collideX) {
                NPC.velocity.X = -NPC.oldVelocity.X * 0.4f;
            }
            if (NPC.collideY) {
                NPC.velocity.Y = -NPC.oldVelocity.Y * 0.4f;
            }

            NPC.rotation = NPC.velocity.ToRotation();
            NPC.spriteDirection = Math.Sign(NPC.velocity.X);

            float minSpeed = 1f;

            switch (NPC.ai[2]) {
                case 1:
                case 0: {
                        float maxSpeed = 2f;
                        Vector2 wantedVelocity = new Vector2(NPC.spriteDirection, Helper.Oscillate(NPC.ai[3], -0.8f, 0.8f));
                        NPC.ai[3] += 0.08f;
                        bool canSeeTarget = hasTarget && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
                        if (hasTarget) {
                            Vector2 toTarget = NPC.DirectionTo(target.Center);
                            if (canSeeTarget) {
                                wantedVelocity = toTarget;
                                maxSpeed = 32f;
                                minSpeed = 2f;
                            }
                            else {
                                wantedVelocity = Vector2.Lerp(wantedVelocity, toTarget, 0.3f);
                            }
                        }

                        wantedVelocity = Utils.SafeNormalize(wantedVelocity, Vector2.UnitX);

                        SoundStyle zapSound = new SoundStyle($"Terraria/Sounds/Dig_0") with { Volume = 0.8f, Pitch = -0.6f, PitchVariance = 0f, };
                        if (distance < 200f && canSeeTarget && NPC.ai[2] == 0f) {
                            if (NPC.soundDelay == 0) {
                                NPC.soundDelay = Math.Max((int)(distance / 4f), 6);
                                SoundEngine.PlaySound(zapSound, NPC.Center);
                            }
                        }

                        if (inWater || inGround) {
                            if (NPC.ai[2] == 1f) {
                                float attackTime = 120f;

                                NPC.velocity *= 0.9f;
                                minSpeed = 0.5f + NPC.ai[3] / attackTime;
                                if (NPC.ai[3] == 0f) {
                                    NPC.localAI[1] = 1f;
                                }

                                float oldZapFrame = NPC.localAI[1];
                                NPC.localAI[1] = (NPC.localAI[1] + 0.33f) % 2f;

                                if (oldZapFrame < 1f && NPC.localAI[1] > 1f) {
                                    SoundEngine.PlaySound(zapSound, NPC.Center);
                                }

                                NPC.ai[3]++;
                                if (NPC.ai[3] > attackTime) {
                                    NPC.ai[3] = 0f;
                                    NPC.ai[2] = 2f;
                                }
                            }

                            NPC.velocity.X += wantedVelocity.X * 0.1f;
                            NPC.velocity.Y += wantedVelocity.Y * 0.05f;
                            if (NPC.velocity.Length() > maxSpeed) {
                                NPC.velocity *= 0.9f;
                            }
                        }
                        else {
                            NPC.localAI[1] = 0f;
                            NPC.velocity.X += wantedVelocity.X * 0.03f;
                            NPC.velocity.Y += 0.08f;
                            if (NPC.velocity.Y > 8f) {
                                NPC.velocity.Y = 8f;
                            }
                        }
                    }
                    break;

                case 2: {
                        NPC.localAI[1] = 0f;

                        NPC.ai[3]++;
                        if (NPC.ai[3] > 20f) {
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = -10f;
                        }
                    }
                    break;
            }

            if (NPC.velocity.Length() < minSpeed) {
                NPC.velocity = Utils.SafeNormalize(NPC.velocity, Vector2.UnitX) * minSpeed;
            }

            return;
        }

        NPC.dontCountMe = true;
        NPC parentNPC = Main.npc[(int)NPC.ai[1]];

        if (!parentNPC.active) {
            NPC.KillEffects(quiet: true);
            return;
        }

        Vector2 wantedVector = Vector2.Normalize(parentNPC.velocity).UnNaN();
        Vector2 difference = parentNPC.Center - NPC.Center;
        float wantedDistance = (parentNPC.Size.Length() + NPC.Size.Length()) / 4f;
        if (difference.Length() > wantedDistance) {
            NPC.Center = parentNPC.Center - Vector2.Normalize(difference) * wantedDistance - wantedVector;
            NPC.rotation = difference.ToRotation();
            NPC.velocity = Vector2.Normalize(difference);
            NPC.spriteDirection = Math.Sign(difference.X);
        }
    }

    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox) {
        if (NPC.localAI[0] == 1f) {
            npcHitbox.Inflate(NPC.width, NPC.height);
        }
        return true;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
        return NPC.ai[0] == 1f ? true : false;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.IsABestiaryIconDummy) {
            return true;
        }

        NPC parent = Main.npc.IndexInRange(NPC.realLife) ? Main.npc[NPC.realLife] : NPC;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawCoords = NPC.Center - screenPos;
        Rectangle frame = new Rectangle(NPC.frame.X + NPC.frame.Width / 4 * (int)parent.localAI[1], NPC.frame.Y, NPC.frame.Width / 4 - 2, NPC.frame.Height - 2);
        Vector2 origin = frame.Size() / 2f;
        float scale = NPC.scale;
        float rotation = NPC.rotation + (NPC.spriteDirection == -1 ? MathHelper.PiOver2 : -MathHelper.PiOver2);
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        spriteBatch.Draw(texture, drawCoords, frame, drawColor, rotation, origin, 1f, effects, 0f);
        spriteBatch.Draw(texture, drawCoords, frame.Frame(2, 0, -2, -2), Color.White, rotation, origin, 1f, effects, 0f);

        if (NPC.ai[0] == 1f && NPC.localAI[1] == 0f) {
            Vector2 eyePosition = drawCoords + new Vector2(-1f, 5f * NPC.spriteDirection).RotatedBy(rotation) * NPC.scale;
            float intensity = Math.Min(Vector2.Distance(eyePosition, DrawHelper.ScreenSize / 2f) / 600f, 1f);
            spriteBatch.Draw(AequusTextures.FlareSoft, eyePosition, null, new Color(180, 180, 60, 0) * intensity, 0f, AequusTextures.FlareSoft.Size() / 2f, new Vector2(2f, 1f) * NPC.scale * 0.3f, SpriteEffects.None, 0f);
            //DrawHelper.DrawMagicLensFlare(spriteBatch, eyePosition, Color.White, 0.3f);
        }
        return false;
    }
}

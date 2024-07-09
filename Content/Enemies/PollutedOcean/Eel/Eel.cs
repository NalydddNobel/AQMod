using AequusRemake.Content.Biomes.PollutedOcean;
using AequusRemake.Content.Graphics.Particles;
using AequusRemake.Content.Items.Tools.Keys;
using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Entities.Bestiary;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Content.Enemies.PollutedOcean.Eel;

[AutoloadBanner]
[BestiaryBiome<PollutedOceanBiomeSurface>()]
[BestiaryBiome<PollutedOceanBiomeUnderground>()]
internal class Eel : ModNPC {
    #region AI Values
    public const float IdleMinSpeed = 1f;
    public const float IdleMaxSpeed = 2f;
    public const float AttackMinSpeed = 2f;
    public const float AttackMaxSpeed = 32f;
    public const float AirXVelocityAccel = 0.03f;
    public const float WaterXVelocityAccel = 0.1f;
    public const float WaterYVelocityAccel = 0.05f;
    /// <summary>Multiplied by <see cref="Timer"/> to get the current position of the wave pattern used when the Eel is moving idly.</summary>
    public const float IdleWaveTimerMultiplier = 0.08f;
    /// <summary>The vertical magnitude of the wave used when the Eel is moving idly.</summary>
    public const float IdleWaveMagnitude = 0.8f;

    /// <summary>The delay (in ticks) before the eel can shock again. This delay is extended by -<see cref="ShockRecoilCooldown"/> after it performs its first shock.</summary>
    public const float ShockDelay = 250f;
    /// <summary>The delay (in ticks) before the eel exits the shock state.</summary>
    public const float ShockTime = 120f;
    /// <summary>Added cooldown before it can perform (in ticks) after the eel does a shock.</summary>
    public const float ShockRecoilCooldown = -125f;
    /// <summary>Delay before <see cref="State"/> returns to <see cref="Normal"/> state from <see cref="ExitShocking"/>.</summary>
    public const float ShockRecoilTime = 20f;

    public const float Gravity = 0.12f;
    public const float TerminalVelocity = 12f;
    #endregion

    #region States
    public const int Normal = 0;
    public const int Shocking = 1;
    public const int ExitShocking = 2;
    #endregion

    #region Properties
    /// <summary>Represents NPC.ai[0]. This value is the order the worm segment spawned in. Starts at 1 for the head, and increases for each next NPC. The tail has this same property, but it is negative.</summary>
    public int SegmentNumber { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
    /// <summary>Represents NPC.ai[1]. This value is the index in <see cref="Main.npc"/> for the next NPC in the worm chain. It is -1 for the head.</summary>
    public int NextSegment { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }
    /// <summary>Represents NPC.ai[2]. This value represents the Head's current state. Unused by the body segments.</summary>
    public int State { get => (int)NPC.ai[2]; set => NPC.ai[2] = value; }
    /// <summary>Represents NPC.ai[3]. This value represents the Head's current timer. Unused by the body segments.</summary>
    public float Timer { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

    /// <summary>Represents NPC.localAI[1]. This value is used as a frame offset in order to alternate between the zapping and regular frames. Use <see cref="RealHorizontalFrame"/> for drawing.</summary>
    public float HorizontalFrame { get => NPC.localAI[1]; set => NPC.localAI[1] = value; }
    /// <summary>Gets the real horizontal frame from the head NPC. (or <see cref="HorizontalFrame"/> if it is the head.)</summary>
    public int RealHorizontalFrame => (int)(NPC.realLife > -1 ? Main.npc[NPC.realLife].localAI[1] : HorizontalFrame);

    public bool InShockFrame => RealHorizontalFrame == 1;

    public bool IsHead => SegmentNumber == 1;
    public bool IsTail => SegmentNumber < 0;

    public static readonly Color EyeFlareColor = new Color(180, 180, 60, 0);
    #endregion

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 3;
        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = false,
            Scale = 1f,
            CustomTexturePath = AequusTextures.Eel_Bestiary.FullPath,
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

    #region AI
    public override void AI() {
        // Initialize worm.
        if (SegmentNumber == 0) {
            SegmentNumber = 1;
            NextSegment = -1;
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                SpawnSegments();
            }
        }

        bool inGround = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
        bool inWater = Collision.WetCollision(NPC.position, NPC.width, NPC.height);
        if (inWater && !inGround) {
            /* Collision is now always set to true in SetDefaults. 

            // Set collision to true if the worm is in Lava
            if (Collision.LavaCollision(NPC.position, NPC.width, NPC.height)) {
                NPC.noTileCollide = false;
            }
            */

            // Bubble effects
            if (Main.netMode != NetmodeID.Server) {
                BubbleEffects();
            }
        }

        if (IsHead) {
            HeadAI(inWater, inGround);
        }
        else {
            BodyAI();
        }
    }

    void HeadAI(bool inWater, bool inGround) {
        NPC.TargetClosest(faceTarget: true);

        bool hasTarget = NPC.HasValidTarget;
        Player target = Main.player[NPC.target];
        float distance = NPC.Distance(target.Center);

        if (NPC.collideX) {
            NPC.velocity.X = -NPC.oldVelocity.X * 0.4f;
        }
        if (NPC.collideY) {
            NPC.velocity.Y = -NPC.oldVelocity.Y * 0.4f;
        }

        if (NPC.soundDelay > 0) {
            NPC.soundDelay--;
        }

        NPC.rotation = NPC.velocity.ToRotation();
        NPC.spriteDirection = Math.Sign(NPC.velocity.X);

        float minSpeed = IdleMinSpeed;

        switch (State) {
            default: {
                    bool normalState = State == Normal;
                    if (normalState && NPC.direction != NPC.oldDirection && hasTarget && distance < 96f) {
                        if (Timer > ShockDelay) {
                            Timer = 0f;
                            State = Shocking;
                        }
                        NPC.netUpdate = true;
                    }

                    // Idle velocity.
                    Vector2 wantedVelocity = new Vector2(NPC.spriteDirection, sin(Timer * IdleWaveTimerMultiplier, -IdleWaveMagnitude, IdleWaveMagnitude));
                    Timer++;
                    // Idle speed
                    float maxSpeed = IdleMaxSpeed;
                    float speed = NPC.velocity.Length();
                    bool canSeeTarget = hasTarget && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

                    if (hasTarget) {
                        Vector2 toTarget = NPC.DirectionTo(target.Center);
                        if (canSeeTarget) {
                            // Override idle velocity with a direct line to the target.
                            wantedVelocity = toTarget;
                            maxSpeed = AttackMaxSpeed;
                            minSpeed = AttackMinSpeed;

                            if (speed > maxSpeed / 6f) {
                                if (NPC.soundDelay <= 0) {
                                    SoundEngine.PlaySound(AequusSounds.EelMoving with { Volume = 2f }, NPC.Center);
                                }
                                NPC.soundDelay = Math.Max(NPC.soundDelay, 16);
                            }
                        }
                        else {
                            wantedVelocity = Vector2.Lerp(wantedVelocity, toTarget, 0.3f);
                        }
                    }

                    wantedVelocity = Utils.SafeNormalize(wantedVelocity, Vector2.UnitX);

                    if (inWater || inGround) {
                        if (State == Shocking) {
                            float attackTime = ShockTime;

                            NPC.velocity *= 0.9f;
                            minSpeed = 0.5f + Timer / attackTime;
                            if (Timer == 0f) {
                                HorizontalFrame = 1f;
                            }

                            float oldZapFrame = HorizontalFrame;
                            HorizontalFrame = (HorizontalFrame + 0.33f) % 2f;
                            /*if (oldZapFrame < 1f && HorizontalFrame > 1f) {

                            }*/

                            Timer++;
                            if (Timer > attackTime) {
                                Timer = 0f;
                                State = ExitShocking;
                            }
                        }

                        NPC.velocity.X += wantedVelocity.X * WaterXVelocityAccel;
                        NPC.velocity.Y += wantedVelocity.Y * WaterYVelocityAccel;
                        if (speed > maxSpeed) {
                            NPC.velocity *= 0.9f;
                        }
                    }
                    else {
                        HorizontalFrame = 0f;
                        NPC.velocity.X += wantedVelocity.X * AirXVelocityAccel;
                        NPC.velocity.Y += Gravity;
                        if (NPC.velocity.Y > TerminalVelocity) {
                            NPC.velocity.Y = TerminalVelocity;
                        }
                    }
                }
                break;

            case ExitShocking: {
                    HorizontalFrame = 0f;

                    Timer++;
                    if (Timer > ShockRecoilTime) {
                        State = Normal;
                        Timer = -ShockRecoilCooldown;
                    }
                }
                break;
        }

        if (NPC.velocity.Length() < minSpeed) {
            NPC.velocity = Utils.SafeNormalize(NPC.velocity, Vector2.UnitX) * minSpeed;
        }
    }

    void BodyAI() {
        NPC.dontCountMe = true;
        NPC.wetCount = 1;
        NPC parentNPC = Main.npc[NextSegment];

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

    void BubbleEffects() {
        Vector2 velocity = NPC.position - NPC.oldPosition;
        if (velocity.Length() > 3f) {
            int bubbleChance = Math.Clamp(10 - (int)velocity.Length(), 1, 20);
            if (Main.rand.NextBool(bubbleChance)) {
                var bubble = UnderwaterBubbles.New();
                bubble.Location = NPC.Center + Vector2.Normalize(NPC.velocity).RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2) * 12f;
                bubble.Frame = (byte)Main.rand.Next(4);
                bubble.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.3f) + velocity * 0.1f;
                bubble.UpLift = 0.001f;
                bubble.Opacity = Main.rand.NextFloat(0.8f, 1f);
            }
        }
    }

    void SpawnSegments() {
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
    #endregion

    public override void FindFrame(int frameHeight) {
        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = false,
            Scale = 1f,
            Position = new Vector2(50f, 20f),
            PortraitPositionXOverride = 0f,
            CustomTexturePath = AequusTextures.Eel_Bestiary.FullPath,
        };

        // Initialize frame.
        if (NPC.localAI[0] == 0f) {
            SetFrame();
            NPC.localAI[0]++;
        }

        void SetFrame() {
            if (IsTail) {
                NPC.frame.Y = NPC.frame.Height * 2;
            }
            else if (!IsHead) {
                NPC.frame.Y = NPC.frame.Height * 1;
            }
            else {
                NPC.frame.Y = 0;
            }
        }
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        if (NPC.life <= 0) {
            IEntitySource source = NPC.GetSource_FromThis();
            for (int i = 0; i < 20; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * hit.HitDirection, -2.5f);
            }

            if (IsTail) {
                NPC.NewGore(AequusTextures.EekGoreTail, NPC.position, NPC.velocity, Scale: NPC.scale);
            }
            else if (!IsHead) {
                NPC.NewGore(AequusTextures.EekGoreBody, NPC.position, NPC.velocity, Scale: NPC.scale);
            }
            else {
                NPC.NewGore(AequusTextures.EekGoreHead, NPC.position, NPC.velocity, Scale: NPC.scale);
            }
        }
        else {
            for (int i = 0; i < hit.Damage / (double)NPC.lifeMax * 50f; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f);
            }
        }
    }

    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox) {
        // Expand hitbox if in the shock frame.
        if (InShockFrame) {
            npcHitbox.Inflate(NPC.width, NPC.height);
        }
        return true;
    }

    public override bool? CanFallThroughPlatforms() {
        return true;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Summon.Whips.EekWhip.ElectricEelWhip>(), chanceDenominator: 15));
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CopperKey>(), chanceDenominator: CopperKey.DropRate));
    }

    #region Drawing
    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
        return IsHead ? null : false;
    }

    private void DrawEye(SpriteBatch spriteBatch, Vector2 drawCoords, float rotation) {
        Vector2 eyePosition = drawCoords + new Vector2(-1f, 5f * NPC.spriteDirection).RotatedBy(rotation) * NPC.scale;
        float intensity = Math.Min(Vector2.Distance(eyePosition, DrawHelper.ScreenSize / 2f) / 600f, 1f);
        spriteBatch.Draw(AequusTextures.FlareSoft, eyePosition, null, EyeFlareColor * intensity, 0f, AequusTextures.FlareSoft.Size() / 2f, new Vector2(2f, 1f) * NPC.scale * 0.3f, SpriteEffects.None, 0f);
        //DrawHelper.DrawMagicLensFlare(spriteBatch, eyePosition, Color.White, 0.3f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.IsABestiaryIconDummy) {
            return true;
        }

        drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
        NPC parent = Main.npc.IndexInRange(NPC.realLife) ? Main.npc[NPC.realLife] : NPC;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawCoords = NPC.Center - screenPos;
        Rectangle frame = new Rectangle(NPC.frame.X + NPC.frame.Width / 4 * RealHorizontalFrame, NPC.frame.Y, NPC.frame.Width / 4 - 2, NPC.frame.Height - 2);
        Vector2 origin = frame.Size() / 2f;
        float scale = NPC.scale;
        float rotation = NPC.rotation + (NPC.spriteDirection == -1 ? MathHelper.PiOver2 : -MathHelper.PiOver2);
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        spriteBatch.Draw(texture, drawCoords, frame, drawColor, rotation, origin, 1f, effects, 0f);
        spriteBatch.Draw(texture, drawCoords, frame.Frame(2, 0, -2, -2), Color.White, rotation, origin, 1f, effects, 0f);

        if (IsHead && !InShockFrame) {
            DrawEye(spriteBatch, drawCoords, rotation);
        }
        return false;
    }
    #endregion
}

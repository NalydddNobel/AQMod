using Aequus.Core.ContentGeneration;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

[AutoloadBanner]
public partial class Conductor : ModNPC {
    /// <summary>Targetting state. The conductor walks towards the player and activates his attack here.</summary>
    public const int A_TARGETING = 0;

    /// <summary>Upon ai[1] reaching this value, set <see cref="State"/> to <see cref="A_TELEPORT"/>.</summary>
    public static int TARGETTING_TELEPORT_TIME_THRESHOLD = -60;
    /// <summary>This tick value is subtracted to ai[1] every AI tick when the Conductor cannot see the player. Upon reaching <see cref="TARGETTING_TELEPORT_TIME_THRESHOLD"/>, set <see cref="State"/> to <see cref="A_TELEPORT"/>.</summary>
    public static float TARGETTING_TELEPORT_TICK = 0.2f;
    /// <summary>Upon ai[1] reaching this value, set <see cref="State"/> to <see cref="A_ATTACKING"/>.</summary>
    public static int TARGETTING_ATTACK_TIME_THRESHOLD = 120;

    public const int A_ATTACKING = 1;

    /// <summary>Dodge state. The Conductor slides backwards to avoid melee confrontation with the player. 
    /// Upon the state ending if the distance is greater than <see cref="MELEE_DISTANCE"/>, set <see cref="State"/> to <see cref="A_TARGETING"/>, otherwise <see cref="A_TELEPORT"/>.</summary>
    public const int A_SLIDE_BACK = 2;

    /// <summary>How long the Conductor must be in the <see cref="A_SLIDE_BACK"/> state even if he has no horizontal velocity.</summary>
    public static int SLIDE_BACK_REQUIRED_TIME = 50;
    /// <summary>How many times the Conductor can enter the <see cref="A_SLIDE_BACK"/> state. Tracked using ai[2]. ai[2] is reset in <see cref="A_ATTACKING"/>.</summary>
    public static int SLIDE_BACK_COUNT = 1;
    /// <summary>Sets ai[1] (attack delay timer) to this value upon entering <see cref="A_TARGETING"/>. This makes the Conductor enter the attack state faster.</summary>
    public static int SLIDE_ATTACK_AGGRESSION = TARGETTING_ATTACK_TIME_THRESHOLD / 2 + 10;
    public static float SLIDE_BACK_VELOCITY_MULTIPLIER = 0.9f;
    public static float SLIDE_BACK_SPEED = 16f;

    public const int A_TELEPORT = 3;
    /// <summary>How many update ticks to waste teleporting when <see cref="Main.zenithWorld"/> equals <see langword="true"/>.</summary>
    public static int GFB_TELEPORT_SPAM = 40;

    /// <summary>The distance in pixels which is considered "Melee distance"</summary>
    public static int MELEE_DISTANCE = 96;

    private int State { 
        get => (int)NPC.ai[0]; 
        set {
            NPC.ai[0] = value;
            NPC.ai[1] = 0f;
            NPC.netUpdate = true;
        } 
    }

    public override void AI() {
        switch (State) {
            case A_TARGETING: {
                    if (CheckForceTP()) {
                        break;
                    }

                    bool walk = false;
                    NPC.TargetClosest(faceTarget: true);
                    if (NPC.ai[1] < TARGETTING_TELEPORT_TIME_THRESHOLD) {
                        State = A_TELEPORT;
                    }
                    else if (NPC.HasValidTarget && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height)) {
                        NPC.ai[1]++;
                        if (NPC.ai[1] > TARGETTING_ATTACK_TIME_THRESHOLD) {
                            State = A_ATTACKING;
                            NPC.ai[2] = 0f;
                        }
                        float distance = NPC.Distance(Main.player[NPC.target].Center);
                        if (distance < MELEE_DISTANCE && NPC.ai[2] < SLIDE_BACK_COUNT) {
                            State = A_SLIDE_BACK;
                        }
                        else if (NPC.ai[3] < 2f && distance > MELEE_DISTANCE * 3f) {
                            walk = true;
                        }
                    }
                    else {
                        walk = true;
                        NPC.ai[3]--;
                        NPC.ai[1] -= TARGETTING_TELEPORT_TICK;
                    }

                    if (walk) {
                        NPC.localAI[0] = 1f;
                        NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X + NPC.direction * 0.066f, -2f, 2f);
                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
                    }
                    else {
                        NPC.localAI[0] = 0f;
                        if (NPC.velocity.Y == 0f) {
                            NPC.velocity.X *= 0.9f;
                        }
                    }
                }
                break;

            case A_ATTACKING: {
                    if (CheckForceTP()) {
                        break;
                    }

                    if (NPC.velocity.Y == 0f) {
                        NPC.velocity.X *= 0.66f;
                    }

                    NPC.TargetClosest(faceTarget: true);

                    if ((int)NPC.ai[1] == 0) {
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    }

                    if (NPC.ai[1] > 240f) {
                        State = A_TELEPORT;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }

                    NPC.ai[1]++;
                }
                break;

            case A_SLIDE_BACK: {
                    if (CheckForceTP()) {
                        break;
                    }

                    NPC.velocity.X *= SLIDE_BACK_VELOCITY_MULTIPLIER;

                    if (NPC.ai[1] > SLIDE_BACK_REQUIRED_TIME && Math.Abs(NPC.velocity.X) < 1f) {
                        NPC.velocity.X = 0f;
                        NPC.TargetClosest(faceTarget: true);
                        if (NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < MELEE_DISTANCE) {
                            State = A_TELEPORT;
                        }
                        else {
                            State = A_TARGETING;
                            NPC.ai[1] = 40f;
                        }
                    }

                    if ((int)NPC.ai[1] == 0) {
                        NPC.TargetClosest(faceTarget: true);
                        NPC.ai[2]++;
                        NPC.velocity.X = SLIDE_BACK_SPEED * -NPC.direction;
                    }

                    NPC.ai[1]++;
                }
                break;

            case A_TELEPORT: {
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        return;
                    }

                    NPC.TargetClosest();
                    if (!NPC.HasValidTarget) {
                        NPC.KillEffects();
                        return;
                    }

                    Player target = Main.player[NPC.target];
                    Point teleportLocation = target.Center.ToTileCoordinates();
                    Vector2 teleportResult = Vector2.Zero;
                    for (int i = 0; i < 10000; i++) {
                        int x = Math.Clamp(teleportLocation.X + Main.rand.Next(-40, 41), 40, Main.maxTilesX - 40);
                        int y = Math.Clamp(teleportLocation.Y + Main.rand.Next(-25, 26), 40, Main.maxTilesX - 40);

                        if (!Main.tile[x, y].IsFullySolid()) {
                            continue;
                        }

                        i += 20;
                        Vector2 position = new Vector2(x * 16f + 8f - NPC.width / 2f, y * 16f - NPC.height);
                        if (Collision.SolidCollision(position, NPC.width, NPC.height - 2)) {
                            continue;
                        }

                        i += 40;
                        int closestPlayer = Player.FindClosest(position, NPC.width, NPC.height);
                        Player closest = Main.player[closestPlayer];
                        if (!closest.active || closest.DeadOrGhost || Vector2.Distance(position, closest.Center) < 120f) {
                            continue;
                        }

                        //Dust.NewDustPerfect(position, DustID.Torch);
                        if (Collision.CanHitLine(position, NPC.width, NPC.height, closest.position, closest.width, closest.height)) {
                            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                            for (int k = 0; k < 40; k++) {
                                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Iron);
                                d.noGravity = true;
                                d.velocity *= 0.5f;
                            }
                            NPC.position = position;
                            NPC.velocity = Vector2.Zero;
                            NPC.ai[3]++;
                            NPC.TargetClosest(faceTarget: true);
                            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                            for (int k = 0; k < 40; k++) {
                                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Iron);
                                d.noGravity = true;
                                d.velocity *= 0.5f;
                            }

                            if (Main.zenithWorld && NPC.ai[1] < GFB_TELEPORT_SPAM) {
                                NPC.ai[1]++;
                                break;
                            }

                            State = A_TARGETING;
                            break;
                        }
                    }
                }
                break;
        }

        NPC.spriteDirection = NPC.direction;
    }

    private bool CheckForceTP() {
        Point bottom = NPC.Bottom.ToTileCoordinates();
        int tileWidth = (int)Math.Ceiling(NPC.width / 16f);
        if (!TileHelper.ScanTiles(new Rectangle(bottom.X, bottom.Y, tileWidth, 3), TileHelper.IsSolid)) {
            State = A_TELEPORT;
            return true;
        }

        return false;
    }

    public override void FindFrame(int frameHeight) {
        int frame = 0;
        switch (State) {
            case A_TARGETING: {
                    if (NPC.localAI[0] < 1f) {
                        frame = 0;
                    }
                    else {
                        NPC.frameCounter += Math.Abs(NPC.velocity.X) * 0.8f;
                        if (NPC.frameCounter > 24.0) {
                            NPC.frameCounter = 0;
                        }
                        frame = 1 + (int)Math.Min(NPC.frameCounter / 6, 3.0);
                    }
                }
                break;

            case A_ATTACKING: {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 60.0) {
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frameCounter >= 30.0) {
                        frame = 13 - (int)Math.Min((NPC.frameCounter - 30.0) / 5, 6.0);
                    }
                    else {
                        frame = 7 + (int)Math.Min(NPC.frameCounter / 5, 6.0);
                    }
                }
                break;

            case A_SLIDE_BACK: {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 6.0) {
                        NPC.frameCounter = 0;
                    }
                    frame = 5 + (int)Math.Min(NPC.frameCounter / 3, 1.0);
                }
                break;

            case A_TELEPORT: {
                }
                break;
        }

        NPC.frame.Y = frame * frameHeight;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Rectangle frame = NPC.frame;
        Vector2 origin = frame.Size() / 2f;
        origin.X -= 14f * NPC.spriteDirection;
        Vector2 drawCoordinates = NPC.Bottom + new Vector2(0f, origin.Y - frame.Height + 4f + NPC.gfxOffY);
        drawColor = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor));
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        spriteBatch.Draw(texture, drawCoordinates - screenPos, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
        return false;
    }
}
